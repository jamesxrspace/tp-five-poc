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

func TestTestSuite(t *testing.T) {
	suite.Run(t, new(TestSuite))
}

type TestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
}

func (s *TestSuite) TestSave() {
	r := NewSpaceGroupRepository(s.DbDoc)
	ctx := context.Background()
	ent := &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
		Name:         "",
		Status:       "",
		StartAt:      time.Time{},
		EndAt:        time.Time{},
	}
	res, _ := r.Create(ctx, ent)

	res.Name = "New Name"
	_ = r.Save(ctx, res)

	res3, _ := r.FindById(ctx, res.SpaceGroupId)

	s.Equal(res3.Name, res.Name)
}

func (s *TestSuite) TestCreate() {
	r := NewSpaceGroupRepository(s.DbDoc)
	ctx := context.Background()
	ent := &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
		Name:         "test name",
		Status:       "",
		StartAt:      time.Time{},
		EndAt:        time.Time{},
	}
	res, _ := r.Create(ctx, ent)
	s.Equal(ent.SpaceGroupId, res.SpaceGroupId)
	s.Equal(ent.Name, res.Name)
}

func (s *TestSuite) TestCreateError() {
	r := NewSpaceGroupRepository(s.DbDoc)
	ctx := context.Background()
	ent := &entity.SpaceGroup{
		Name: "test name",
	}
	_, err := r.Create(ctx, ent)
	s.Equal(true, errors.Is(ErrIdentifyFieldEmpty, err))

}
