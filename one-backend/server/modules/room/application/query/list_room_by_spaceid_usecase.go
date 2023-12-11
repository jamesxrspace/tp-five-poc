package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/room/application/define"
)

type ListRoomBySpaceIDQuery struct {
	*pagination.PaginationQuery
	SpaceID string `validate:"required" form:"space_id"`
}

type ListRoomBySpaceIDUsecase struct {
	repository define.IQueryRepository
}

var _ application.IUseCase = (*ListRoomBySpaceIDUsecase)(nil)

func NewListRoomBySpaceIDUsecase(dep *define.Dependency) *ListRoomBySpaceIDUsecase {
	return &ListRoomBySpaceIDUsecase{
		repository: dep.QueryRepo,
	}
}

func (u *ListRoomBySpaceIDUsecase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*ListRoomBySpaceIDQuery)
	result, total, err := u.repository.ListRoomBySpaceID(ctx, cmd.SpaceID, cmd.PaginationQuery)

	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	return pagination.NewPaginationResponse(result, total), nil
}
