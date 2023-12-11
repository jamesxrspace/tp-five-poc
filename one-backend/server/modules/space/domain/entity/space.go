package entity

import (
	"time"

	"github.com/google/uuid"
)

type Space struct {
	SpaceId      string    `json:"space_id" bson:"space_id" pk:"true"`
	SpaceGroupId string    `json:"space_group_id,omitempty" bson:"space_group_id"`
	Name         string    `json:"name" bson:"name"`
	Description  string    `json:"description" bson:"description"`
	Thumbnail    string    `json:"thumbnail" bson:"thumbnail"`
	Addressable  string    `json:"addressable" bson:"addressable"`
	StartAt      time.Time `json:"start_at" bson:"start_at"`
	EndAt        time.Time `json:"end_at" bson:"end_at"`
	ArchivedAt   time.Time `json:"-" bson:"archived_at,omitempty"`
}

func NewSpace(spaceGroupId string, name string, description string, startAt time.Time, endAt time.Time, thumbnail string, addressable string) *Space {
	s := &Space{
		SpaceGroupId: spaceGroupId,
		Name:         name,
		Description:  description,
		Thumbnail:    thumbnail,
		Addressable:  addressable,
		StartAt:      startAt,
		EndAt:        endAt,
	}
	s.SetSpaceId()
	return s
}

func (s *Space) SetSpaceId() {
	s.SpaceId = uuid.New().String()
}

func (s *Space) Archive() {
	now := time.Now().UTC()
	s.ArchivedAt = now
}
