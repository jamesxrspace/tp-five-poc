package controller

import (
	"github.com/gin-gonic/gin"
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/modules/streaming/application"
	"xrspace.io/server/modules/streaming/application/command"
	"xrspace.io/server/modules/streaming/domain/rtc_token_provider"
)

type StreamingController struct {
	facade *application.Facade
}

func NewStreamingController(rtcTokenProvider rtc_token_provider.IRTCTokenProvider) *StreamingController {
	return &StreamingController{
		facade: application.NewFacade(rtcTokenProvider),
	}
}

func (c *StreamingController) GenerateRTCToken(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.GenerateRTCTokenCommand{})
}
