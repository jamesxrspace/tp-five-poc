package query

import (
	"context"
	"time"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/space/domain/error_code"
	"xrspace.io/server/modules/space/domain/value_object"
)

type ListSpaceGroupUseCase struct {
	repo IQuerySpaceGroupRepository
}

var _ application.IUseCase = (*ListSpaceGroupUseCase)(nil)

type ListSpaceGroupQuery struct {
	pagination.PaginationQuery
}

type ListSpaceGroupFilter struct {
	Archive bool
}

type SpaceResponse struct {
	SpaceId string `json:"space_id" bson:"space_id"`
	Name    string `json:"name" bson:"name"`
}

type SpaceGroupResponse struct {
	Spaces       []*SpaceResponse              `json:"spaces" bson:"spaces"`
	SpaceGroupId string                        `json:"space_group_id" bson:"space_group_id" identifier:"true"`
	Name         string                        `json:"name" bson:"name"`
	Description  string                        `json:"description" bson:"description"`
	Thumbnail    string                        `json:"thumbnail" bson:"thumbnail"`
	Addressable  string                        `json:"addressable" bson:"addressable"`
	Status       value_object.SpaceGroupStatus `json:"status" bson:"status"`
	StartAt      time.Time                     `json:"start_at" bson:"start_at"`
	EndAt        time.Time                     `json:"end_at" bson:"end_at"`
}

type ListSpaceGroupResponse struct {
	pagination.PaginationResponse[SpaceGroupResponse]
}

func NewListSpaceGroupUseCase(repo IQuerySpaceGroupRepository) *ListSpaceGroupUseCase {
	return &ListSpaceGroupUseCase{
		repo: repo,
	}
}

func (s *ListSpaceGroupUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*ListSpaceGroupQuery)

	filter := ListSpaceGroupFilter{Archive: false}

	listSpaceGroupResponse, err := s.repo.List(ctx, cmd.PaginationQuery, filter)

	if err != nil {
		return nil, core_error.NewCoreError(
			error_code.ListSpaceGroupError,
			err,
		)
	}

	return listSpaceGroupResponse, nil
}
