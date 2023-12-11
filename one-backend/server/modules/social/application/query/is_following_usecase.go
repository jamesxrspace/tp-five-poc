package query

import (
	"context"

	"xrspace.io/server/modules/social/application/define"
)

type IsFollowingQuery struct {
	XrID        string `json:"xrid" validate:"required" token:"xrid"`
	FollowingID string `json:"following_id" validate:"required,nefield=XrID"`
}

type IsFollowingUseCase struct {
	dep define.Dependency
}

func NewIsFollowingUseCase(dep define.Dependency) *IsFollowingUseCase {
	return &IsFollowingUseCase{
		dep: dep,
	}
}

func (uc *IsFollowingUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	panic("implement me")
}
