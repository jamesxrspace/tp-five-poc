package domain

import (
	"time"

	"github.com/google/uuid"
)

type NewFollowParams struct {
	ID          string
	XrID        string
	Status      FollowStatus
	FollowingID string
	CreatedAt   time.Time
}

type Follow struct {
	ID          string       `json:"id" bson:"id" pk:"true"`
	XrID        string       `json:"xrid" bson:"xrid"`
	Status      FollowStatus `json:"status" bson:"status"`
	FollowingID string       `json:"following_id" bson:"following_id"`
	CreatedAt   time.Time    `json:"created_at" bson:"created_at"`
	UpdatedAt   time.Time    `json:"updated_at" bson:"updated_at"`
}

func NewFollow(params NewFollowParams) *Follow {
	if params.ID == "" {
		params.ID = uuid.NewString()
	}

	now := time.Now()

	if params.CreatedAt.IsZero() {
		params.CreatedAt = now
	}

	return &Follow{
		ID:          params.ID,
		XrID:        params.XrID,
		Status:      params.Status,
		FollowingID: params.FollowingID,
		CreatedAt:   params.CreatedAt,
		UpdatedAt:   now,
	}
}

func (f *Follow) IsFollowing() bool {
	return f.Status == Following
}

func (f *Follow) Toggle() {
	if f.IsFollowing() {
		f.unfollow()
	} else {
		f.follow()
	}
}

func (f *Follow) follow() {
	f.Status = Following
	f.UpdatedAt = time.Now()
}

func (f *Follow) unfollow() {
	f.Status = Unfollowed
	f.UpdatedAt = time.Now()
}
