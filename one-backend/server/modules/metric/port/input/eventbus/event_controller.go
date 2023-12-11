package eventbus

import (
	"context"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/modules/metric/application/command"
	"xrspace.io/server/modules/metric/domain/repository"
)

type Subscriber struct {
	bus eventbus.IEventBus
	uc  *command.HandleEventUseCase
}

func NewController(bus eventbus.IEventBus, repo repository.IMetricRepository) *Subscriber {
	sub := &Subscriber{
		bus: bus,
		uc:  command.NewHandleEventUseCase(repo),
	}

	return sub
}

func (s *Subscriber) Subscribe() error {
	return s.bus.SubscribeAll(s.handle)
}

func (s *Subscriber) handle(ctx context.Context, e *event.Event) error {
	cmd := command.NewHandleEventCommand(e)
	return s.uc.Execute(ctx, cmd)
}
