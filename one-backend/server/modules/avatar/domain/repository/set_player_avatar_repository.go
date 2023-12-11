package repository

import (
	"context"

	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type ISetPlayerAvatarRepository interface {
	GetAvatar(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error)
	GetAvatarPlayer(ctx context.Context, xrID value_object.XrID) (*entity.AvatarPlayer, error)
	SaveAvatarPlayer(ctx context.Context, ent *entity.AvatarPlayer) error
}
