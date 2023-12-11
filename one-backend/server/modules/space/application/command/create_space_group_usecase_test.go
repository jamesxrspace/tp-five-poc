package command_test

import (
	"context"
	"errors"
	"testing"
	"time"

	"github.com/stretchr/testify/mock"
	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	docdbLocal "xrspace.io/server/core/dependency/database/docdb/local"
	"xrspace.io/server/modules/space/application/command"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/error_code"
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

func TestCreateSpaceGroupUseCase(t *testing.T) {
	s := new(CreateTestSuite)
	suite.Run(t, s)
}

type CreateTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo      *mongo.SpaceRepository
	spaceGroupRepo *mongo.SpaceGroupRepository
	spaceGroup     *entity.SpaceGroup
	space_1        *entity.Space
	space_2        *entity.Space
}

func (s *CreateTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroupRepo = mongo.NewSpaceGroupRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroup = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id",
	}
	s.space_1 = &entity.Space{
		SpaceId: "test_space_id_1",
	}
	s.space_2 = &entity.Space{
		SpaceId: "test_space_id_2",
	}
}

func (s *CreateTestSuite) TestCreateSpaceGroupWithName() {
	// arrange
	uc := command.NewCreateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceGroupCommand{
		Name: "test_space_group_name",
	}
	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.CreateSpaceGroupResponse)

	// assert
	s.Equal("test_space_group_name", res.Name)
}

func (s *CreateTestSuite) TestCreateSpaceGroupWithStartAt() {
	// arrange
	uc := command.NewCreateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	start := time.Now().UTC().Truncate(time.Millisecond) // https://stackoverflow.com/questions/52061986/time-precision-issue-on-comparison-in-mongodb-driver-in-go-and-possibly-in-other
	cmd := command.CreateSpaceGroupCommand{
		Name:    "test_space_group_name",
		StartAt: start,
	}

	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.CreateSpaceGroupResponse)

	// assert
	s.Equal(start, res.StartAt)
}

func (s *CreateTestSuite) TestCreateSpaceGroupWithEndAt() {
	// arrange
	uc := command.NewCreateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	end := time.Now().UTC().Truncate(time.Millisecond)
	cmd := command.CreateSpaceGroupCommand{
		Name:  "test_space_group_name",
		EndAt: end,
	}

	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.CreateSpaceGroupResponse)

	// assert
	s.Equal(end, res.EndAt)
}

func (s *CreateTestSuite) TestCreateSpaceGroupWithDescription() {
	// arrange
	uc := command.NewCreateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceGroupCommand{
		Name:        "test_space_group_name",
		Description: "test_space_group_description",
	}

	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.CreateSpaceGroupResponse)

	// assert
	s.Equal("test_space_group_description", res.Description)
}

func (s *CreateTestSuite) TestCreateSpaceGroupWithThumbnail() {
	// arrange
	uc := command.NewCreateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceGroupCommand{
		Name:      "test_space_group_name",
		Thumbnail: "test_space_group_thumbnail",
	}

	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.CreateSpaceGroupResponse)

	// assert
	s.Equal("test_space_group_thumbnail", res.Thumbnail)
}

func (s *CreateTestSuite) TestCreateSpaceGroupWithSpaceIds() {
	// arrange
	uc := command.NewCreateSpaceGroupUseCase(s.spaceGroupRepo, s.spaceRepo, docdbLocal.NewUnitOfWork())
	s.space_1.SpaceGroupId = "other_space_group_id"
	s.spaceRepo.Create(context.TODO(), s.space_1)
	s.spaceRepo.Create(context.TODO(), s.space_2)
	cmd := command.CreateSpaceGroupCommand{
		Name:     "test_space_group_name",
		SpaceIds: []string{s.space_1.SpaceId, s.space_2.SpaceId},
	}

	//act
	resz, _ := uc.Execute(context.TODO(), &cmd)
	res := resz.(*command.CreateSpaceGroupResponse)

	// assert
	spacesResult := []string{}
	for _, space := range res.Spaces {
		spacesResult = append(spacesResult, space.SpaceId)
	}
	s.Equal([]string{s.space_1.SpaceId, s.space_2.SpaceId}, spacesResult)

	space_1, _ := s.spaceRepo.FindById(context.TODO(), s.space_1.SpaceId)
	s.Equal(res.SpaceGroupId, space_1.SpaceGroupId)
	space_2, _ := s.spaceRepo.FindById(context.TODO(), s.space_2.SpaceId)
	s.Equal(res.SpaceGroupId, space_2.SpaceGroupId)
}

type mockRepo struct {
	mock.Mock
}

func (m *mockRepo) Create(ctx context.Context, spaceGroup *entity.SpaceGroup) (*entity.SpaceGroup, error) {
	args := m.Mock.Called(spaceGroup)
	return spaceGroup, args.Error(1)
}

func (m *mockRepo) FindById(ctx context.Context, spaceGroupId string) (*entity.SpaceGroup, error) {
	args := m.Called(spaceGroupId)
	return args.Get(0).(*entity.SpaceGroup), args.Error(1)
}

func (m *mockRepo) Save(ctx context.Context, spaceGroup *entity.SpaceGroup) error {
	args := m.Called(spaceGroup)
	return args.Error(1)
}

func (s *CreateTestSuite) TestCreateSpaceGroupUseCaseWithCreateError() {
	// arrange
	mRepo := new(mockRepo)
	mRepo.Mock.On("Create", mock.Anything).Return(nil, errors.New("create error"))
	uc := command.NewCreateSpaceGroupUseCase(mRepo, nil, docdbLocal.NewUnitOfWork())
	cmd := command.CreateSpaceGroupCommand{Name: "not important"}

	// act
	result, err := uc.Execute(context.TODO(), &cmd)

	// assert
	s.Nil(result)
	s.Error(err)
	s.Equal(error_code.CreateSpaceGroupError.ErrorCode, err.(*core_error.CoreError).MError.ErrorCode)
}
