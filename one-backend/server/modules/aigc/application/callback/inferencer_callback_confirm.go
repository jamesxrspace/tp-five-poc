package callback

// Package callback provides functionality to handle AWS SNS confirmation requests.
// This file is designed to be used when an endpoint subscribes to an AWS SNS (Simple Notification Service) topic.
// The primary purpose of this file is to confirm the subscription between the endpoint and the SNS topic,
// ensuring that the endpoint receives messages from the subscribed topic.
//
// This code should be executed only once when an endpoint initially subscribes to an AWS SNS topic.
// After successful subscription confirmation, the endpoint becomes eligible to receive notifications from the topic.
//
// For more information about AWS SNS, visit: https://docs.aws.amazon.com/sns/latest/dg/SendMessageToHttp.prepare.html

import (
	"context"

	sns_client "xrspace.io/server/core/dependency/sns"
	"xrspace.io/server/modules/aigc/domain/define"
)

type InferencerCallbackConfirmation struct {
	snsClient *sns_client.SNSClient
}

type ConfirmaCommand struct {
	Token    string `json:"Token" binding:"required" validate:"required"`
	TopicArn string `json:"TopicArn" binding:"required" validate:"required"`
}

func NewInferencerCallbackConfirmation(
	snsClient *sns_client.SNSClient) *InferencerCallbackConfirmation {
	return &InferencerCallbackConfirmation{
		snsClient: snsClient,
	}
}

func (i *InferencerCallbackConfirmation) Execute(ctx context.Context, cmdz any) (any, error) {
	command := cmdz.(*ConfirmaCommand)
	err := i.snsClient.ConfirmSubscription(command.TopicArn, command.Token)
	if err != nil {
		return nil, err
	}

	return define.OK, nil
}
