package query

import (
	"context"

	"xrspace.io/server/modules/social/application/define"
)

type GetFollowersCountQuery struct {
	XrID string `json:"xrid" validate:"required" token:"xrid"`
}

type GetFollowersCountUseCase struct {
	dep define.Dependency
}

func NewGetFollowersCountUseCase(dep define.Dependency) *GetFollowersCountUseCase {
	return &GetFollowersCountUseCase{
		dep: dep,
	}
}

func (uc *GetFollowersCountUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	panic("implement me")
}
