package event

import (
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
)

func NewRoomUserLeaveFailedEvent(roomID, userID, reason string) *coreDomainEvent.Event {
	return coreDomainEvent.New(
		RoomUserLeaveFailedEvent,
		moduleName,
		"v1",
		map[string]string{
			"room_id": roomID,
			"user_id": userID,
			"reason":  reason,
		},
	)
}
