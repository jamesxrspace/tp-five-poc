package docdbLocal

import (
	"context"

	"github.com/joho/godotenv"

	"xrspace.io/server/core/dependency/unit_of_work"
)

type UnitOfWork struct {
}

func init() {
	_ = godotenv.Load("../../../../../.env")
}

func NewUnitOfWork() unit_of_work.IUnitOfWork {
	return &UnitOfWork{}
}

func (w *UnitOfWork) WithTransaction(ctx context.Context, fn func(ctx context.Context) (any, error)) (any, error) {
	return fn(ctx)
}
