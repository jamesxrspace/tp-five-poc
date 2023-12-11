package application

import (
	"context"
	"fmt"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/example/domain/repository"
)

type LeaveCommand struct {
	XrId   string `json:"xrId" form:"xrid" validate:"required"`
	RoomId string `json:"roomId" form:"roomid" validate:"required"`
}

func (A *LeaveCommand) Validate() error {
	if A.XrId == "error" {
		return fmt.Errorf("it's a fake error, because it's not 123 bug '%v'", A.XrId)
	}
	return nil
}

type leaveResponse struct {
	RoomId string
	XrId   string
	XrRoom string
	Count  int64
	Users  []string
}

type leaveUseCase struct {
	repo repository.IExampleRoomRepository
}

var _ application.IUseCase = (*leaveUseCase)(nil)

func newLeaveUseCase(repo repository.IExampleRoomRepository) *leaveUseCase {
	return &leaveUseCase{
		repo: repo,
	}
}

func (A *leaveUseCase) Execute(ctx context.Context, c any) (any, error) {
	cmd := c.(*LeaveCommand)
	room, err := A.repo.Get(ctx, cmd.RoomId)
	if err != nil {
		return nil, err
	}

	if room == nil {
		return nil, core_error.NewEntityNotFoundError("room", cmd.RoomId)
	}

	room.Leave(cmd.XrId)

	_ = A.repo.Save(ctx, room)

	rsp := &leaveResponse{
		RoomId: cmd.RoomId,
		XrId:   cmd.XrId,
		XrRoom: cmd.RoomId + cmd.XrId,
		Count:  room.Count,
		Users:  room.XrIds,
	}
	return rsp, nil
}
