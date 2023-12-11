package mongo

import (
	"context"
	"errors"
	"testing"
	"time"

	"github.com/stretchr/testify/suite"

	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/space/domain/entity"
)

func TestSpace(t *testing.T) {
	suite.Run(t, new(TestSuite))
}

type TestSpaceRepo struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
}

func (s *TestSpaceRepo) TestCreate() {
	r := NewSpaceRepository(s.DbDoc)
	ctx := context.Background()
	ent := &entity.Space{
		SpaceId: "test_space_group_id",
		Name:    "test name",
		StartAt: time.Time{},
		EndAt:   time.Time{},
	}
	res, _ := r.Create(ctx, ent)
	s.Equal(ent.SpaceId, res.SpaceId)
	s.Equal(ent.Name, res.Name)
}

func (s *TestSpaceRepo) TestCreateError() {
	r := NewSpaceRepository(s.DbDoc)
	ctx := context.Background()
	ent := &entity.Space{
		Name: "test name",
	}
	_, err := r.Create(ctx, ent)
	s.Equal(true, errors.Is(ErrSpaceIdEmpty, err))

}

func (s *TestSpaceRepo) TestSave() {
	r := NewSpaceRepository(s.DbDoc)
	ctx := context.Background()
	ent := &entity.Space{
		SpaceId: "test_space_group_id",
		Name:    "",
		StartAt: time.Time{},
		EndAt:   time.Time{},
	}
	res, _ := r.Create(ctx, ent)

	res.Name = "New Name"
	_ = r.Save(ctx, res)

	res3, _ := r.FindById(ctx, res.SpaceId)

	s.Equal(res3.Name, res.Name)
}
