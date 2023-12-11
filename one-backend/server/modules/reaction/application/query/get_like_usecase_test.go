package query

import (
	"context"
	"testing"

	"github.com/alecthomas/assert/v2"
	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/enum"
	"xrspace.io/server/modules/reaction/domain/repository"
	"xrspace.io/server/modules/reaction/port/output/repository/inmem"
	"xrspace.io/server/modules/reaction/port/output/repository/mongo"
)

func TestGetLikeUseCaseExecute(t *testing.T) {
	type args struct {
		query *GetLikeQuery
	}
	type want struct {
		likeResponse *GetLikeResponse
	}
	tests := []struct {
		name    string
		args    args
		want    want
		wantErr bool
		errCode string
	}{
		{
			name: "success_with_user_like",
			args: args{
				query: &GetLikeQuery{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
			},
			want: want{
				likeResponse: &GetLikeResponse{
					IsLike: true,
					Count:  2,
				},
			},
		},
		{
			name: "success_with_user_not_like",
			args: args{
				query: &GetLikeQuery{
					XrID:       "xrid3",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
			},
			want: want{
				likeResponse: &GetLikeResponse{
					IsLike: false,
					Count:  2,
				},
			},
		},
		{
			name: "success_with_no_matched_result",
			args: args{
				query: &GetLikeQuery{
					XrID:       "xrid4",
					TargetType: "feed",
					TargetID:   "feed_id2",
				},
			},
			want: want{
				likeResponse: &GetLikeResponse{
					IsLike: false,
					Count:  0,
				},
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			uc := NewGetLikeUseCase(define.Dependency{
				QueryRepo: inmem.NewQueryRepository(testLikes()),
			})

			// act
			got, err := uc.Execute(ctx, tt.args.query)
			if (err != nil) != tt.wantErr {
				t.Errorf("GetLikeUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			// assert
			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}

			result := got.(*GetLikeResponse)

			assert.Equal(t, tt.want.likeResponse, result)
		})
	}
}

func testLikes() map[interface{}]*entity.Like {
	return map[interface{}]*entity.Like{
		"001": entity.NewLike(&entity.LikeParams{
			XrID:       "xrid1",
			TargetType: "feed",
			TargetID:   "feed_id1",
			Status:     enum.LikeStatusLiked,
		}),
		"002": entity.NewLike(&entity.LikeParams{
			XrID:       "xrid2",
			TargetType: "feed",
			TargetID:   "feed_id1",
			Status:     enum.LikeStatusLiked,
		}),
		"003": entity.NewLike(&entity.LikeParams{
			XrID:       "xrid3",
			TargetType: "feed",
			TargetID:   "feed_id1",
			Status:     enum.LikeStatusUnliked,
		}),
	}
}

func TestLikeUseCase(t *testing.T) {
	suite.Run(t, new(LikeUseCaseTestSuite))
}

type LikeUseCaseTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	likeRepo  repository.ILikeRepository
	queryRepo define.IQueryRepository
}

func (s *LikeUseCaseTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.likeRepo = mongo.NewLikeRepository(s.DbDoc)
	s.queryRepo = mongo.NewQueryRepository(s.DbDoc)

	likes := testLikes()
	for _, like := range likes {
		err := s.likeRepo.Save(context.Background(), like)
		if err != nil {
			panic(err)
		}
	}

	s.queryRepo = mongo.NewQueryRepository(s.DbDoc)
}

func (s *LikeUseCaseTestSuite) TestLikeUseCaseExecute() {
	type args struct {
		query *GetLikeQuery
	}
	type want struct {
		likeResponse *GetLikeResponse
	}
	tests := []struct {
		name    string
		args    args
		want    want
		wantErr bool
		errCode string
	}{
		{
			name: "success_with_user_like",
			args: args{
				query: &GetLikeQuery{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
			},
			want: want{
				likeResponse: &GetLikeResponse{
					IsLike: true,
					Count:  2,
				},
			},
		},
		{
			name: "success_with_user_not_like",
			args: args{
				query: &GetLikeQuery{
					XrID:       "xrid3",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
			},
			want: want{
				likeResponse: &GetLikeResponse{
					IsLike: false,
					Count:  2,
				},
			},
		},
		{
			name: "success_with_no_matched_result",
			args: args{
				query: &GetLikeQuery{
					XrID:       "xrid3",
					TargetType: "feed",
					TargetID:   "feed_id2",
				},
			},
			want: want{
				likeResponse: &GetLikeResponse{
					IsLike: false,
					Count:  0,
				},
			},
		},
	}
	for _, tt := range tests {
		s.T().Run(tt.name, func(t *testing.T) {
			// arrange
			uc := NewGetLikeUseCase(define.Dependency{
				QueryRepo: s.queryRepo,
			})

			// act
			got, err := uc.Execute(context.Background(), tt.args.query)
			if (err != nil) != tt.wantErr {
				s.T().Errorf("GetLikeUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			// assert
			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}

			result := got.(*GetLikeResponse)

			assert.Equal(t, tt.want.likeResponse, result)
		})
	}
}
