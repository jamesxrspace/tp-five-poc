package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/domain/repository"
)

type GetAllRoomDataUseCase struct {
	repo repository.IRoomRepository
}

var _ application.IUseCase = (*GetAllRoomDataUseCase)(nil)

type GetAllRoomDataQuery struct {
}

type GetAllRoomDataResponse struct {
	Msg     string         `json:"msg"`
	ErrCode int            `json:"err_code"`
	Rooms   []*entity.Room `json:"rooms"`
}

func NewGetAllRoomDataUseCase(dep *define.Dependency) *GetAllRoomDataUseCase {
	return &GetAllRoomDataUseCase{
		repo: dep.Repo,
	}
}

func (c *GetAllRoomDataUseCase) Execute(ctx context.Context, _ any) (any, error) {
	rooms, err := c.repo.AllRoomData(ctx)
	if err != nil {
		return &GetAllRoomDataResponse{
			Msg:   "get all room data error",
			Rooms: []*entity.Room{},
		}, err
	}

	// rooms length is 0 message room not found
	if len(rooms) == 0 {
		return &GetAllRoomDataResponse{
			Msg:   "room is empty",
			Rooms: []*entity.Room{},
		}, nil
	}

	return &GetAllRoomDataResponse{
		Msg:   "success",
		Rooms: rooms,
	}, nil
}
