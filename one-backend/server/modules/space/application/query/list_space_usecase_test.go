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
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

func TestListSpaceUseCase(t *testing.T) {
	s := new(TestSpaceSuite)
	suite.Run(t, s)
}

type TestSpaceSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	spaceRepo *mongo.SpaceRepository
	space_1   *entity.Space
	space_2   *entity.Space
	space_3   *entity.Space
}

func (s *TestSpaceSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.spaceRepo = mongo.NewSpaceRepository(s.InmemMongoBasicTestSuite.DbDoc)
	s.space_1 = &entity.Space{
		SpaceId: "test_space_id_1",
	}
	s.space_2 = &entity.Space{
		SpaceId: "test_space_id_2",
	}
	s.space_3 = &entity.Space{
		SpaceId: "test_space_id_3",
	}
}

func (s *TestSpaceSuite) TestListSpaceUseCase() {
	// arrange
	s.space_1.Name = "test1"
	s.space_2.Name = "test2"
	s.space_3.Name = "test3"
	s.spaceRepo.Create(context.Background(), s.space_1)
	s.spaceRepo.Create(context.Background(), s.space_2)
	s.spaceRepo.Create(context.Background(), s.space_3)

	uc := query.NewListSpaceUseCase(s.spaceRepo)
	q := query.ListSpaceQuery{
		PaginationQuery: pagination.PaginationQuery{
			Offset: 0,
			Size:   2,
		},
	}

	// act
	resz, _ := uc.Execute(context.TODO(), &q)
	res := resz.(*query.ListSpaceResponse)

	// assert
	s.Equal(3, res.Total)
	s.Equal(2, len(res.Items))
	s.Equal("test1", res.Items[0].Name)
	s.Equal("test2", res.Items[1].Name)
}

func (s *TestSpaceSuite) TestListSpaceWithThumbnail() {
	// arrange
	s.space_1.Thumbnail = "test_spacep_thumbnail_1"
	s.space_2.Thumbnail = "test_spacep_thumbnail_2"
	s.spaceRepo.Create(context.Background(), s.space_1)
	s.spaceRepo.Create(context.Background(), s.space_2)

	// act
	uc := query.NewListSpaceUseCase(s.spaceRepo)
	q := query.ListSpaceQuery{
		PaginationQuery: pagination.PaginationQuery{
			Offset: 0,
			Size:   2,
		},
	}

	// assert
	spaceThumbnailResult := []string{}
	resz, _ := uc.Execute(context.Background(), &q)
	res := resz.(*query.ListSpaceResponse)
	for _, space := range res.Items {
		spaceThumbnailResult = append(spaceThumbnailResult, space.Thumbnail)
	}
	s.Equal([]string{
		s.space_1.Thumbnail,
		s.space_2.Thumbnail,
	}, spaceThumbnailResult)
}

func (s *TestSpaceSuite) TestListSpaceWithAddressable() {
	// arrange
	s.space_1.Addressable = "test_spacep_addressable_1"
	s.space_2.Addressable = "test_spacep_addressable_2"
	s.spaceRepo.Create(context.Background(), s.space_1)
	s.spaceRepo.Create(context.Background(), s.space_2)

	// act
	uc := query.NewListSpaceUseCase(s.spaceRepo)
	q := query.ListSpaceQuery{
		PaginationQuery: pagination.PaginationQuery{
			Offset: 0,
			Size:   2,
		},
	}

	// assert
	spaceAddressableResult := []string{}
	resz, _ := uc.Execute(context.Background(), &q)
	res := resz.(*query.ListSpaceResponse)
	for _, space := range res.Items {
		spaceAddressableResult = append(spaceAddressableResult, space.Addressable)
	}
	s.Equal([]string{
		s.space_1.Addressable,
		s.space_2.Addressable,
	}, spaceAddressableResult)
}

func (s *TestSpaceSuite) TestListSpaceUseCaseWithoutArchivedAtDocument() {
	// arrange
	now := time.Now()
	s.space_1.Name = "test1"
	s.space_2.Name = "test2"
	s.space_3.Name = "test3"
	s.space_3.ArchivedAt = now
	s.spaceRepo.Create(context.Background(), s.space_1)
	s.spaceRepo.Create(context.Background(), s.space_2)
	s.spaceRepo.Create(context.Background(), s.space_3)

	uc := query.NewListSpaceUseCase(s.spaceRepo)
	q := query.ListSpaceQuery{
		PaginationQuery: pagination.PaginationQuery{
			Offset: 0,
			Size:   5,
		},
	}

	// act
	resz, _ := uc.Execute(context.TODO(), &q)
	res := resz.(*query.ListSpaceResponse)

	// assert
	s.Equal(2, res.Total)
	s.Equal("test1", res.Items[0].Name)
	s.Equal("test2", res.Items[1].Name)
}
