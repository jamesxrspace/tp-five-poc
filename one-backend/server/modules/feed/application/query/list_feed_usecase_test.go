package query

import (
	"context"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	accountEntity "xrspace.io/server/modules/account/domain/entity"
	accountRepository "xrspace.io/server/modules/account/domain/repository"
	accountMongo "xrspace.io/server/modules/account/port/output/repository/mongo"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/enum"
	"xrspace.io/server/modules/feed/domain/repository"
	feedMongo "xrspace.io/server/modules/feed/port/output/repository/mongo"
)

func testFeeds() map[string]*entity.Feed {
	return map[string]*entity.Feed{
		"feed1": {
			ID:           "feed1",
			XrID:         "91c1101d-c21c-41a2-b191-17b5fb606842",
			ThumbnailUrl: "thumbnailurl1",
			Type:         enum.FeedTypeReel,
			RefID:        "refid1",
			Categories:   []string{"music", "culture"},
			Status:       enum.FeedStatusActive,
			UpdatedAt:    time.Date(2023, time.October, 18, 9, 30, 0, 0, time.UTC),
		},
		"feed2": {
			ID:           "feed2",
			XrID:         "91c1101d-c21c-41a2-b191-17b5fb606842",
			ThumbnailUrl: "thumbnailurl2",
			Type:         enum.FeedTypeReel,
			RefID:        "refid2",
			Categories:   []string{"music"},
			Status:       enum.FeedStatusActive,
			UpdatedAt:    time.Date(2023, time.October, 18, 10, 30, 0, 0, time.UTC),
		},
		"feed3": {
			ID:           "feed3",
			XrID:         "91c1101d-c21c-41a2-b191-17b5fb606842",
			ThumbnailUrl: "thumbnailurl3",
			Type:         enum.FeedTypeReel,
			RefID:        "refid3",
			Categories:   []string{"culture"},
			Status:       enum.FeedStatusActive,
			UpdatedAt:    time.Date(2023, time.October, 18, 11, 30, 0, 0, time.UTC),
		},
		"feed4": {
			ID:           "feed4",
			XrID:         "91c1101d-c21c-41a2-b191-17b5fb606842",
			ThumbnailUrl: "thumbnailurl4",
			Type:         enum.FeedTypeReel,
			RefID:        "refid3",
			Categories:   []string{"music", "culture"},
			Status:       enum.FeedStatusDeleted,
			UpdatedAt:    time.Date(2023, time.October, 18, 11, 30, 0, 0, time.UTC),
		},
		"feed5": {
			ID:           "feed5",
			XrID:         "91c1101d-c21c-41a2-b191-17b5fb606842",
			ThumbnailUrl: "thumbnailurl5",
			Type:         enum.FeedTypeReel,
			RefID:        "refid5",
			Categories:   []string{"talk_show"},
			Status:       enum.FeedStatusActive,
			UpdatedAt:    time.Date(2023, time.October, 18, 12, 30, 0, 0, time.UTC),
		},
	}
}

func testCategories() map[string]*assetEntity.CategoryItem {
	return map[string]*assetEntity.CategoryItem{
		"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
		"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
	}
}

func testAccounts() map[string]*accountEntity.Account {
	return map[string]*accountEntity.Account{
		"xrid1": {
			ID:       "001",
			XrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
			Nickname: "xrspacetest1001",
		},
		"xrid2": {
			ID:       "002",
			XrID:     "0e319304-70cb-4500-810f-7434b9f71af7",
			Nickname: "xrspacetest1002",
		},
	}
}

func TestListFeedUseCase(t *testing.T) {
	suite.Run(t, new(ListFeedTestSuite))
}

type ListFeedTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	feedRepo     repository.IFeedRepository
	accountRepo  accountRepository.IAccountRepository
	categoryRepo *mongo.GenericMongoRepository[*assetEntity.CategoryItem]
	queryRepo    define.IQueryRepository
}

func (s *ListFeedTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.feedRepo = feedMongo.NewFeedRepository(s.DbDoc)
	s.accountRepo = accountMongo.NewAccountRepository(s.DbDoc)
	// TODO: should be refactored after this PR being merged https://github.com/XRSPACE-Inc/tp-five/pull/710
	s.categoryRepo = mongo.NewGenericMongoRepository[*assetEntity.CategoryItem](s.DbDoc.Collection("category"))

	accounts := testAccounts()
	for _, account := range accounts {
		err := s.accountRepo.Save(context.Background(), account)
		if err != nil {
			panic(err)
		}
	}

	categories := testCategories()
	for _, category := range categories {
		s.categoryRepo.Save(context.Background(), category)
	}

	feeds := testFeeds()
	for _, feed := range feeds {
		s.feedRepo.Save(context.Background(), feed)
	}

	s.queryRepo = feedMongo.NewQueryRepository(s.DbDoc)
}

