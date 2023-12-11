package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/modules/avatar/domain/enum"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type AvatarParams Avatar

func NewAvatar(params *AvatarParams) *Avatar {
	if params.AvatarID == "" {
		params.AvatarID = value_object.AvatarID(uuid.New().String())
	}

	if params.Author == "" {
		params.Author = params.XrID
	}

	if params.CreatedAt.IsZero() {
		params.CreatedAt = time.Now()
	}

	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = time.Now()
	}

	if params.Thumbnail == nil {
		params.Thumbnail = &AvatarThumbnail{}
	}

	return &Avatar{
		AvatarID:     params.AvatarID,
		XrID:         params.XrID,
		AppID:        params.AppID,
		Type:         params.Type,
		AvatarFormat: params.AvatarFormat,
		AvatarUrl:    params.AvatarUrl,
		Thumbnail:    params.Thumbnail,
		CreatedAt:    params.CreatedAt,
		UpdatedAt:    params.UpdatedAt,
		Author:       params.Author,
	}
}

type Avatar struct {
	UpdatedAt    time.Time               `json:"updated_at" bson:"updated_at"`
	CreatedAt    time.Time               `json:"created_at" bson:"created_at"`
	AvatarFormat map[string]any          `json:"avatar_format" bson:"avatar_format"`
	Thumbnail    *AvatarThumbnail        `json:"thumbnail" bson:"thumbnail"`
	AvatarID     value_object.AvatarID   `json:"avatar_id" bson:"avatar_id"`
	XrID         value_object.XrID       `json:"xrid" bson:"xrid"`
	AppID        value_object.AppID      `json:"app_id" bson:"app_id"`
	Type         value_object.AvatarType `json:"type" bson:"type"`
	AvatarUrl    value_object.AssetUrl   `json:"avatar_url" bson:"avatar_url"`
	Author       value_object.XrID       `json:"author" bson:"author"`
}

type AvatarThumbnail struct {
	Head      value_object.AssetUrl `json:"head"`
	UpperBody value_object.AssetUrl `json:"upper_body"`
	FullBody  value_object.AssetUrl `json:"full_body"`
}

func (a *Avatar) SetAvatarUrl(avatarUrl value_object.AssetUrl) {
	a.AvatarUrl = avatarUrl
}

func (a *Avatar) SetAvatarFormat(avatarFormat map[string]any) {
	a.AvatarFormat = avatarFormat
}

func (a *Avatar) SetAvatarThumbnail(part string, url value_object.AssetUrl) {
	switch part {
	case enum.ThumbnailPartHead:
		a.Thumbnail.Head = url
	case enum.ThumbnailPartUpperBody:
		a.Thumbnail.UpperBody = url
	case enum.ThumbnailPartFullBody:
		a.Thumbnail.FullBody = url
	}
}
