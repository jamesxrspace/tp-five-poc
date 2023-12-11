package command

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/enum"
)

var _ application.IUseCase = (*LikeUseCase)(nil)

type LikeCommand struct {
	XrID       string          `json:"xr_id" token:"xrid" validate:"required"`
	TargetType enum.TargetType `uri:"target_type" validate:"required"`
	TargetID   string          `uri:"target_id" validate:"required"`
}

type LikeResponse struct {
}

type LikeUseCase struct {
	dep define.Dependency
}

func NewLikeUseCase(dep define.Dependency) *LikeUseCase {
	return &LikeUseCase{dep: dep}
}

func (uc *LikeUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*LikeCommand)

	_, err := uc.dep.UnitOfWork.WithTransaction(ctx, func(ctx context.Context) (interface{}, error) {
		if !uc.checkTargetExist(ctx, cmd.TargetType, cmd.TargetID) {
			return nil, core_error.NewEntityNotFoundError(string(cmd.TargetType), cmd.TargetID)
		}

		like, err := uc.dep.QueryRepo.GetLike(ctx, define.GetLikeFilter{
			XrID:       cmd.XrID,
			TargetType: cmd.TargetType,
			TargetID:   cmd.TargetID,
		})
		if err != nil {
			return nil, err
		}

		if like == nil {
			like = entity.NewLike(&entity.LikeParams{
				XrID:       cmd.XrID,
				TargetType: cmd.TargetType,
				TargetID:   cmd.TargetID,
			})
		}

		like.Toggle()
		err = uc.dep.LikeRepo.Save(ctx, like)
		if err != nil {
			return nil, err
		}

		return nil, nil
	})

	if err != nil {
		return nil, err
	}

	return &LikeResponse{}, nil
}

func (uc *LikeUseCase) checkTargetExist(ctx context.Context, targetType enum.TargetType, target string) bool {
	if targetType == "feed" {
		return uc.dep.QueryRepo.IsFeedExist(ctx, target)
	}
	return false
}
