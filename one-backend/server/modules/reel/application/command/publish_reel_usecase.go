package command

import (
	"context"
	"fmt"

	"github.com/rs/zerolog"
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/reel/application/define"
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

	_, err := u.dep.UnitOfWork.WithTransaction(ctx, func(ctx context.Context) (interface{}, error) {
		reel, err := u.dep.ReelRepo.Get(ctx, cmd.ReelID)
		if err != nil {
			return nil, err
		}

		if reel == nil {
			return nil, core_error.NewEntityNotFoundError("reel", cmd.ReelID)
		}

		if !reel.IsOwner(cmd.XrID) {
			return nil, core_error.NewCodeError(core_error.PermissionErrCode, fmt.Errorf("not allowed to publish reel, xrid: %s , reel_id: %s", cmd.XrID, cmd.ReelID))
		}

		if reel.IsPublish() {
			log := zerolog.Ctx(ctx)
			log.Warn().Msgf("reel already published, reel_id: %s", cmd.ReelID)
			return &PublishReelResponse{}, nil
		}

		if reel.IsDelete() {
			return nil, core_error.NewCodeError(core_error.EntityNotFoundErrCode, fmt.Errorf("cannot publish the reel, it already deleted, reel_id: %s", cmd.ReelID))
		}

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
