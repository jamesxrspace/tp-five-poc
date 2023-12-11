package sns_client

import (
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/sns"
	"xrspace.io/server/core/arch/core_error"
)

type SNSClient struct {
	client *sns.SNS
}

type SubscriptionConfirmation struct {
	Token    string `json:"Token"`
	TopicArn string `json:"TopicArn"`
}

func NewSNSClient(awsSession *session.Session) *SNSClient {
	return &SNSClient{
		client: sns.New(awsSession),
	}
}

func (c *SNSClient) ConfirmSubscription(topicArn string, token string) error {
	_, err := c.client.ConfirmSubscription(&sns.ConfirmSubscriptionInput{
		TopicArn: aws.String(topicArn),
		Token:    aws.String(token),
	})
	return core_error.NewInternalError(err)
}
