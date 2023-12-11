package query

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/social/application/define"
)

type ListFollowersQuery struct {
	XrID string `json:"xrid" validate:"required" token:"xrid"`
	pagination.PaginationQuery
}

type ListFollowersUseCase struct {
	dep define.Dependency
}

func NewListFollowersUseCase(dep define.Dependency) *ListFollowersUseCase {
	return &ListFollowersUseCase{
		dep: dep,
	}
}

func (uc *ListFollowersUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	panic("implement me")
}
