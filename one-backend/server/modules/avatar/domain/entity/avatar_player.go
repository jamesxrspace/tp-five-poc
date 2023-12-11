package entity

import (
	"errors"
	"time"

	"xrspace.io/server/modules/avatar/domain/value_object"
)

type AvatarPlayerParams AvatarPlayer

func NewAvatarPlayer(params *AvatarPlayerParams) *AvatarPlayer {

	if params.CreatedAt.IsZero() {
		params.CreatedAt = time.Now()
	}

	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = time.Now()
	}

	return &AvatarPlayer{
		XrID:      params.XrID,
		AvatarID:  params.AvatarID,
		CreatedAt: params.CreatedAt,
		UpdatedAt: params.UpdatedAt,
	}
}

type AvatarPlayer struct {
	CreatedAt time.Time                                    `bson:"created_at"`
	UpdatedAt time.Time                                    `bson:"updated_at"`
	AvatarID  map[value_object.AppID]value_object.AvatarID `bson:"avatar_id"`
	XrID      value_object.XrID                            `bson:"xrid"`
}

func (a *AvatarPlayer) SetCurrentAvatar(avatarXrID value_object.XrID, avatarID value_object.AvatarID, appID value_object.AppID) error {
	if a.XrID != avatarXrID {
		return errors.New("avatar not belong to player")
	}
	a.AvatarID[appID] = avatarID
	a.UpdatedAt = time.Now()
	return nil
}
