package sagemaker

import (
	"context"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/sagemakerruntime"
	"xrspace.io/server/core/arch/core_error"
)

type SageMakerRuntimeClient struct {
	client *sagemakerruntime.SageMakerRuntime
}

func NewSageMakerRuntimeClient(awsSession *session.Session) *SageMakerRuntimeClient {
	return &SageMakerRuntimeClient{
		client: sagemakerruntime.New(awsSession),
	}
}

func (c *SageMakerRuntimeClient) InvokeEndpointAsyncWithContext(
	ctx context.Context,
	endPointName string,
	inputLocation string,
) (*sagemakerruntime.InvokeEndpointAsyncOutput, error) {
	response, err := c.client.InvokeEndpointAsyncWithContext(
		ctx,
		&sagemakerruntime.InvokeEndpointAsyncInput{
			EndpointName:  aws.String(endPointName),
			InputLocation: aws.String(inputLocation),
		})

	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	return response, nil
}
