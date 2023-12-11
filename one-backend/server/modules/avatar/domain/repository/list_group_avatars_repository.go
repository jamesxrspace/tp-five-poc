package repository

import (
	"context"

	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type ListAvatarPlayersByXrIDsResult struct {
	Items []*entity.AvatarPlayer
	Total int
}

type ListAvatarByAvatarIDsResult struct {
	Items []*entity.Avatar
	Total int
}

type IListGroupAvatarsRepository interface {
	ListAvatarPlayersByXrIDs(ctx context.Context, xrIDs []value_object.XrID, offset, size int) (*ListAvatarPlayersByXrIDsResult, error)
	ListAvatarByAvatarIDs(ctx context.Context, avatarIDs []value_object.AvatarID, offset, size int) (*ListAvatarByAvatarIDsResult, error)
	GetDefaultAvatar(ctx context.Context, appID value_object.AppID) (*entity.Avatar, error)
}
