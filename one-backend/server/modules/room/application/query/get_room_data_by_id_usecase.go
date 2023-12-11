package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/domain/repository"
)

type GetRoomByIDUseCase struct {
	repo repository.IRoomRepository
}

var _ application.IUseCase = (*GetRoomByIDUseCase)(nil)

type GetRoomByIDQuery struct {
	RoomID string `uri:"id" binding:"required" validate:"required"`
}

type GetRoomByIDUseCaseResponse struct {
	*entity.Room
	UserCount int `json:"user_count" example:"1"`
}

func NewGetRoomByIDUseCase(dep *define.Dependency) *GetRoomByIDUseCase {
	return &GetRoomByIDUseCase{
		repo: dep.Repo,
	}
}

func (c *GetRoomByIDUseCase) Execute(ctx context.Context, queryz any) (any, error) {
	query := queryz.(*GetRoomByIDQuery)
	room, err := c.repo.GetByID(ctx, query.RoomID)
	if err != nil || room == nil {
		return &GetRoomByIDUseCaseResponse{
			Room: nil,
		}, err
	}

	return &GetRoomByIDUseCaseResponse{
		Room:      room,
		UserCount: len(room.Users),
	}, nil
}
