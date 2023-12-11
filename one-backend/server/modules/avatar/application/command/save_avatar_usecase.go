package command

import (
	"context"
	"fmt"
	"io"

	"github.com/go-playground/validator/v10"
	"golang.org/x/exp/slices"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/avatar/domain/avatar_errors"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/enum"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/service"
	"xrspace.io/server/modules/avatar/domain/storage"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type SaveAvatarCommand struct {
	AvatarFormat    map[string]any            `form:"avatar_format"`
	AvatarAsset     *value_object.AvatarAsset `form:"-" validate:"required_without=AvatarID"`
	AvatarHead      *value_object.AvatarAsset `form:"-" validate:"required_without=AvatarID"`
	AvatarUpperBody *value_object.AvatarAsset `form:"-" validate:"required_without=AvatarID"`
	AvatarFullBody  *value_object.AvatarAsset `form:"-" validate:"required_without=AvatarID"`
	AvatarID        value_object.AvatarID     `form:"avatar_id"`
	XrID            value_object.XrID         `form:"-" validate:"required"`
	AppID           value_object.AppID        `form:"-" validate:"required"`
	Type            value_object.AvatarType   `form:"type" validate:"required_without=AvatarID"`
}

func (c *SaveAvatarCommand) Validate() error {
	validate := validator.New(validator.WithRequiredStructEnabled())
	if err := validate.Struct(c); err != nil {
		return err
	}

	// check avatar type
	if c.Type != "" && c.Type != enum.AvatarTypeXrV2 {
		return fmt.Errorf("avatar type is not allowed")
	}

	avatarThumbnails := c.getAvatarThumbnails()
	for part, avatarThumbnail := range avatarThumbnails {
		if avatarThumbnail == nil {
			continue
		}

		thumbnailType, err := avatarThumbnail.GetFileType()
		if err != nil {
			return err
		}
		if !slices.Contains([]string{enum.AssetFileTypeJpeg, enum.AssetFileTypePng}, thumbnailType) {
			return fmt.Errorf("%s file type is not allowed", part)
		}
	}

	return nil
}

func (c *SaveAvatarCommand) getAvatarThumbnails() map[string]*value_object.AvatarAsset {
	return map[string]*value_object.AvatarAsset{
		enum.ThumbnailPartHead:      c.AvatarHead,
		enum.ThumbnailPartUpperBody: c.AvatarUpperBody,
		enum.ThumbnailPartFullBody:  c.AvatarFullBody,
	}
}

func (c *SaveAvatarCommand) hasAvatarID() bool {
	return c.AvatarID != ""
}

type SaveAvatarResponse struct {
	Avatar *entity.Avatar `json:"avatar"`
}

// SaveAvatarUseCase TODO: ticket TFB-88 [Server] Separate save_avatar_usecase into create_avatar and update_avatar
type SaveAvatarUseCase struct {
	repository repository.ISaveAvatarRepository
	storage    storage.IAssetStorage
	service    service.IAvatarService
}

func NewSaveAvatarUseCase(
	repository repository.ISaveAvatarRepository,
	storage storage.IAssetStorage,
	service service.IAvatarService,
) *SaveAvatarUseCase {
	return &SaveAvatarUseCase{
		repository: repository,
		storage:    storage,
		service:    service,
	}
}

func (u *SaveAvatarUseCase) Execute(ctx context.Context, cmd *SaveAvatarCommand) (*SaveAvatarResponse, error) {
	if err := cmd.Validate(); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.ValidateError,
			err,
		)
	}

	var avatar *entity.Avatar
	var err error
	if cmd.hasAvatarID() {
		avatar, err = u.getAvatar(ctx, cmd.AvatarID)
		if err != nil {
			return nil, core_error.NewCoreError(
				avatar_errors.GetAvatarError,
				err,
			)
		}
	} else {
		avatar = entity.NewAvatar(&entity.AvatarParams{
			AvatarID:     cmd.AvatarID,
			XrID:         cmd.XrID,
			AppID:        cmd.AppID,
			Type:         cmd.Type,
			AvatarFormat: cmd.AvatarFormat,
		})
	}

	// TODO: ticket TFB-86: [Server] While saving avatars, use gorutine to upload all assets instead
	// of uploading each avatar asset one by one
	thumbnails := cmd.getAvatarThumbnails()
	for part, thumbnail := range thumbnails {
		if thumbnail == nil {
			continue
		}

		toPath := u.service.GetAvatarThumbnailUploadPath(cmd.XrID, avatar.AvatarID, part, thumbnail.GetExt())
		outputPath, err := u.uploadFile(ctx, toPath, thumbnail)
		if err != nil {
			return nil, core_error.NewCoreError(
				avatar_errors.AssetUploadError,
				err,
			)
		}
		avatar.SetAvatarThumbnail(part, outputPath)
	}

	// upload avatar binary
	toPath := u.service.GetAvatarUploadPath(cmd.XrID, avatar.AvatarID, cmd.AvatarAsset.GetExt())
	outputPath, err := u.uploadFile(ctx, toPath, cmd.AvatarAsset)
	if err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.AssetUploadError,
			err,
		)
	}
	avatar.SetAvatarUrl(outputPath)

	if cmd.AvatarFormat != nil {
		avatar.SetAvatarFormat(cmd.AvatarFormat)
	}

	if err := u.repository.SaveAvatar(ctx, avatar); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.SaveAvatarError,
			err,
		)
	}

	return &SaveAvatarResponse{
		Avatar: avatar,
	}, nil
}

func (u *SaveAvatarUseCase) getAvatar(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error) {
	avatar, err := u.repository.GetAvatar(ctx, avatarID)
	if err != nil {
		return nil, err
	}
	if avatar == nil {
		return nil, fmt.Errorf("avatar not found")
	}
	return avatar, nil
}

func (u *SaveAvatarUseCase) uploadFile(ctx context.Context, toPath string, asset io.Reader) (value_object.AssetUrl, error) {
	return u.storage.SaveAsset(ctx, toPath, asset)
}