func (s *ListFeedTestSuite) TestListFeedUseCase_Execute() {
	type args struct {
		query *ListFeedQuery
	}
	type want struct {
		feeds     []*ResponseFeed
		feedCount int
		total     int
	}
	tests := []struct {
		args    args
		name    string
		errMsg  string
		want    want
		wantErr bool
	}{
		{
			name: "success_query_all",
			args: args{
				query: &ListFeedQuery{
					UserXrID: "0e319304-70cb-4500-810f-7434b9f71af7",
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   10,
					},
				},
			},
			want: want{
				feedCount: 4,
				feeds: []*ResponseFeed{
					{
						ID:            "feed5",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid5",
							ThumbnailUrl: "thumbnailurl5",
						},
						Categories: []string{"talk_show"},
						UpdatedAt:  time.Date(2023, time.October, 18, 12, 30, 0, 0, time.UTC),
					},
					{
						ID:            "feed3",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid3",
							ThumbnailUrl: "thumbnailurl3",
						},
						Categories: []string{"culture"},
						UpdatedAt:  time.Date(2023, time.October, 18, 11, 30, 0, 0, time.UTC),
					},
					{
						ID:            "feed2",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid2",
							ThumbnailUrl: "thumbnailurl2",
						},
						Categories: []string{"music"},
						UpdatedAt:  time.Date(2023, time.October, 18, 10, 30, 0, 0, time.UTC),
					},
					{
						ID:            "feed1",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid1",
							ThumbnailUrl: "thumbnailurl1",
						},
						Categories: []string{"music", "culture"},
						UpdatedAt:  time.Date(2023, time.October, 18, 9, 30, 0, 0, time.UTC),
					},
				},
				total: 4,
			},
		},
		{
			name: "success_query_by_categories",
			args: args{
				query: &ListFeedQuery{
					UserXrID:   "0e319304-70cb-4500-810f-7434b9f71af7",
					Categories: []string{"music"},
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   10,
					},
				},
			},
			want: want{
				feedCount: 2,
				feeds: []*ResponseFeed{
					{
						ID:            "feed2",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid2",
							ThumbnailUrl: "thumbnailurl2",
						},
						Categories: []string{"music"},
						UpdatedAt:  time.Date(2023, time.October, 18, 10, 30, 0, 0, time.UTC),
					},
					{
						ID:            "feed1",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid1",
							ThumbnailUrl: "thumbnailurl1",
						},
						Categories: []string{"music", "culture"},
						UpdatedAt:  time.Date(2023, time.October, 18, 9, 30, 0, 0, time.UTC),
					},
				},
				total: 2,
			},
		},
		{
			name: "success_query_by_categories_and_offset",
			args: args{
				query: &ListFeedQuery{
					UserXrID:   "0e319304-70cb-4500-810f-7434b9f71af7",
					Categories: []string{"music", "culture"},
					PaginationQuery: pagination.PaginationQuery{
						Offset: 1,
						Size:   2,
					},
				},
			},
			want: want{
				feedCount: 1,
				feeds: []*ResponseFeed{
					{
						ID:            "feed1",
						OwnerXrID:     "91c1101d-c21c-41a2-b191-17b5fb606842",
						OwnerNickname: "xrspacetest1001",
						Type:          enum.FeedTypeReel,
						Content: &Content{
							RefID:        "refid1",
							ThumbnailUrl: "thumbnailurl1",
						},
						Categories: []string{"music", "culture"},
						UpdatedAt:  time.Date(2023, time.October, 18, 9, 30, 0, 0, time.UTC),
					},
				},
				total: 3,
			},
		},
		{
			name: "success_query_feeds_excluded_self",
			args: args{
				query: &ListFeedQuery{
					UserXrID: "91c1101d-c21c-41a2-b191-17b5fb606842",
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   10,
					},
				},
			},
			want: want{
				feedCount: 0,
				feeds:     []*ResponseFeed{},
				total:     0,
			},
		},
	}
	for _, tt := range tests {
		s.T().Run(tt.name, func(t *testing.T) {
			// arrange
			dep := define.Dependency{
				QueryRepo: s.queryRepo,
			}
			u := NewListFeedUseCase(dep)

			// act
			got, err := u.Execute(context.Background(), tt.args.query)
			if (err != nil) != tt.wantErr {
				t.Errorf("ListFeedUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			result := got.(pagination.PaginationResponse[*ResponseFeed])

			// assert
			assert.Equal(t, tt.want.feedCount, len(result.Items))
			assert.Equal(t, tt.want.feeds, result.Items)
			assert.Equal(t, tt.want.total, result.Total)
		})
	}
}
