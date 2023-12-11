package event

import (
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
)

func NewRoomUserJoinFailedEvent(roomID, userID, reason string) *coreDomainEvent.Event {
	return coreDomainEvent.New(
		RoomUserJoinFailedEvent,
		moduleName,
		"v1",
		map[string]string{
			"room_id": roomID,
			"user_id": userID,
			"reason":  reason,
		},
	)
}
