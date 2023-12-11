package sagemaker

import (
	"context"

	sagemaker_runtime "xrspace.io/server/core/dependency/sagemaker"
	"xrspace.io/server/modules/aigc/domain"
)

var _ domain.IInferencer = (*AigcInferencer)(nil)

type AigcInferencer struct {
	sgClient     *sagemaker_runtime.SageMakerRuntimeClient
	endPointName string
}

func NewAigcInferencer(sgClient *sagemaker_runtime.SageMakerRuntimeClient, endpointName string) *AigcInferencer {
	return &AigcInferencer{
		sgClient:     sgClient,
		endPointName: endpointName,
	}
}

func (a *AigcInferencer) Inference(ctx context.Context, inputLocation string) (string, error) {
	s3Url := "s3://" + inputLocation
	output, err := a.sgClient.InvokeEndpointAsyncWithContext(ctx, a.endPointName, s3Url)

	if err != nil {
		return "", err
	}

	return *output.InferenceId, nil
}
