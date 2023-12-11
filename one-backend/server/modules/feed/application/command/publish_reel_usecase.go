package command

import (
	"context"
	"fmt"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/feed/application/define"
)

var _ application.IUseCase = (*PublishReelUseCase)(nil)

type PublishReelCommand struct {
	XrID   string `json:"xrid" validate:"required" token:"xrid"`
	ReelID string `uri:"reel_id" json:"reel_id" validate:"required"`
}

type PublishReelResponse struct {
}

type PublishReelUseCase struct {
	dep define.Dependency
}

func NewPublishReelUseCase(dep define.Dependency) *PublishReelUseCase {
	return &PublishReelUseCase{
		dep: dep,
	}
}

func (u *PublishReelUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*PublishReelCommand)

	reel, err := u.dep.ReelRepo.Get(ctx, cmd.ReelID)
	if err != nil {
		return nil, err
	}

	if reel == nil {
		return nil, core_error.StackError(fmt.Sprintf("reel not found, reel_id: %s", cmd.ReelID))
	}

	if !reel.IsOwner(cmd.XrID) {
		return nil, core_error.StackError(fmt.Sprintf("not allowed to publish reel, reel_id: %s", cmd.ReelID))
	}

	if !reel.IsDraft() {
		return nil, core_error.StackError(fmt.Sprintf("reel already published, reel_id: %s", cmd.ReelID))
	}

	_, err = u.dep.UnitOfWork.WithTransaction(ctx, func(ctx context.Context) (interface{}, error) {
		reel.Publish()

		if err := u.dep.ReelRepo.Save(ctx, reel); err != nil {
			return nil, err
		}
		err = u.dep.EventBus.PublishAll(reel.Events)

		if err != nil {
			return nil, err
		}

		return nil, nil
	})

	if err != nil {
		return nil, err
	}

	return &PublishReelResponse{}, nil
}
