package entity

import (
	"xrspace.io/server/core/arch/domain/event"
	domainError "xrspace.io/server/modules/room/domain/error"
	domainEvent "xrspace.io/server/modules/room/domain/event"
)

type Room struct {
	ID      string         `json:"room_id" bson:"room_id"`
	SpaceID string         `json:"space_id" bson:"space_id"`
	Users   map[XrID]User  `json:"users" bson:"users"`
	Events  []*event.Event `json:"-" bson:"-"`
}

func NewRoom(spaceID string, roomID string) *Room {

	return &Room{
		ID:      roomID,
		SpaceID: spaceID,
		Users:   make(map[XrID]User),
		Events:  []*event.Event{domainEvent.NewRoomCreatedEvent(roomID, spaceID)},
	}
}

func (r *Room) JoinRoom(user *User) error {

	if _, ok := r.Users[user.XrID]; ok {
		r.Events = append(r.Events, domainEvent.NewRoomUserJoinFailedEvent(r.ID, string(user.XrID), "user already in room"))
		return domainError.ErrRoomUserAlreadyInRoom
	}

	r.Users[user.XrID] = *user
	r.Events = append(r.Events, domainEvent.NewRoomUserJoinedEvent(r.ID, string(user.XrID)))

	return nil
}

func (r *Room) LeaveRoom(user *User) error {
	if _, ok := r.Users[user.XrID]; !ok {
		r.Events = append(r.Events, domainEvent.NewRoomUserLeaveFailedEvent(r.ID, string(user.XrID), "user not found"))
		return domainError.ErrRoomUserNotFound
	}

	r.Events = append(r.Events, domainEvent.NewRoomUserLeavedEvent(r.ID, string(user.XrID)))
	delete(r.Users, user.XrID)

	return nil
}
