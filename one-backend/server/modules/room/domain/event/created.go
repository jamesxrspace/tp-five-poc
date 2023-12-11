package event

import (
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
)

func NewRoomCreatedEvent(roomID, userID string) *coreDomainEvent.Event {
	return newRoomEvent(RoomCreatedEvent, roomID, userID)
}
