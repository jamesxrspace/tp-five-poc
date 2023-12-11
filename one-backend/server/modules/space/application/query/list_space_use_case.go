package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/error_code"
)

type ListSpaceUseCase struct {
	repo IQuerySpaceRepository
}

var _ application.IUseCase = (*ListSpaceUseCase)(nil)

type ListSpaceQuery struct {
	pagination.PaginationQuery
	ListSpaceFilter
}

type ListSpaceFilter struct {
	Archive      bool
	SpaceGroupId string `form:"space_group_id"`
}

type ListSpaceResponse struct {
	pagination.PaginationResponse[entity.Space]
}

func NewListSpaceUseCase(repo IQuerySpaceRepository) *ListSpaceUseCase {
	return &ListSpaceUseCase{
		repo: repo,
	}
}

func (s *ListSpaceUseCase) Execute(ctx context.Context, queryz any) (any, error) {
	query := queryz.(*ListSpaceQuery)

	filter := ListSpaceFilter{Archive: false, SpaceGroupId: query.SpaceGroupId}

	listSpaceResponse, err := s.repo.List(ctx, query.PaginationQuery, filter)

	if err != nil {
		return nil, core_error.NewCoreError(
			error_code.ListSpaceError,
			err,
		)
	}

	return listSpaceResponse, nil
}
