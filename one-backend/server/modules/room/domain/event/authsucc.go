package event

import (
	"xrspace.io/server/core/arch/domain/event"
)

func NewRoomAuthSuccessEvent() *event.Event {
	return event.New(
		RoomAuthSucceedEvent,
		moduleName,
		"v1",
		nil,
	)
}
