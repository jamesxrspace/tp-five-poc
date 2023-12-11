package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/core/arch/domain"
	"xrspace.io/server/modules/reel/domain/enum"
	"xrspace.io/server/modules/reel/domain/event"
)

type ReelParams Reel

type Reel struct {
	domain.Entity `bson:"-"`

	CreatedAt        time.Time `json:"created_at" bson:"created_at"`
	UpdatedAt        time.Time `json:"updated_at" bson:"updated_at"`
	ID               string    `json:"id" bson:"id" pk:"true"`
	Description      string    `json:"description" bson:"description"`
	Thumbnail        string    `json:"thumbnail" bson:"thumbnail"`
	Video            string    `json:"video" bson:"video" validate:"required"`
	Xrs              string    `json:"xrs" bson:"xrs" validate:"required"`
	XrID             string    `json:"xrid" bson:"xrid"`
	JoinMode         string    `json:"join_mode" bson:"join_mode"`
	Status           string    `json:"status" bson:"status"`
	RootReelID       string    `json:"root_reel_id" bson:"root_reel_id"`
	ParentReelIDs    []string  `json:"parent_reel_ids" bson:"parent_reel_ids"`
	Categories       []string  `json:"categories" bson:"categories"`
	MusicToMotionUrl string    `json:"music_to_motion_url" bson:"music_to_motion_url"`
}

func NewReel(params *ReelParams) *Reel {
	params.ID = uuid.New().String()

	if params.Status == "" {
		params.Status = enum.ReelStatusDraft
	}

	now := time.Now()

	if params.CreatedAt.IsZero() {
		params.CreatedAt = now
	}

	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = now
	}
	return &Reel{
		ID:               params.ID,
		Description:      params.Description,
		Thumbnail:        params.Thumbnail,
		Video:            params.Video,
		Xrs:              params.Xrs,
		XrID:             params.XrID,
		JoinMode:         params.JoinMode,
		Status:           params.Status,
		ParentReelIDs:    params.ParentReelIDs,
		RootReelID:       params.RootReelID,
		Categories:       params.Categories,
		MusicToMotionUrl: params.MusicToMotionUrl,
		CreatedAt:        params.CreatedAt,
		UpdatedAt:        params.UpdatedAt,
	}
}

func (r *Reel) Delete() {
	r.Status = enum.ReelStatusDeleted
	r.UpdatedAt = time.Now()
}

func (r *Reel) Draft() {
	r.Status = enum.ReelStatusDraft
	r.UpdatedAt = time.Now()
}

func (r *Reel) Publish() {
	r.Status = enum.ReelStatusPublished
	r.UpdatedAt = time.Now()
	r.AddEvent(event.NewReelPublishedEvent(r.ID, r.XrID, r.Thumbnail, r.Categories))
}

func (r *Reel) IsOwner(xrID string) bool {
	return r.XrID == xrID
}

func (r *Reel) IsDraft() bool {
	return r.Status == enum.ReelStatusDraft
}

func (r *Reel) IsPublish() bool {
	return r.Status == enum.ReelStatusPublished
}

func (r *Reel) IsDelete() bool {
	return r.Status == enum.ReelStatusDeleted
}
