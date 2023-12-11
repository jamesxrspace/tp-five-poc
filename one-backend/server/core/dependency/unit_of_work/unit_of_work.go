package unit_of_work

import "context"

type IUnitOfWork interface {
	WithTransaction(ctx context.Context, fn func(sc context.Context) (any, error)) (any, error)
}
