package query

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/social/application/define"
)

type ListFollowingQuery struct {
	XrID string `json:"xrid" validate:"required" token:"xrid"`
	pagination.PaginationQuery
}

type ListFollowingUseCase struct {
	dep define.Dependency
}

func NewListFollowingUseCase(dep define.Dependency) *ListFollowingUseCase {
	return &ListFollowingUseCase{
		dep: dep,
	}
}

func (uc *ListFollowingUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	panic("implement me")
}
