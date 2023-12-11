package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/modules/space/domain/enum"
	"xrspace.io/server/modules/space/domain/value_object"
)

type SpaceGroup struct {
	SpaceGroupId string                        `json:"space_group_id" bson:"space_group_id" pk:"true"`
	Name         string                        `json:"name" bson:"name"`
	Description  string                        `json:"description" bson:"description"`
	Thumbnail    string                        `json:"thumbnail" bson:"thumbnail"`
	Status       value_object.SpaceGroupStatus `json:"status" bson:"status"`
	StartAt      time.Time                     `json:"start_at" bson:"start_at"`
	EndAt        time.Time                     `json:"end_at" bson:"end_at"`
	ArchivedAt   time.Time                     `json:"-" bson:"archived_at,omitempty"`
}

func NewSpaceGroup(name string, startAt time.Time, endAt time.Time, description string, thumbail string) *SpaceGroup {
	s := &SpaceGroup{
		Name:        name,
		Status:      enum.SpaceGroupEnabled,
		StartAt:     startAt,
		EndAt:       endAt,
		Description: description,
		Thumbnail:   thumbail,
	}
	s.SetSpaceGroupId()
	return s
}

func (s *SpaceGroup) SetSpaceGroupId() {
	s.SpaceGroupId = uuid.New().String()
}

func (s *SpaceGroup) Archive() {
	now := time.Now().UTC()
	s.ArchivedAt = now
}
