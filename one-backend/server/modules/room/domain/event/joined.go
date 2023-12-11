package event

import (
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
)

func NewRoomUserJoinedEvent(roomID, userID string) *coreDomainEvent.Event {
	return newRoomEvent(RoomUserJoinedEvent, roomID, userID)
}
