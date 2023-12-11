package command

import (
	"context"
	"errors"

	"github.com/go-playground/validator/v10"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/avatar/domain/avatar_errors"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type SetPlayerAvatarCommand struct {
	XrID     value_object.XrID     `json:"-" validate:"required"`
	AppID    value_object.AppID    `json:"-" validate:"required"`
	AvatarID value_object.AvatarID `json:"-" validate:"required"`
}

func (c *SetPlayerAvatarCommand) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

type SetPlayerAvatarResponse struct {
	Message string `json:"message"`
}

type SetPlayerAvatarUseCase struct {
	repo repository.ISetPlayerAvatarRepository
}

func NewSetPlayerAvatarUseCase(
	repository repository.ISetPlayerAvatarRepository,
) *SetPlayerAvatarUseCase {
	return &SetPlayerAvatarUseCase{
		repo: repository,
	}
}

func (u *SetPlayerAvatarUseCase) Execute(ctx context.Context, cmd *SetPlayerAvatarCommand) (*SetPlayerAvatarResponse, error) {
	if err := cmd.Validate(); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.ValidateError,
			err,
		)
	}

	player, err := u.repo.GetAvatarPlayer(ctx, cmd.XrID)
	if err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.GetAvatarPlayerError,
			err,
		)
	}
	if player == nil {
		player = entity.NewAvatarPlayer(
			&entity.AvatarPlayerParams{
				XrID:     cmd.XrID,
				AvatarID: make(map[value_object.AppID]value_object.AvatarID),
			},
		)
	}

	avatar, err := u.repo.GetAvatar(ctx, cmd.AvatarID)
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

	if err := player.SetCurrentAvatar(avatar.XrID, cmd.AvatarID, cmd.AppID); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.SetCurrentAvatarError,
			err,
		)
	}

	if err := u.repo.SaveAvatarPlayer(ctx, player); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.SaveAvatarPlayerError,
			err,
		)
	}

	return &SetPlayerAvatarResponse{
		Message: "ok",
	}, nil
}
