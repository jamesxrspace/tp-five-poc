package command

import (
	"context"

	"github.com/hashicorp/go-multierror"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/room/application/define"
	domainEvent "xrspace.io/server/modules/room/domain/event"

	"xrspace.io/server/modules/room/domain/entity"
	domainError "xrspace.io/server/modules/room/domain/error"
	"xrspace.io/server/modules/room/domain/repository"
)

type SyncLeaveRoomStatusUseCase struct {
	repo       repository.IRoomRepository
	unitOfWork unit_of_work.IUnitOfWork
	eventbus   eventbus.IEventBus
}

var _ application.IUseCase = (*SyncLeaveRoomStatusUseCase)(nil)

type SyncLeaveRoomStatusCommand struct {
	SpaceID string `json:"space_id" validate:"required"`
	RoomID  string `json:"room_id" validate:"required"`
	UserID  string `json:"user_id" validate:"required"`
}

type SyncLeaveRoomResponse struct {
	Msg     string `json:"msg"`
	ErrCode int    `json:"err_code"`
}

func NewSyncLeaveRoomStatusUseCase(dep *define.Dependency) *SyncLeaveRoomStatusUseCase {
	return &SyncLeaveRoomStatusUseCase{
		repo:       dep.Repo,
		unitOfWork: dep.UnitOfWork,
		eventbus:   dep.Bus,
	}
}

func (c *SyncLeaveRoomStatusUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*SyncLeaveRoomStatusCommand)

	var errs *multierror.Error
	var events []*event.Event

	defer func() {
		err := c.eventbus.PublishAll(events)
		errs = multierror.Append(errs, err)
	}()

	result, err := c.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		room, err := c.repo.Get(ctx, cmd.RoomID)
		if err != nil {
			return room, err
		}

		if room == nil {
			return nil, domainError.ErrRoomNotFound
		}

		user := entity.NewUser(cmd.UserID)

		err = room.LeaveRoom(user)
		if err != nil {
			events = append(events, room.Events...)
			return room, err
		}

		err = c.repo.Save(ctx, room)
		if err != nil {
			events = append(events, domainEvent.NewRoomUserLeaveFailedEvent(cmd.RoomID, cmd.UserID, err.Error()))
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
