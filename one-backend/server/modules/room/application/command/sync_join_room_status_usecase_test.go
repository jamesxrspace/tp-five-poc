package command

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"

	"xrspace.io/server/core/arch/domain/event"
	docdbLocal "xrspace.io/server/core/dependency/database/docdb/local"
	inmem2 "xrspace.io/server/core/dependency/eventbus/inmem"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
	domainEvent "xrspace.io/server/modules/room/domain/event"
	"xrspace.io/server/modules/room/port/output/repo/inmem"
)

func newDependency() (*define.Dependency, *[]*event.Event) {
	bus := inmem2.NewInMemEventBus()
	var events []*event.Event
	_ = bus.SubscribeAll(func(_ context.Context, e *event.Event) error {
		events = append(events, e)
		return nil
	})

	dep := &define.Dependency{
		Repo:           inmem.NewInMemRoomRepo(),
		Bus:            bus,
		UnitOfWork:     docdbLocal.NewUnitOfWork(),
		TokenValidator: nil,
		QueryRepo:      nil,
	}

	return dep, &events
}

func TestSyncRoomStatusUseCase_SyncRoomStatus(t *testing.T) {
	dep, events := newDependency()
	uc := NewSyncJoinRoomStatusUseCase(dep)
	cmd := &SyncJoinRoomStatusCommand{
		SpaceID: "S1",
		RoomID:  "R1",
		UserID:  "U1",
	}

	r, err := uc.Execute(context.Background(), cmd)
	room := r.(*entity.Room)

	expected := 1
	assert.Equal(t, expected, len(room.Users))
	assert.Nil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
	}

	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}

func TestSyncRoomStatusUseCase_When2Users_ShouldHave2InRoom(t *testing.T) {
	dep, events := newDependency()
	uc := NewSyncJoinRoomStatusUseCase(dep)
	cmd := &SyncJoinRoomStatusCommand{
		SpaceID: "S1",
		RoomID:  "R1",
		UserID:  "U1",
	}

	_, _ = uc.Execute(context.Background(), cmd)

	// act
	cmd.UserID = "2"
	r, err := uc.Execute(context.Background(), cmd)
	room := r.(*entity.Room)

	// assert
	expected := 2
	assert.Equal(t, expected, len(room.Users))
	assert.Nil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomUserJoinedEvent,
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}

func TestSyncRoomStatusUseCase_When2UsersInDiffRoom_ShouldHaveEachInRoom(t *testing.T) {
	dep, events := newDependency()

	uc := NewSyncJoinRoomStatusUseCase(dep)
	cmd := &SyncJoinRoomStatusCommand{
		RoomID:  "1",
		UserID:  "1",
		SpaceID: "S1",
	}

	r1, _ := uc.Execute(context.Background(), cmd)
	room1 := r1.(*entity.Room)

	// act
	cmd.RoomID = "2"
	cmd.UserID = "2"
	r2, err := uc.Execute(context.Background(), cmd)
	room2 := r2.(*entity.Room)

	// assert
	expected := 1
	assert.Equal(t, expected, len(room1.Users))
	assert.Equal(t, expected, len(room2.Users))
	assert.Nil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}

func TestSyncRoomStatusUseCase_WhenUsersJoinTwice_ShouldError(t *testing.T) {
	dep, events := newDependency()
	uc := NewSyncJoinRoomStatusUseCase(dep)
	cmd := &SyncJoinRoomStatusCommand{
		SpaceID: "S1",
		RoomID:  "1",
		UserID:  "1",
	}

	_, _ = uc.Execute(context.Background(), cmd)

	// act
	r, err := uc.Execute(context.Background(), cmd)
	room := r.(*entity.Room)

	// assert
	expected := 1
	assert.Equal(t, expected, len(room.Users))
	assert.NotNil(t, err)

	// check event
	expectEvents := []event.Topic{
		domainEvent.RoomCreatedEvent,
		domainEvent.RoomUserJoinedEvent,
		domainEvent.RoomUserJoinFailedEvent,
	}
	assert.Equal(t, len(expectEvents), len(*events))
	for i, expectEvent := range expectEvents {
		assert.Equal(t, expectEvent, (*events)[i].Topic)
	}
}
