package event

import (
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
)

const moduleName = "room"

const (
	RoomAuthSucceedEvent     coreDomainEvent.Topic = "room_auth_success"
	RoomAuthFailedEvent      coreDomainEvent.Topic = "room_auth_failed"
	RoomCreatedEvent         coreDomainEvent.Topic = "room_created"
	RoomUserJoinedEvent      coreDomainEvent.Topic = "room_user_joined"
	RoomUserJoinFailedEvent  coreDomainEvent.Topic = "room_user_join_failed"
	RoomUserLeavedEvent      coreDomainEvent.Topic = "room_user_leaved"
	RoomUserLeaveFailedEvent coreDomainEvent.Topic = "room_user_leave_failed"
)

func newRoomEvent(topic coreDomainEvent.Topic, roomID, userID string) *coreDomainEvent.Event {
	return coreDomainEvent.New(
		topic,
		moduleName,
		"v1",
		map[string]string{
			"room_id": roomID,
			"user_id": userID,
		},
	)
}
