package application

import "context"

type IUseCase interface {
	Execute(ctx context.Context, cmd any) (any, error)
}
