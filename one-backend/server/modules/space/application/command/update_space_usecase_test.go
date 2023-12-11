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

func TestUpdateSpaceUseCase(t *testing.T) {
	s := &UpdateSpaceTestSuite{}
	suite.Run(t, s)
}

type UpdateSpaceTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo      *mongo.SpaceRepository
	spaceGroupRepo *mongo.SpaceGroupRepository
	spaceGroup     *entity.SpaceGroup
	space          *entity.Space
}

func (s *UpdateSpaceTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroupRepo = mongo.NewSpaceGroupRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroup = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
	}
	s.spaceGroupRepo.Create(context.Background(), s.spaceGroup)
	s.space = &entity.Space{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		SpaceId:      "test_space_id",
	}
	s.spaceRepo.Create(context.Background(), s.space)
}

func (s *UpdateSpaceTestSuite) TestUpdateSpaceUseCase() {
	// arrange
	startAt := time.Now().UTC().Truncate(time.Millisecond)
	endAt := startAt.Add(time.Hour * 24 * 3)
	space := entity.Space{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		SpaceId:      "test_space_id",
		Name:         "test_space_name",
		StartAt:      startAt,
		EndAt:        endAt,
	}
	s.spaceRepo.Create(context.Background(), &space)
	updateBody := command.UpdateSpaceBody{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		Name:         "test_space_name_updated",
		StartAt:      startAt.Add(time.Hour * 24 * 3),
		EndAt:        endAt.Add(time.Hour * 24 * 3),
	}
	cmd := command.UpdateSpaceCommand{
		SpaceId:         space.SpaceId,
		UpdateSpaceBody: updateBody,
	}

	// act
	uc := command.NewUpdateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	rz, _ := uc.Execute(context.TODO(), &cmd)
	r := rz.(*command.UpdateSpaceResponse)

	// assert
	s.Equal(cmd.SpaceId, r.SpaceId)
	s.Equal("test_space_name_updated", r.Name)
	s.Equal(startAt.Add(time.Hour*24*3), r.StartAt)
	s.Equal(endAt.Add(time.Hour*24*3), r.EndAt)

}

func (s *UpdateSpaceTestSuite) TestUpdateSpaceWithThumbnail() {
	// arrange
	uc := command.NewUpdateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	cmd := command.UpdateSpaceCommand{
		SpaceId: s.space.SpaceId,
		UpdateSpaceBody: command.UpdateSpaceBody{
			Thumbnail: "update_thumbnail",
		},
	}
	// act
	rz, _ := uc.Execute(context.Background(), &cmd)
	r := rz.(*command.UpdateSpaceResponse)

	// assert
	s.Equal("update_thumbnail", r.Thumbnail)
	s.Equal(s.space.SpaceId, r.SpaceId)
}

func (s *UpdateSpaceTestSuite) TestUpdateSpaceWithAddressable() {
	// arrange
	uc := command.NewUpdateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	cmd := command.UpdateSpaceCommand{
		SpaceId: s.space.SpaceId,
		UpdateSpaceBody: command.UpdateSpaceBody{
			Addressable: "update_addressable",
		},
	}
	// act
	rz, _ := uc.Execute(context.Background(), &cmd)
	r := rz.(*command.UpdateSpaceResponse)

	// assert
	s.Equal("update_addressable", r.Addressable)
	s.Equal(s.space.SpaceId, r.SpaceId)
}

func (s *UpdateSpaceTestSuite) TestUpdateSpaceUseCaseWithNotExistSpaceGroup() {
	// arrange
	startAt := time.Now().UTC().Truncate(time.Millisecond)
	endAt := startAt.Add(time.Hour * 24 * 3)
	space := &entity.Space{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		SpaceId:      "test_space_id",
		Name:         "test_space_name",
		StartAt:      startAt,
		EndAt:        endAt,
	}
	s.spaceRepo.Create(context.TODO(), space)

	updateBody := command.UpdateSpaceBody{
		SpaceGroupId: "space_group_id_not_exist",
		Name:         "test_space_name_updated",
		StartAt:      startAt.Add(time.Hour * 24 * 3),
		EndAt:        endAt.Add(time.Hour * 24 * 3),
	}
	cmd := command.UpdateSpaceCommand{
		SpaceId:         space.SpaceId,
		UpdateSpaceBody: updateBody,
	}

	// act
	uc := command.NewUpdateSpaceUseCase(s.spaceRepo, s.spaceGroupRepo, docdbLocal.NewUnitOfWork())
	_, err := uc.Execute(context.TODO(), &cmd)

	// assert
	var expErr *core_error.CodeError
	s.Equal(true, errors.As(err, &expErr))
	s.Equal(core_error.EntityNotFoundErrCode, expErr.ErrorCode)

}
