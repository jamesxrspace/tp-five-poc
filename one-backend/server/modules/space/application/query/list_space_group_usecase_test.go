package query_test

import (
	"context"
	"testing"
	"time"

	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/space/application/query"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/enum"
	"xrspace.io/server/modules/space/domain/value_object"
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

func TestListSpaceGroupUseCase(t *testing.T) {
	s := new(ListTestSuite)
	suite.Run(t, s)
}

type ListTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo      *mongo.SpaceRepository
	spaceGroupRepo *mongo.SpaceGroupRepository
	spaceGroup_1   *entity.SpaceGroup
	spaceGroup_2   *entity.SpaceGroup
	spaceGroup_3   *entity.SpaceGroup
	space_1        *entity.Space
	space_2        *entity.Space
	space_3        *entity.Space
	q              *query.ListSpaceGroupQuery
}

func (s *ListTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroupRepo = mongo.NewSpaceGroupRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.spaceGroup_1 = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id_1",
	}
	s.spaceGroup_2 = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id_2",
	}
	s.spaceGroup_3 = &entity.SpaceGroup{
		SpaceGroupId: "test_space_group_id_3",
	}
	s.space_1 = &entity.Space{
		SpaceId: "test_space_id_1",
	}
	s.space_2 = &entity.Space{
		SpaceId: "test_space_id_2",
	}
	s.space_3 = &entity.Space{
		SpaceId: "test_space_id_3",
	}
	s.q = &query.ListSpaceGroupQuery{
		PaginationQuery: pagination.PaginationQuery{
			Offset: 0,
			Size:   3,
		},
	}
}

func (s *ListTestSuite) TestListSpaceGroupWithSPaceGroupId() {
	// arrange
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_3)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupIdResult := []string{}
	for _, spaceGroup := range res.Items {
		spaceGroupIdResult = append(spaceGroupIdResult, spaceGroup.SpaceGroupId)
	}
	s.Equal([]string{
		s.spaceGroup_1.SpaceGroupId,
		s.spaceGroup_2.SpaceGroupId,
		s.spaceGroup_3.SpaceGroupId,
	}, spaceGroupIdResult)
}

func (s *ListTestSuite) TestListSpaceGroupWithName() {
	// arrange
	s.spaceGroup_1.Name = "test_space_group_name_1"
	s.spaceGroup_2.Name = "test_space_group_name_2"
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupNameResult := []string{}
	for _, spaceGroup := range res.Items {
		spaceGroupNameResult = append(spaceGroupNameResult, spaceGroup.Name)
	}
	s.Equal([]string{
		s.spaceGroup_1.Name,
		s.spaceGroup_2.Name,
	}, spaceGroupNameResult)
}

func (s *ListTestSuite) TestListSpaceGroupWithDescription() {
	// arrange
	s.spaceGroup_1.Description = "test_space_group_description_1"
	s.spaceGroup_2.Description = "test_space_group_description_2"
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q
	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupDescriptionResult := []string{}
	for _, spaceGroup := range res.Items {
		spaceGroupDescriptionResult = append(spaceGroupDescriptionResult, spaceGroup.Description)
	}
	s.Equal([]string{
		s.spaceGroup_1.Description,
		s.spaceGroup_2.Description,
	}, spaceGroupDescriptionResult)
}

func (s *ListTestSuite) TestListSpaceGroupWithStatus() {
	// arrange
	s.spaceGroup_1.Status = enum.SpaceGroupEnabled
	s.spaceGroup_2.Status = enum.SpaceGroupEnabled
	s.spaceGroup_3.Status = enum.SpaceGroupDisabled
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_3)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupStatusResult := []value_object.SpaceGroupStatus{}
	for _, spaceGroup := range res.Items {
		spaceGroupStatusResult = append(spaceGroupStatusResult, spaceGroup.Status)
	}
	s.Equal([]value_object.SpaceGroupStatus{
		s.spaceGroup_1.Status,
		s.spaceGroup_2.Status,
		s.spaceGroup_3.Status,
	}, spaceGroupStatusResult)
}

