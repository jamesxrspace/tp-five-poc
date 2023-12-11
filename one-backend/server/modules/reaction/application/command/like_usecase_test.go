package command

import (
	"context"
	"testing"

	"github.com/alecthomas/assert/v2"
	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	dblocal "xrspace.io/server/core/dependency/database/docdb/local"
	"xrspace.io/server/core/dependency/unit_of_work"
	feedEntity "xrspace.io/server/modules/feed/domain/entity"
	feedRepository "xrspace.io/server/modules/feed/domain/repository"
	feedMongo "xrspace.io/server/modules/feed/port/output/repository/mongo"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/enum"
	"xrspace.io/server/modules/reaction/domain/repository"
	"xrspace.io/server/modules/reaction/port/output/repository/inmem"
	"xrspace.io/server/modules/reaction/port/output/repository/mongo"
)

func TestLikeUseCaseExecute(t *testing.T) {
	type args struct {
		cmd          *LikeCommand
		defaultLikes map[interface{}]*entity.Like
	}
	type want struct {
		LikeResponse *LikeResponse
		wantErr      bool
		likeStatus   enum.LikeStatus
	}
	tests := []struct {
		name    string
		args    args
		want    want
		wantErr bool
		errCode string
	}{
		{
			name: "success_toggle_to_like",
			args: args{
				cmd: &LikeCommand{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			want: want{
				LikeResponse: &LikeResponse{},
				wantErr:      false,
				likeStatus:   enum.LikeStatusLiked,
			},
		},
		{
			name: "success_toggle_to_unlike",
			args: args{
				cmd: &LikeCommand{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
				defaultLikes: map[interface{}]*entity.Like{
					"1": entity.NewLike(&entity.LikeParams{
						XrID:       "xrid1",
						TargetType: "feed",
						TargetID:   "feed_id1",
						Status:     enum.LikeStatusLiked,
					}),
				},
			},
			want: want{
				LikeResponse: &LikeResponse{},
				wantErr:      false,
				likeStatus:   enum.LikeStatusUnliked,
			},
		},
		{
			name: "failed_if_target_not_exist",
			args: args{
				cmd: &LikeCommand{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id2",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			wantErr: true,
			errCode: core_error.EntityNotFoundErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			uc := NewLikeUseCase(define.Dependency{
				UnitOfWork: dblocal.NewUnitOfWork(),
				QueryRepo:  inmem.NewQueryRepository(tt.args.defaultLikes),
				LikeRepo:   inmem.NewLikeRepository(tt.args.defaultLikes),
			})

			// act
			got, err := uc.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("LikeUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}

			result := got.(*LikeResponse)
			like, _ := uc.dep.QueryRepo.GetLike(ctx, define.GetLikeFilter{
				XrID:       tt.args.cmd.XrID,
				TargetType: tt.args.cmd.TargetType,
				TargetID:   tt.args.cmd.TargetID,
			})

			// assert
			assert.Equal(t, tt.want.LikeResponse, result)
			assert.Equal(t, tt.args.cmd.XrID, like.XrID)
			assert.Equal(t, tt.want.likeStatus, like.Status)
		})
	}
}

func TestLikeUseCase(t *testing.T) {
	suite.Run(t, new(LikeUseCaseTestSuite))
}

type LikeUseCaseTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	unit_of_work unit_of_work.IUnitOfWork
	likeRepo     repository.ILikeRepository
	feedRepo     feedRepository.IFeedRepository
	queryRepo    define.IQueryRepository
}

func (s *LikeUseCaseTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.unit_of_work = dblocal.NewUnitOfWork()
	s.likeRepo = mongo.NewLikeRepository(s.DbDoc)
	s.feedRepo = feedMongo.NewFeedRepository(s.DbDoc)
	s.feedRepo.Save(context.Background(), &feedEntity.Feed{
		ID: "feed1",
	})
	s.queryRepo = mongo.NewQueryRepository(s.DbDoc)
}

func (s *LikeUseCaseTestSuite) TestLikeUseCaseExecute() {
	type args struct {
		cmd *LikeCommand
	}
	type want struct {
		LikeResponse *LikeResponse
		wantErr      bool
		likeStatus   enum.LikeStatus
	}
	tests := []struct {
		name    string
		args    args
		want    want
		wantErr bool
		errCode string
	}{
		{
			name: "success_toggle_to_like",
			args: args{
				cmd: &LikeCommand{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
			},
			want: want{
				LikeResponse: &LikeResponse{},
				wantErr:      false,
				likeStatus:   enum.LikeStatusLiked,
			},
		},
		{
			name: "success_toggle_to_unlike",
			args: args{
				cmd: &LikeCommand{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id1",
				},
			},
			want: want{
				LikeResponse: &LikeResponse{},
				wantErr:      false,
				likeStatus:   enum.LikeStatusUnliked,
			},
		},
		{
			name: "failed_if_target_not_exist",
			args: args{
				cmd: &LikeCommand{
					XrID:       "xrid1",
					TargetType: "feed",
					TargetID:   "feed_id2",
				},
			},
			wantErr: true,
			errCode: core_error.EntityNotFoundErrCode,
		},
	}
	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			ctx := context.Background()
			uc := NewLikeUseCase(define.Dependency{
				UnitOfWork: s.unit_of_work,
				QueryRepo:  s.queryRepo,
				LikeRepo:   s.likeRepo,
			})

			// act
			got, err := uc.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				s.Errorf(err, "LikeUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Equal(s.T(), err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}

			result := got.(*LikeResponse)
			like, _ := uc.dep.QueryRepo.GetLike(ctx, define.GetLikeFilter{
				XrID:       tt.args.cmd.XrID,
				TargetType: tt.args.cmd.TargetType,
				TargetID:   tt.args.cmd.TargetID,
			})

			// assert
			assert.Equal(s.T(), tt.want.LikeResponse, result)
			assert.Equal(s.T(), tt.args.cmd.XrID, like.XrID)
			assert.Equal(s.T(), tt.want.likeStatus, like.Status)
		})
	}
}
