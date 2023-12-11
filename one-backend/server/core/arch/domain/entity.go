package domain

import "xrspace.io/server/core/arch/domain/event"

type Entity struct {
	Events []*event.Event `json:"-"`
}

func (e *Entity) AddEvent(event *event.Event) {
	e.Events = append(e.Events, event)
}
