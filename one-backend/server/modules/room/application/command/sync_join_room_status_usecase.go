package command

import (
	"context"

	"github.com/hashicorp/go-multierror"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	coreDomainEvent "xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/domain/event"
	"xrspace.io/server/modules/room/domain/repository"
)

type SyncJoinRoomStatusUseCase struct {
	repo       repository.IRoomRepository
	bus        eventbus.IEventBus
	unitOfWork unit_of_work.IUnitOfWork
}

var _ application.IUseCase = (*SyncJoinRoomStatusUseCase)(nil)

type SyncJoinRoomStatusCommand struct {
	SpaceID string `json:"space_id" validate:"required"`
	RoomID  string `json:"room_id" validate:"required"`
	UserID  string `json:"user_id" validate:"required"`
}

type SyncJoinRoomResponse struct {
	Msg     string `json:"msg"`
	ErrCode int    `json:"err_code"`
}

func NewSyncJoinRoomStatusUseCase(dep *define.Dependency) *SyncJoinRoomStatusUseCase {
	return &SyncJoinRoomStatusUseCase{
		repo:       dep.Repo,
		bus:        dep.Bus,
		unitOfWork: dep.UnitOfWork,
	}
}

func (c *SyncJoinRoomStatusUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*SyncJoinRoomStatusCommand)

	var errs *multierror.Error
	var events []*coreDomainEvent.Event
	defer func() {
		err := c.bus.PublishAll(events)
		errs = multierror.Append(errs, err)
	}()

	result, err := c.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		room, err := c.repo.Get(cx, cmd.RoomID)
		if err != nil {
			return nil, err
		}

		if room == nil {
			room = entity.NewRoom(cmd.SpaceID, cmd.RoomID)
		}

		user := entity.NewUser(cmd.UserID)

		err = room.JoinRoom(user)
		if err != nil {
			events = append(events, room.Events...)
			return room, err
		}

		err = c.repo.Save(cx, room)
		if err != nil {
			events = append(events, event.NewRoomUserJoinFailedEvent(room.ID, cmd.UserID, err.Error()))
			return room, err
		}

		events = append(events, room.Events...)
		return room, nil
	})

	errs = multierror.Append(errs, err)

	if room, ok := result.(*entity.Room); !ok {
		return nil, core_error.NewValidateErrorOrNil(errs.ErrorOrNil())
	} else {
		return room, core_error.NewValidateErrorOrNil(errs.ErrorOrNil())
	}
}
