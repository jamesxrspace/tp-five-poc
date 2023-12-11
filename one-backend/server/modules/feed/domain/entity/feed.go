package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/modules/feed/domain/enum"
)

type FeedParams Feed

type Feed struct {
	CreatedAt    time.Time `json:"created_at" bson:"created_at"`
	UpdatedAt    time.Time `json:"updated_at" bson:"updated_at"`
	ID           string    `json:"id" bson:"id" pk:"true"`
	XrID         string    `json:"xrid" bson:"xrid"`
	Type         string    `json:"type" bson:"type"`
	RefID        string    `json:"ref_id" bson:"ref_id"`
	ThumbnailUrl string    `json:"thumbnail_url" bson:"thumbnail_url"`
	Status       string    `json:"status" bson:"status"`
	Categories   []string  `json:"categories" bson:"categories"`
}

func NewFeed(params *FeedParams) *Feed {
	if params.ID == "" {
		params.ID = uuid.New().String()
	}

	now := time.Now()

	if params.CreatedAt.IsZero() {
		params.CreatedAt = now
	}

	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = now
	}

	return &Feed{
		ID:           params.ID,
		XrID:         params.XrID,
		Type:         params.Type,
		RefID:        params.RefID,
		ThumbnailUrl: params.ThumbnailUrl,
		Categories:   params.Categories,
		Status:       enum.FeedStatusActive,
		CreatedAt:    params.CreatedAt,
		UpdatedAt:    params.UpdatedAt,
	}
}

func (f *Feed) IsOwner(xrID string) bool {
	return f.XrID == xrID
}
