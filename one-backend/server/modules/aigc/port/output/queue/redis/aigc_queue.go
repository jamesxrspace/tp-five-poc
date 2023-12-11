package redis

import (
	"context"
	"time"

	"xrspace.io/server/core/dependency/database"
	"xrspace.io/server/modules/aigc/domain"
)

var _ domain.IQueue = (*AigcQueue)(nil)

type AigcQueue struct {
	redis database.IRedis
}

func NewAigcQueue(redis database.IRedis) *AigcQueue {
	return &AigcQueue{
		redis: redis,
	}
}

func (q *AigcQueue) Push(ctx context.Context, key string, values string) error {
	_, err := q.redis.RPush(ctx, key, values)
	return err
}

func (q *AigcQueue) BlockLeftPop(ctx context.Context, key string, timeout time.Duration) (string, error) {
	result, err := q.redis.BLPop(ctx, timeout, key)
	if err != nil {
		return "", err
	}
	// result[0] is the list name, result[1] is the popped message.
	return result[1], nil
}