func (s *ListTestSuite) TestListSpaceGroupWithEndAt() {
	// arrange
	end := time.Now().UTC().Truncate(time.Millisecond)
	s.spaceGroup_1.EndAt = end
	s.spaceGroup_2.EndAt = end
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupEndAtResult := []time.Time{}
	for _, spaceGroup := range res.Items {
		spaceGroupEndAtResult = append(spaceGroupEndAtResult, spaceGroup.EndAt)
	}
	s.Equal([]time.Time{
		s.spaceGroup_1.EndAt,
		s.spaceGroup_2.EndAt,
	}, spaceGroupEndAtResult)
}

func (s *ListTestSuite) TestListSpaceGroupWithStartAt() {
	// arrange
	start := time.Now().UTC().Truncate(time.Millisecond)
	s.spaceGroup_1.StartAt = start
	s.spaceGroup_2.StartAt = start
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupStartAtResult := []time.Time{}
	for _, spaceGroup := range res.Items {
		spaceGroupStartAtResult = append(spaceGroupStartAtResult, spaceGroup.StartAt)
	}
	s.Equal([]time.Time{
		s.spaceGroup_1.StartAt,
		s.spaceGroup_2.StartAt,
	}, spaceGroupStartAtResult)
}

// TestListSpaceGroupWithSpaceIds
func (s *ListTestSuite) TestListSpaceGroupWithSpaceIds() {
	// arrange
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.space_1.SpaceGroupId = s.spaceGroup_1.SpaceGroupId
	s.space_2.SpaceGroupId = s.spaceGroup_1.SpaceGroupId
	s.space_3.SpaceGroupId = s.spaceGroup_1.SpaceGroupId
	s.spaceRepo.Create(context.TODO(), s.space_1)
	s.spaceRepo.Create(context.TODO(), s.space_2)
	s.spaceRepo.Create(context.TODO(), s.space_3)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupSpaceIdResult := []string{}
	for _, space := range res.Items[0].Spaces {
		spaceGroupSpaceIdResult = append(spaceGroupSpaceIdResult, space.SpaceId)
	}
	s.Equal([]string{
		s.space_1.SpaceId,
		s.space_2.SpaceId,
		s.space_3.SpaceId,
	}, spaceGroupSpaceIdResult)
}

func (s *ListTestSuite) TestListSpaceGroupWithThumbnail() {
	// arrange
	s.spaceGroup_1.Thumbnail = "test_space_group_thumbnail_1"
	s.spaceGroup_2.Thumbnail = "test_space_group_thumbnail_2"
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	spaceGroupThumbnailResult := []string{}
	for _, spaceGroup := range res.Items {
		spaceGroupThumbnailResult = append(spaceGroupThumbnailResult, spaceGroup.Thumbnail)
	}
	s.Equal([]string{
		s.spaceGroup_1.Thumbnail,
		s.spaceGroup_2.Thumbnail,
	}, spaceGroupThumbnailResult)

}

func (s *ListTestSuite) TestListSpaceGroupWithOffestAndSize() {
	// arrange
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_3)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := query.ListSpaceGroupQuery{
		PaginationQuery: pagination.PaginationQuery{
			Offset: 0,
			Size:   2,
		},
	}

	// act
	resz, _ := uc.Execute(context.TODO(), &q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	s.Equal(3, res.Total)
	s.Equal(2, len(res.Items))
}

func (s *ListTestSuite) TestListSpaceGroupWithoutArchivedDocument() {
	// arrange
	now := time.Now().UTC().Truncate(time.Millisecond)
	s.spaceGroup_3.ArchivedAt = now
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_1)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_2)
	s.spaceGroupRepo.Create(context.TODO(), s.spaceGroup_3)

	uc := query.NewListSpaceGroupUseCase(s.spaceGroupRepo)
	q := s.q

	// act
	resz, _ := uc.Execute(context.TODO(), q)
	res := resz.(*query.ListSpaceGroupResponse)

	// assert
	s.Equal(2, res.Total)
	s.Equal(2, len(res.Items))
}
