package repository

import "context"

type IDomainRepository[T any] interface {
	Save(ctx context.Context, item T) error
	Get(ctx context.Context, id string) (T, error)
}
