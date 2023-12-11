package error

import "errors"

var (
	ErrRoomNotFound          = errors.New("room not found")
	ErrRoomUserNotFound      = errors.New("user is not in the room")
	ErrRoomUserAlreadyInRoom = errors.New("user is already in the room")
)
