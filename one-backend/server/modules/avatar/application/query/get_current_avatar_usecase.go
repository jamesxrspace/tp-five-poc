package query

import (
	"context"
	"errors"
	"fmt"

	"github.com/go-playground/validator/v10"
	"github.com/rs/zerolog/log"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/avatar/domain/avatar_errors"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type GetCurrentAvatarQuery struct {
	XrID  value_object.XrID  `form:"xrid" validate:"required"`
	AppID value_object.AppID `form:"app_id" validate:"required"`
}

type GetCurrentAvatarResponse struct {
	Avatar *entity.Avatar `json:"avatar"`
}

func (c *GetCurrentAvatarQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

type GetCurrentAvatarUseCase struct {
	repository repository.IGetCurrentAvatarRepository
}

func NewGetCurrentAvatarUseCase(
	repository repository.IGetCurrentAvatarRepository,
) *GetCurrentAvatarUseCase {
	return &GetCurrentAvatarUseCase{
		repository: repository,
	}
}

func (u *GetCurrentAvatarUseCase) Execute(ctx context.Context, q *GetCurrentAvatarQuery) (*GetCurrentAvatarResponse, error) {
	if err := q.Validate(); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.ValidateError,
			err,
		)
	}

	player, err := u.repository.GetAvatarPlayer(ctx, q.XrID)
	if err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.GetAvatarPlayerError,
			err,
		)
	}
	if player == nil {
		return nil, core_error.NewCoreError(
			avatar_errors.GetAvatarPlayerError,
			fmt.Errorf("player does not exist for xrid: %s", q.XrID),
		)
	}
	if _, ok := player.AvatarID[q.AppID]; !ok {
		log.Info().Msgf("player does not have avatar for app_id: %s", q.AppID)
		return nil, core_error.NewCoreError(
			avatar_errors.AvatarNotExistError,
			errors.New("player does not have avatar for this app"),
		)
	}
	if player.AvatarID[q.AppID] == "" {
		log.Info().Msgf("player should set avatar first, app_id: %s", q.AppID)
		return nil, core_error.NewCoreError(
			avatar_errors.AvatarNotExistError,
			errors.New("player should set avatar first"),
		)
	}

	avatar, err := u.repository.GetAvatar(ctx, player.AvatarID[q.AppID])
	if err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.GetAvatarError,
			err,
		)
	}
	if avatar == nil {
		return nil, core_error.NewCoreError(
			avatar_errors.GetAvatarError,
			errors.New("avatar not found"),
		)
	}

	return &GetCurrentAvatarResponse{
		Avatar: avatar,
	}, nil
}
