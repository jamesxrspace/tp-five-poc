package command_test

import (
	"context"
	"testing"
	"time"

	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/dependency/database/docdb"
	docdbLocal "xrspace.io/server/core/dependency/database/docdb/local"
	"xrspace.io/server/modules/space/application/command"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

func TestUpdateSpaceGroupUseCase(t *testing.T) {
	s := &UpdateTestSuite{}
	suite.Run(t, s)
}

type UpdateTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo      *mongo.SpaceRepository
	spaceGroupRepo *mongo.SpaceGroupRepository
	spaceGroup     *entity.SpaceGroup
	space_1        *entity.Space
	space_2        *entity.Space
}

func (s *UpdateTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroupRepo = mongo.NewSpaceGroupRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroup = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
	}
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup)
	s.space_1 = &entity.Space{
		SpaceId: "test_space_id_1",
	}
	s.space_2 = &entity.Space{
		SpaceId: "test_space_id_2",
	}
	s.spaceRepo.Create(context.TODO(), s.space_1)
	s.spaceRepo.Create(context.TODO(), s.space_2)
}

func (s *UpdateTestSuite) TestUpdateSpaceGroupWithName() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			Name: "update_space_group_name",
		},
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	s.Equal("update_space_group_name", res.Name)
}

func (s *UpdateTestSuite) TestUpdateSpaceGroupWithDescription() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			Description: "update_space_group_description",
		},
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	s.Equal("update_space_group_description", res.Description)
}

// TesetUPdateSpaceGroupWithStatus
func (s *UpdateTestSuite) TestUpdateSpaceGroupWithStatus() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			Status: "disabled",
		},
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	s.Equal("disabled", string(res.Status))
}

func (s *UpdateTestSuite) TestUpdateSpaceGroupWithStartAt() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	updateTime := time.Now().UTC().Truncate(time.Millisecond)
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			StartAt: updateTime,
		},
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	s.Equal(updateTime, res.StartAt)
}

func (s *UpdateTestSuite) TestUpdateSpaceGroupWithEndAt() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	updateTime := time.Now().UTC().Truncate(time.Millisecond)
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			EndAt: updateTime,
		},
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	s.Equal(updateTime, res.EndAt)
}

func (s *UpdateTestSuite) TestUpdateSpaceGroupWithThumbnail() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			Thumbnail: "update_space_group_thumbnail",
		},
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	s.Equal("update_space_group_thumbnail", res.Thumbnail)
}

// TestUpdateSpaceGroupWithSpaceIds
func (s *UpdateTestSuite) TestUpdateSpaceGroupWithSpaceIds() {
	// arrange
	uc := command.NewUpdateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	s.space_1.SpaceGroupId = "test_space_group_id"
	s.space_2.SpaceGroupId = "other_space_group_id"
	s.spaceRepo.Save(context.TODO(), s.space_1)
	s.spaceRepo.Save(context.TODO(), s.space_2)
	cmd := command.UpdateSpaceGroupCommand{
		SpaceGroupId: s.spaceGroup.SpaceGroupId,
		UpdateSpaceGroupBody: command.UpdateSpaceGroupBody{
			SpaceIds: []string{s.space_1.SpaceId, s.space_2.SpaceId},
		},
	}

	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.UpdateSpaceGroupResponse)

	// assert
	spaceResult := []string{}
	for _, space := range res.Spaces {
		spaceResult = append(spaceResult, space.SpaceId)
	}
	s.Equal([]string{s.space_1.SpaceId, s.space_2.SpaceId}, spaceResult)

	space_1, _ := s.spaceRepo.FindById(context.TODO(), s.space_1.SpaceId)
	s.Equal(res.SpaceGroupId, space_1.SpaceGroupId)
	space_2, _ := s.spaceRepo.FindById(context.TODO(), s.space_2.SpaceId)
	s.Equal(res.SpaceGroupId, space_2.SpaceGroupId)
}
