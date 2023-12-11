package application

import (
	"xrspace.io/server/core/arch/domain/event"
)

type ICommand interface {
	Validate() error
}

type IEventCommand interface {
	SetEvent(event *event.Event) error
}
