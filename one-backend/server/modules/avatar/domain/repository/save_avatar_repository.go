package repository

import (
	"context"

	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type ISaveAvatarRepository interface {
	GetAvatar(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error)
	SaveAvatar(ctx context.Context, ent *entity.Avatar) error
}
