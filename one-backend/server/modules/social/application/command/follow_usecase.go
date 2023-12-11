package command

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/social/application/define"
	"xrspace.io/server/modules/social/domain"
)

var _ application.IUseCase = (*FollowUseCase)(nil)

type FollowCommand struct {
	XrID        string `json:"xrid" validate:"required" token:"xrid"`
	FollowingID string `json:"following_id" validate:"required,nefield=XrID"`
}

type FollowUseCase struct {
	dep define.Dependency
}

type FolloweResponse struct {
	Follow *domain.Follow
}

func NewFollowUseCase(dep define.Dependency) *FollowUseCase {
	return &FollowUseCase{dep: dep}
}

func (uc *FollowUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	panic("implement me")
}
