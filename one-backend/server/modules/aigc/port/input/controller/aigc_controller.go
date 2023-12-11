package controller

/*
                               ┌────────┐
                               │        │
                               │ Redis  │
                               │        │
                               └─┬────▲─┘
                                 │    │
                7.Pulling result │    │6.Update result
                  with Request ID│    │  with Request ID
                                 │    │
                                 │    │
┌────────┐   1.Music S3 URL    ┌─▼────┴─┐    2.Invoke SageMaker    ┌───────────┐
│        ├────────────────────►│        ├─────────────────────────►│           │
│ Client │                     │ Server │                          │ SageMaker │
│        │◄────────────────────┤        │◄─────────────────────────┤           │
└────────┘   8.Return Motion   └────▲───┘    3.Request ID          └─────┬─────┘
               S3 URL               │                                    │
                                    │                                    │ 4.Send Result
                                    │                                    │
                                    │                              ┌─────▼─────┐
                                    │ 5.Publish Result to callback │           │
                                    └──────────────────────────────┤  AWS SNS  │
                                                                   │           │
                                                                   └───────────┘
*/

import (
	"strings"

	"github.com/gin-gonic/gin"
	portGin "xrspace.io/server/core/arch/port/gin"
	sns_client "xrspace.io/server/core/dependency/sns"
	"xrspace.io/server/modules/aigc/application"
	"xrspace.io/server/modules/aigc/application/callback"
	"xrspace.io/server/modules/aigc/application/command"
	"xrspace.io/server/modules/aigc/application/mock"
	"xrspace.io/server/modules/aigc/domain"
	"xrspace.io/server/modules/aigc/domain/define"
)

const msgTypeHeader = "X-Amz-Sns-Message-Type"
const confirmType = "SubscriptionConfirmation"

type AigcController struct {
	facade *application.Facade
	env    string
}

func NewAigcController(
	queue domain.IQueue,
	storage domain.IStorage,
	inferencer domain.IInferencer,
	snsClient *sns_client.SNSClient,
	env string) *AigcController {
	return &AigcController{
		facade: application.NewFacade(queue, storage, inferencer, snsClient),
		env:    env,
	}
}

func (c *AigcController) GenerateMotion(ctx *gin.Context) {
	if strings.ToLower(c.env) == define.LOCAL {
		portGin.Handle(ctx, c.facade, &mock.GenerateMotionCommand{})
		return
	}
	portGin.Handle(ctx, c.facade, &command.GenerateMotionCommand{})
}

func (c *AigcController) GenerateMockMotion(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &mock.GenerateMotionCommand{})
}

func (c *AigcController) AwsSnsHandler(ctx *gin.Context) {
	// The "Content-Type" ctx.Request is actually json, but the sender
	// did not set it up correctly.
	// Set content type to Json to make gin binding right.
	ctx.Request.Header.Set("Content-Type", "application/json")
	if msgType := ctx.Request.Header.Get(msgTypeHeader); msgType == confirmType {
		portGin.Handle(ctx, c.facade, &callback.ConfirmaCommand{})
		return
	}

	portGin.Handle(ctx, c.facade, &callback.Payload{})
}
