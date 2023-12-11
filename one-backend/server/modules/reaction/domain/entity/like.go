package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/modules/reaction/domain/enum"
)

type LikeParams Like

type Like struct {
	ID         string          `json:"id" bson:"id" pk:"true"`
	TargetType enum.TargetType `json:"target_type" bson:"target_type"`
	TargetID   string          `json:"target_id" bson:"target_id"`
	XrID       string          `json:"xrid" bson:"xrid"`
	Status     enum.LikeStatus `json:"status" bson:"status"`
	CreatedAt  time.Time       `json:"created_at" bson:"created_at"`
	UpdatedAt  time.Time       `json:"updated_at" bson:"updated_at"`
}

func NewLike(params *LikeParams) *Like {
	if params.ID == "" {
		params.ID = uuid.NewString()
	}

	now := time.Now()

	if params.CreatedAt.IsZero() {
		params.CreatedAt = now
	}

	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = now
	}

	return &Like{
		ID:         params.ID,
		TargetType: params.TargetType,
		TargetID:   params.TargetID,
		XrID:       params.XrID,
		Status:     params.Status,
		CreatedAt:  params.CreatedAt,
		UpdatedAt:  params.UpdatedAt,
	}
}

func (l *Like) IsLike() bool {
	return l.Status == enum.LikeStatusLiked
}

func (l *Like) Toggle() {
	if l.Status == enum.LikeStatusLiked {
		l.unlike()
	} else {
		l.like()
	}
}

func (l *Like) like() {
	l.Status = enum.LikeStatusLiked
	l.UpdatedAt = time.Now()
}

func (l *Like) unlike() {
	l.Status = enum.LikeStatusUnliked
	l.UpdatedAt = time.Now()
}
