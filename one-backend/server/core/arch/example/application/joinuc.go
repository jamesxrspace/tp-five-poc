package application

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/example/domain/entity"
	"xrspace.io/server/core/arch/example/domain/repository"
)

type JoinCommand struct {
	XrId   string `json:"xrId" form:"xrid" validate:"required"`
	RoomId string `form:"roomid" validate:"required"` //use for to get the parameter from the url, in this case http://localhost:8080/join?roomid=123
}

type joinResponse struct {
	RoomId string
	XrId   string
	XrRoom string
	Count  int64
}

type joinUseCase struct {
	repo repository.IExampleRoomRepository
}

var _ application.IUseCase = (*joinUseCase)(nil)

func newJoinUseCase(repo repository.IExampleRoomRepository) *joinUseCase {
	return &joinUseCase{
		repo: repo,
	}
}

func (A *joinUseCase) Execute(ctx context.Context, c any) (any, error) {
	cmd := c.(*JoinCommand)
	room, err := A.repo.Get(ctx, cmd.RoomId)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	if room == nil {
		room = entity.NewExampleRoom(cmd.RoomId)

		// Data driven
		//room = ExampleRoom{
		//	Id:    cmd.RoomId,
		//	XrIds: []string{cmd.XrId},
		//	Count: 1,
		//}
	}

	// Domain driven
	room.Join(cmd.XrId)

	err = A.repo.Save(ctx, room)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	// response
	rsp := &joinResponse{
		RoomId: cmd.RoomId,
		XrId:   cmd.XrId,
		XrRoom: cmd.RoomId + cmd.XrId,
		Count:  room.Count,
	}

	return rsp, nil
}
