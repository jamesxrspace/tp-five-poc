package domain

import "context"

type IInferencer interface {
	Inference(ctx context.Context, inputLocation string) (string, error)
}
