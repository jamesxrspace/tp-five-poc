package command_test

import (
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/dependency/database/docdb"
	docdbLocal "xrspace.io/server/core/dependency/database/docdb/local"
	"xrspace.io/server/modules/space/application/command"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

func TestDeleteSpaceGroupUseCase(t *testing.T) {
	s := new(DeleteTestSuite)
	suite.Run(t, s)
}

type DeleteTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo      *mongo.SpaceRepository
	spaceGroupRepo *mongo.SpaceGroupRepository
	spaceGroup     *entity.SpaceGroup
	space          *entity.Space
}

func (s *DeleteTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroupRepo = mongo.NewSpaceGroupRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroup = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
	}
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup)
	s.space = &entity.Space{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		SpaceId:      "test_space_id",
	}
	s.spaceRepo.Create(context.TODO(), s.space)
}

func (s *DeleteTestSuite) TestDeleteSpaceGroup() {
	// arrange
	cmd := command.DeleteSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
	}

	// act
	uc := command.NewDeleteSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	uc.Execute(context.TODO(), &cmd)

	// assert
	spaceGroup, _ := s.spaceGroupRepo.FindById(context.TODO(), s.spaceGroup.SpaceGroupId)
	space, _ := s.spaceRepo.FindById(context.TODO(), s.space.SpaceId)
	s.NotEmpty(spaceGroup.ArchivedAt)
	s.NotEmpty(space.ArchivedAt)
}
