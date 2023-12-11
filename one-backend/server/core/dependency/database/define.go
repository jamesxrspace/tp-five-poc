package database

import (
	"context"
	"time"
)

type IRedis interface {
	// String
	Get(ctx context.Context, key string) string

	// List
	LRange(ctx context.Context, key string) ([]string, error)
	LPush(ctx context.Context, key string, data string) (int64, error)
	RPush(ctx context.Context, key string, data string) (int64, error)
	BLPop(ctx context.Context, duration time.Duration, key ...string) ([]string, error)

	IncrBy(ctx context.Context, key string, increment int64) (int64, error)

	Set(ctx context.Context, key string, value string, expiration time.Duration) (string, error)
	Expire(ctx context.Context, key string, duration time.Duration) (bool, error)
	Del(ctx context.Context, keys ...string) (int64, error)
	Ping(ctx context.Context) error
}
