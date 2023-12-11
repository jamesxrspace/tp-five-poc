package sagemaker

import (
	"context"

	"xrspace.io/server/modules/aigc/domain"
)

var _ domain.IInferencer = (*AigcInferencer)(nil)

type AigcInferencer struct {
}

func NewAigcInferencer() *AigcInferencer {
	return &AigcInferencer{}
}

func (a *AigcInferencer) Inference(ctx context.Context, inputLocation string) (string, error) {
	return "test_id", nil
}
