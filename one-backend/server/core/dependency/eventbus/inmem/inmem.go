package inmem

import (
	"context"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/arch/domain/eventbus"
)

type InMemEventBus struct {
	subscribers  map[event.Topic][]eventbus.Handler
	subAllMighty []eventbus.Handler
}

var _ eventbus.IEventBus = (*InMemEventBus)(nil)

func NewInMemEventBus() *InMemEventBus {
	return &InMemEventBus{
		subscribers: make(map[event.Topic][]eventbus.Handler),
	}
}

func (i *InMemEventBus) Publish(event *event.Event) error {
	subs := i.subscribers[event.Topic]
	subs = append(subs, i.subAllMighty...)
	for _, handler := range subs {
		err := handler(context.Background(), event)
		if err != nil {
			return err
		}
	}

	return nil
}

func (i *InMemEventBus) PublishAll(events []*event.Event) error {
	for _, e := range events {
		err := i.Publish(e)
		if err != nil {
			return err
		}
	}
	return nil
}

func (i *InMemEventBus) SubscribeAll(handler eventbus.Handler) error {
	i.subAllMighty = append(i.subAllMighty, handler)
	return nil
}

func (i *InMemEventBus) Subscribe(topic event.Topic, handler eventbus.Handler) error {
	i.subscribers[topic] = append(i.subscribers[topic], handler)
	return nil
}
