package event

import (
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
)

func NewRoomUserLeavedEvent(roomID, userID string) *coreDomainEvent.Event {
	return newRoomEvent(RoomUserLeavedEvent, roomID, userID)
}
