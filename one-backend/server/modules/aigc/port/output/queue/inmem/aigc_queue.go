package inmem

import (
	"context"
	"sync"
	"time"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/aigc/domain"
)

var _ domain.IQueue = (*AigcQueue)(nil)

type AigcQueue struct {
	queue       map[string][]string
	mustTimeout bool
	mu          sync.Mutex
}

func NewAigcQueue() *AigcQueue {
	return &AigcQueue{
		queue: make(map[string][]string),
	}
}

func (q *AigcQueue) Push(ctx context.Context, key string, values string) error {
	q.mu.Lock()
	defer q.mu.Unlock()
	q.queue[key] = append(q.queue[key], values)
	return nil
}

func (q *AigcQueue) SetMustTimeout(mustTimeout bool) {
	q.mustTimeout = mustTimeout
}

func (q *AigcQueue) BlockLeftPop(ctx context.Context, key string, timeout time.Duration) (string, error) {
	q.mu.Lock()
	defer q.mu.Unlock()
	if q.mustTimeout {
		return "", core_error.NewInternalError("timeout")
	}
	for stay, timer := true, time.After(timeout); stay; {
		select {
		case <-timer:
			stay = false
		default:
		}
		if len(q.queue[key]) > 0 {
			result := q.queue[key][0]
			q.queue[key] = q.queue[key][1:]
			return result, nil
		}
	}

	return "", core_error.NewInternalError("timeout")
}
