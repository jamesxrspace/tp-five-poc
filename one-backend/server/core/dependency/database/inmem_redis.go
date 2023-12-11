package database

import (
	"context"
	"errors"
	"strconv"
	"sync"
	"time"
)

var _ IRedis = (*InmemRedis)(nil)

// Deprecated: this will be replace when find a mock Redis package.
type InmemRedis struct {
	data   map[string][]string
	locker *sync.RWMutex
}

func (c *InmemRedis) Ping(ctx context.Context) error {
	return nil
}

func (c *InmemRedis) Expire(ctx context.Context, key string, duration time.Duration) (bool, error) {
	go func() {
		time.Sleep(duration)
		_, _ = c.Del(ctx, key)
	}()
	time.Sleep(time.Duration(100) * time.Millisecond)
	return true, nil
}

func (m *InmemRedis) Del(ctx context.Context, keys ...string) (int64, error) {
	m.locker.Lock()
	defer m.locker.Unlock()
	for _, key := range keys {
		delete(m.data, key)
	}
	return 0, nil
}

func (c *InmemRedis) LRange(ctx context.Context, key string) ([]string, error) {
	c.locker.Lock()
	defer c.locker.Unlock()

	value, existed := c.data[key]
	if !existed {
		return nil, errors.New("key not existed")
	}

	return value, nil
}

func (c *InmemRedis) Get(ctx context.Context, key string) string {
	c.locker.RLock()
	defer c.locker.RUnlock()
	if len(c.data[key]) > 0 {
		return c.data[key][0]
	}
	return ""
}

func (c *InmemRedis) LPush(ctx context.Context, key string, data string) (int64, error) {
	c.locker.Lock()
	defer c.locker.Unlock()
	if c.data[key] == nil {
		c.data[key] = []string{data}
		return 1, nil
	}

	c.data[key] = append([]string{data}, c.data[key]...)
	return 1, nil
}

func (c *InmemRedis) RPush(ctx context.Context, key string, data string) (int64, error) {
	c.locker.Lock()
	defer c.locker.Unlock()
	if c.data[key] == nil {
		c.data[key] = []string{data}
		return 1, nil
	}

	c.data[key] = append(c.data[key], data)
	return 1, nil
}

func (c *InmemRedis) BLPop(ctx context.Context, duration time.Duration, key ...string) ([]string, error) {
	c.locker.Lock()
	defer c.locker.Unlock()

	// Create a map to store the channels waiting for keys.
	waitingChannels := make(map[string]chan []string)

	// Start a timer for the specified duration.
	timer := time.After(duration)

	// Iterate through the keys and check if they have items in the data store.
	for _, k := range key {
		if len(c.data[k]) > 0 {
			items := c.data[k][1:]
			return items, nil
		}

		// If the key has no items, create a channel for it and add it to the waitingChannels map.
		waitingChannels[k] = make(chan []string)
	}

	// Check for any available items within the specified duration.
	for {
		select {
		case <-timer:
			return nil, errors.New("timeout")
		default:
			// Iterate through the waitingChannels and check if any of them have received items.
			for k, ch := range waitingChannels {
				select {
				case items := <-ch:
					// Delete the channel from waitingChannels.
					delete(waitingChannels, k)
					return items, nil
				default:
				}
			}
		}
	}
}

func (c *InmemRedis) IncrBy(ctx context.Context, key string, increment int64) (int64, error) {
	c.locker.Lock()
	defer c.locker.Unlock()

	currentValueString, exists := c.data[key]

	if !exists {
		incrementString := strconv.Itoa(int(increment))
		c.data[key] = []string{string(incrementString)}
		return 1, nil
	}

	currentValue, err := strconv.Atoi(currentValueString[0])
	updateValue := currentValue + int(increment)
	c.data[key] = []string{strconv.Itoa(updateValue)}
	if err != nil {
		return 0, err
	}

	return 1, nil
}

func (c *InmemRedis) Set(ctx context.Context, key string, value string, expiration time.Duration) (string, error) {
	_, _ = c.Expire(ctx, key, expiration)
	return c.SetStrings(key, value)
}

func (c *InmemRedis) SetStrings(key, value string) (string, error) {
	c.locker.Lock()
	defer c.locker.Unlock()
	c.data[key] = []string{value}
	return key, nil
}

func NewInmemCacheDB() *InmemRedis {
	return &InmemRedis{
		data:   map[string][]string{},
		locker: new(sync.RWMutex),
	}
}
