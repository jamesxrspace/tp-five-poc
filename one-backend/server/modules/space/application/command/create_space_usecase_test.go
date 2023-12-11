package command_test

import (
	"context"
	"errors"
	"testing"
	"time"

	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	docdbLocal "xrspace.io/server/core/dependency/database/docdb/local"
	"xrspace.io/server/modules/space/application/command"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

func TestCreateSpaceUseCase(t *testing.T) {
	s := new(CreateSpaceTestSuite)
	suite.Run(t, s)
}

type CreateSpaceTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo      *mongo.SpaceRepository
	spaceGroupRepo *mongo.SpaceGroupRepository
	spaceGroup     *entity.SpaceGroup
}

func (s *CreateSpaceTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroupRepo = mongo.NewSpaceGroupRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroup = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
	}
	s.spaceGroupRepo.Create(context.Background(), s.spaceGroup)
}

func (s *CreateSpaceTestSuite) TestCreateSpaceUseCase() {
	// arrange
	uc := command.NewCreateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	start := time.Now().UTC().Truncate(time.Millisecond)
	end := start.Add(time.Hour * 24 * 3)
	cmd := command.CreateSpaceCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		Name:         "test_space_name",
		StartAt:      start,
		EndAt:        end,
	}

	// act
	resz, _ := uc.Execute(context.Background(), &cmd)
	res := resz.(*command.CreateSpaceResponse)

	// assert
	s.Equal("test_space_name", res.Name)
	s.Equal(start, res.StartAt)
	s.Equal(end, res.EndAt)
	s.NotEmpty(res.SpaceId)
}

func (s *CreateSpaceTestSuite) TestCreateSpaceWithThumbnail() {
	// arrange
	uc := command.NewCreateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		Thumbnail:    "test_thumbnail",
	}

	// act
	resz, _ := uc.Execute(context.Background(), &cmd)
	res := resz.(*command.CreateSpaceResponse)

	// assert
	s.Equal("test_thumbnail", res.Thumbnail)
	s.NotEmpty(res.SpaceId)
}

func (s *CreateSpaceTestSuite) TestCreateSpaceWithAddressable() {
	// arrange
	uc := command.NewCreateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		Addressable:  "test_addressable",
	}

	// act
	resz, _ := uc.Execute(context.Background(), &cmd)
	res := resz.(*command.CreateSpaceResponse)

	// assert
	s.Equal("test_addressable", res.Addressable)
	s.NotEmpty(res.SpaceId)
}

func (s *CreateSpaceTestSuite) TestCreateSpaceUseCaseWithNotExistSpaceGroup() {
	// arrange
	uc := command.NewCreateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceCommand{
		SpaceGroupId: "id_not_exist",
		Name:         "test_space_name",
	}

	// act
	_, err := uc.Execute(context.Background(), &cmd)

	// assert
	var expErr *core_error.CodeError
	s.Equal(true, errors.As(err, &expErr))
	s.Equal(core_error.EntityNotFoundErrCode, expErr.ErrorCode)
}
