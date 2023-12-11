package command

import (
	"context"

	"github.com/go-playground/validator/v10"

	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/modules/metric/domain/repository"
)

type HandleEventCommand struct {
	event *coreDomainEvent.Event `validate:"required"`
}

func NewHandleEventCommand(event *coreDomainEvent.Event) *HandleEventCommand {
	return &HandleEventCommand{
		event: event,
	}
}

func (c *HandleEventCommand) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

type HandleEventResponse struct {
	Msg string `json:"msg"`
}

type HandleEventUseCase struct {
	repo repository.IMetricRepository
}

func NewHandleEventUseCase(repo repository.IMetricRepository) *HandleEventUseCase {
	return &HandleEventUseCase{
		repo: repo,
	}
}

func (c *HandleEventUseCase) Execute(ctx context.Context, cmd *HandleEventCommand) error {
	if err := cmd.Validate(); err != nil {
		return err
	}

	c.repo.Inc(cmd.event.Topic, cmd.event.ToMap())
	return nil
}
