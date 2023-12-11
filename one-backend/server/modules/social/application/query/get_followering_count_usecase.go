package query

import (
	"context"

	"xrspace.io/server/modules/social/application/define"
)

type GetFollowingCountQuery struct {
	XrID string `json:"xrid" validate:"required" token:"xrid"`
}

type GetFollowingCountUseCase struct {
	dep define.Dependency
}

func NewGetFollowingCountUseCase(dep define.Dependency) *GetFollowingCountUseCase {
	return &GetFollowingCountUseCase{
		dep: dep,
	}
}

func (uc *GetFollowingCountUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	panic("implement me")
}
