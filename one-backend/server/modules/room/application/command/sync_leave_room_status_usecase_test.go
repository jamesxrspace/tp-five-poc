package command

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
	domainEvent "xrspace.io/server/modules/room/domain/event"
)

func initRoomWithDefaultUser(roomId, userId string) (*define.Dependency, *[]*event.Event, *entity.Room) {
	dep, events := newDependency()

	uc := NewSyncJoinRoomStatusUseCase(dep)
	cmd := &SyncJoinRoomStatusCommand{
		SpaceID: "S1",
		RoomID:  roomId,
		UserID:  userId,
	}
	r, _ := uc.Execute(context.Background(), cmd)
	room := r.(*entity.Room)
	return dep, events, room
}

func joinRoom(dep *define.Dependency, roomId, userId string) (*entity.Room, error) {
	uc := NewSyncJoinRoomStatusUseCase(dep)
	cmd := &SyncJoinRoomStatusCommand{
		SpaceID: "S1",
		RoomID:  roomId,
		UserID:  userId,
	}
	r, err := uc.Execute(context.Background(), cmd)
	room := r.(*entity.Room)
	return room, err
}

// func leaveRoom(repo repository.IRoomRepository, roomId, userId string) (*entity.Room, error) {
func leaveRoom(dep *define.Dependency, roomId, userId string) (*entity.Room, error) {
	uc := NewSyncLeaveRoomStatusUseCase(dep)
	cmd := &SyncLeaveRoomStatusCommand{
		SpaceID: "S1",
		RoomID:  roomId,
		UserID:  userId,
	}
	r, err := uc.Execute(context.Background(), cmd)
	room, _ := r.(*entity.Room)
	return room, err
}

func TestLeaveRoomStatusUseCase_WhenEmptyRoom_ShouldReturnEmpty(t *testing.T) {
	dep, events := newDependency()

	room, err := leaveRoom(dep, "R1", "U1")
	if err != nil {
		return
	}

	assert.Nil(t, room)
	assert.NotNil(t, err)

	// check event
	assert.Equal(t, 1, len(*events))
	assert.Equal(t, domainEvent.RoomUserLeaveFailedEvent, (*events)[0].Topic)
}

func TestLeaveRoomStatusUseCase_Given1User_WhenLeave_ShouldHave0InRoom(t *testing.T) {

	dep, events, _ := initRoomWithDefaultUser("R1", "U1")

	room, err := leaveRoom(dep, "R1", "U1")
	// assert
	expected := 0
	assert.Equal(t, expected, len(room.Users))
	assert.Nil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomUserLeavedEvent,
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}

//nolint:dupl
func TestLeaveRoomStatusUseCase_Given2UsersInDiffRoom_When1Leave_ShouldHave0_1InRoom(t *testing.T) {
	dep, events, room1 := initRoomWithDefaultUser("R1", "U1")

	room1OrgUserCount := len(room1.Users)
	room2, _ := joinRoom(dep, "R2", "U2")

	//act
	room1, err := leaveRoom(dep, "R1", "U1")

	// assert
	assert.Equal(t, 1, room1OrgUserCount)
	assert.Equal(t, 0, len(room1.Users))
	assert.Equal(t, 1, len(room2.Users))
	assert.Nil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomUserLeavedEvent,
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}

func TestLeaveRoomStatusUseCase_Given2UsersInDiffRoom_When1LeaveTwice_ShouldHave0_1InRoom(t *testing.T) {
	dep, events, room1 := initRoomWithDefaultUser("R1", "U1")
	room1OrgUserCount := len(room1.Users)
	room2, _ := joinRoom(dep, "R2", "U2")

	//act
	_, _ = leaveRoom(dep, "R1", "U1")
	room1, err := leaveRoom(dep, "R1", "U1")

	// assert
	assert.Equal(t, 1, room1OrgUserCount)
	assert.Equal(t, 0, len(room1.Users))
	assert.Equal(t, 1, len(room2.Users))
	assert.NotNil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomUserLeavedEvent,
		domainEvent.RoomUserLeaveFailedEvent, // leave twice should fail
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}

//nolint:dupl
func TestLeaveRoomStatusUseCase_Given2UsersInDiffRoom_WhenLeaveWrongUser_ShouldHave1_1InRoom(t *testing.T) {
	dep, events, room1 := initRoomWithDefaultUser("R1", "U1")

	room1OrgUserCount := len(room1.Users)
	room2, _ := joinRoom(dep, "R2", "U2")

	//act
	room1, err := leaveRoom(dep, "R1", "Wrong")

	// assert
	assert.Equal(t, 1, room1OrgUserCount)
	assert.Equal(t, 1, len(room1.Users))
	assert.Equal(t, 1, len(room2.Users))
	assert.NotNil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomUserLeaveFailedEvent, // leave wrong user should fail
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}

}
