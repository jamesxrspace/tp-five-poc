package domain

import (
	"context"
	"time"
)

type IQueue interface {
	Push(ctx context.Context, key string, values string) error
	BlockLeftPop(ctx context.Context, key string, timeout time.Duration) (string, error)
}
