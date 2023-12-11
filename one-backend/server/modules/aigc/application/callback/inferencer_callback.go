package callback

import (
	"context"
	"encoding/json"

	"xrspace.io/server/core/arch/core_error"
	sns_client "xrspace.io/server/core/dependency/sns"
	"xrspace.io/server/modules/aigc/domain"
	"xrspace.io/server/modules/aigc/domain/define"
)

type InferencerCallback struct {
	snsClient *sns_client.SNSClient
	queue     domain.IQueue
}

type Payload struct {
	Message string `json:"Message" binding:"required"`
}

type Message struct {
	Status             string             `json:"invocationStatus"`
	InferenceId        string             `json:"inferenceId"`
	FailureReason      string             `json:"failureReason"`
	ResponseParameters responseParameters `json:"responseParameters"`
}

type responseParameters struct {
	OutputLocation string `json:"outputLocation"`
}

func NewInferencerCallback(queue domain.IQueue, snsClient *sns_client.SNSClient) *InferencerCallback {
	return &InferencerCallback{
		queue:     queue,
		snsClient: snsClient,
	}
}

func (i *InferencerCallback) Execute(ctx context.Context, cmdz any) (any, error) {
	command := cmdz.(*Payload)

	var message Message
	err := json.Unmarshal([]byte(command.Message), &message)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	resp := define.InferenceResponse{
		OutputLocation: message.ResponseParameters.OutputLocation,
		Status:         message.Status,
		Error:          message.FailureReason,
	}

	jsonString, err := json.Marshal(resp)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	err = i.queue.Push(ctx, message.InferenceId, string(jsonString))
	if err != nil {
		return nil, err
	}

	return define.OK, nil
}
