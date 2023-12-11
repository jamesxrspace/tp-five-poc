package database

import (
	"context"
	"time"

	"github.com/redis/go-redis/extra/redisotel/v9"
	"github.com/redis/go-redis/v9"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/settings"
)

var _ IRedis = (*RedisClient)(nil)

type RedisClient struct {
	client *redis.Client
}

func NewRedis(ctx context.Context, config *settings.RedisConfig) (*RedisClient, error) {
	client := redis.NewClient(&redis.Options{
		Addr:     config.Address,
		Password: config.Password,
		DB:       *config.DB,
	})

	if err := redisotel.InstrumentTracing(client); err != nil {
		panic(err)
	}

	_, err := client.Ping(ctx).Result()
	if err != nil {
		return nil, core_error.StackError(err)
	}
	rtn := &RedisClient{
		client: client,
	}
	return rtn, nil
}

func (c *RedisClient) Ping(ctx context.Context) error {
	_, err := c.client.Ping(ctx).Result()
	if err != nil {
		return core_error.StackError(err)
	}
	return nil
}

func (c *RedisClient) Get(ctx context.Context, key string) string {
	return c.client.Get(ctx, key).Val()
}

func (c *RedisClient) LRange(ctx context.Context, key string) ([]string, error) {
	rtn, err := c.client.LRange(ctx, key, 0, -1).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) LPush(ctx context.Context, key string, data string) (int64, error) {
	rtn, err := c.client.LPush(ctx, key, data).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) RPush(ctx context.Context, key string, data string) (int64, error) {
	rtn, err := c.client.RPush(ctx, key, data).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) BLPop(ctx context.Context, duration time.Duration, key ...string) ([]string, error) {
	rtn, err := c.client.BLPop(ctx, duration, key...).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) IncrBy(ctx context.Context, key string, increment int64) (int64, error) {
	rtn, err := c.client.IncrBy(ctx, key, increment).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) Set(ctx context.Context, key string, value string, expiration time.Duration) (string, error) {
	rtn, err := c.client.Set(ctx, key, value, expiration).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) Expire(ctx context.Context, key string, duration time.Duration) (bool, error) {
	rtn, err := c.client.Expire(ctx, key, duration).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}

func (c *RedisClient) Del(ctx context.Context, keys ...string) (int64, error) {
	rtn, err := c.client.Del(ctx, keys...).Result()
	if err != nil {
		return rtn, core_error.StackError(err)
	}
	return rtn, nil
}
