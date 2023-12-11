package inmem

import (
	"context"

	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

var _ repository.IAvatarRepository = (*AvatarRepository)(nil)

type AvatarRepository struct {
	avatars       map[value_object.AvatarID]*entity.Avatar
	avatarPlayers map[value_object.XrID]*entity.AvatarPlayer
}

func NewAvatarRepository(
	avatars map[value_object.AvatarID]*entity.Avatar,
	avatarPlayer map[value_object.XrID]*entity.AvatarPlayer,
) *AvatarRepository {

	if avatars == nil {
		avatars = make(map[value_object.AvatarID]*entity.Avatar)
	}

	if avatarPlayer == nil {
		avatarPlayer = make(map[value_object.XrID]*entity.AvatarPlayer)
	}

	return &AvatarRepository{
		avatars:       avatars,
		avatarPlayers: avatarPlayer,
	}
}

func (r *AvatarRepository) GetAvatarPlayer(_ context.Context, xrID value_object.XrID) (*entity.AvatarPlayer, error) {
	if r.avatarPlayers == nil {
		r.avatarPlayers = make(map[value_object.XrID]*entity.AvatarPlayer)
	}
	if _, ok := r.avatarPlayers[xrID]; !ok {
		return nil, nil
	}
	return r.avatarPlayers[xrID], nil
}

func (r *AvatarRepository) SaveAvatarPlayer(_ context.Context, avatarPlayer *entity.AvatarPlayer) error {
	if r.avatarPlayers == nil {
		r.avatarPlayers = make(map[value_object.XrID]*entity.AvatarPlayer)
	}
	r.avatarPlayers[avatarPlayer.XrID] = avatarPlayer
	return nil
}

func (r *AvatarRepository) GetAvatar(_ context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error) {
	avatar, ok := r.avatars[avatarID]
	if !ok {
		return nil, nil
	}

	return avatar, nil
}

func (r *AvatarRepository) SaveAvatar(_ context.Context, ent *entity.Avatar) error {
	r.avatars[ent.AvatarID] = ent
	return nil
}

func (r *AvatarRepository) ListAvatarPlayersByXrIDs(_ context.Context, xrIDs []value_object.XrID, offset, size int) (*repository.ListAvatarPlayersByXrIDsResult, error) {
	var items []*entity.AvatarPlayer
	for _, xrID := range xrIDs {
		if avatarPlayer, ok := r.avatarPlayers[xrID]; ok {
			items = append(items, avatarPlayer)
		}
	}

	return &repository.ListAvatarPlayersByXrIDsResult{
		Items: items,
		Total: len(items),
	}, nil
}

func (r *AvatarRepository) ListAvatarByAvatarIDs(_ context.Context, avatarIDs []value_object.AvatarID, offset, size int) (*repository.ListAvatarByAvatarIDsResult, error) {
	var items []*entity.Avatar
	for _, avatarID := range avatarIDs {
		if avatar, ok := r.avatars[avatarID]; ok {
			items = append(items, avatar)
		}
	}

	return &repository.ListAvatarByAvatarIDsResult{
		Items: items,
		Total: len(items),
	}, nil
}

func (r *AvatarRepository) GetDefaultAvatar(_ context.Context, appID value_object.AppID) (*entity.Avatar, error) {
	return &entity.Avatar{
		AvatarID: value_object.AvatarID("default_avatar_id"),
		AppID:    appID,
	}, nil
}
