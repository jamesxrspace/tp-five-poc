package event

import (
	"xrspace.io/server/core/arch/domain/event"
)

func NewRoomAuthFailedEvent(reason string, err error) *event.Event {
	return event.New(
		RoomAuthFailedEvent,
		moduleName,
		"v1",
		map[string]string{
			"reason": reason,
			"error":  err.Error(),
		},
	)
}
