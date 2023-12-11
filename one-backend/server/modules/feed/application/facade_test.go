package application_test

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"

	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/application"
	"xrspace.io/server/modules/feed/application/command"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/application/query"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/enum"
	"xrspace.io/server/modules/feed/port/output/repository/inmem"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
)

func TestCreateFeedUseCase_Execute(t *testing.T) {
	type args struct {
		cmd          *command.CreateFeedCommand
		defaultFeeds map[string]*entity.Feed
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		wantErr bool
	}{
		{
			name: "failed_create_reel_feed_without_reel_id",
			args: args{
				cmd: &command.CreateFeedCommand{
					XrID:         "xrid",
					Type:         enum.FeedTypeReel,
					Categories:   []string{"music", "culture"},
					ThumbnailUrl: "thumbnail_url",
				},
				defaultFeeds: nil,
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_create_reel_feed_without_thumbnail",
			args: args{
				cmd: &command.CreateFeedCommand{
					XrID:       "xrid",
					RefID:      "ref_id",
					Type:       enum.FeedTypeReel,
					Categories: []string{"music", "culture"},
				},
				defaultFeeds: nil,
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_without_tags",
			args: args{
				cmd: &command.CreateFeedCommand{
					XrID:         "xrid",
					Type:         enum.FeedTypeReel,
					RefID:        "ref_id",
					ThumbnailUrl: "thumbnail_url",
				},
				defaultFeeds: nil,
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := inmem.NewFeedRepository(tt.args.defaultFeeds)
			dep := define.Dependency{FeedRepo: repo}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.cmd)

			// got, err := u.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateFeedUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestCreateReelUseCase_Execute(t *testing.T) {
	type args struct {
		cmd               *command.CreateReelCommand
		defaultReels      map[string]*entity.Reel
		defaultCategories map[string]*assetEntity.CategoryItem
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		errMsg  string
		wantErr bool
	}{
		{
			name: "failed_if_video_is_empty",
			args: args{
				cmd: &command.CreateReelCommand{
					Description:      "description",
					Thumbnail:        "thumbnail",
					XrID:             "xrid",
					Xrs:              "xrs",
					ParentReelID:     "parent_reel_id",
					Categories:       []string{"music", "culture"},
					JoinMode:         enum.ReelJoinModeMe,
					MusicToMotionUrl: "http://music.com/motion",
				},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_if_xrs_is_empty",
			args: args{
				cmd: &command.CreateReelCommand{
					Description:      "description",
					Thumbnail:        "thumbnail",
					XrID:             "xrid",
					Video:            "video",
					ParentReelID:     "parent_reel_id",
					Categories:       []string{"music", "culture"},
					JoinMode:         enum.ReelJoinModeMe,
					MusicToMotionUrl: "http://music.com/motion",
				},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_if_categories_are_invalid",
			args: args{
				cmd: &command.CreateReelCommand{
					Description:      "description",
					Thumbnail:        "http://thumbnail",
					Video:            "http://video",
					Xrs:              "http://xrs",
					XrID:             "xrid",
					ParentReelID:     "parent_reel_id",
					Categories:       []string{"category1", "category2"},
					JoinMode:         enum.ReelJoinModeMe,
					MusicToMotionUrl: "http://music.com/motion",
				},
				defaultCategories: map[string]*assetEntity.CategoryItem{
					"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
					"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
				},
			},
			wantErr: true,
			errMsg:  "feed category not found",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			reelRepo := inmem.NewReelRepository(tt.args.defaultReels)
			queryRepo := inmem.NewQueryRepository(nil, tt.args.defaultCategories, nil, tt.args.defaultReels)
			dep := define.Dependency{ReelRepo: reelRepo, QueryRepo: queryRepo}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestDeleteReelUseCase_Execute(t *testing.T) {
	type args struct {
		cmd          *command.DeleteReelCommand
		defaultReels map[string]*entity.Reel
	}
	tests := []struct {
		args    args
		name    string
		errMsg  string
		wantErr bool
	}{
		{
			name: "failed_if_reel_id_is_empty",
			args: args{
				cmd: &command.DeleteReelCommand{
					ReelID: "",
					XrID:   "xrid",
				},
			},
			wantErr: true,
			errMsg:  "Key: 'DeleteReelCommand.ReelID' Error:Field validation",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := inmem.NewReelRepository(tt.args.defaultReels)
			dep := define.Dependency{ReelRepo: repo}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("DeleteReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}
		})
	}
}

func TestListReelUseCase_Execute(t *testing.T) {
	type args struct {
		query        *query.ListReelQuery
		defaultReels map[string]*entity.Reel
	}
	tests := []struct {
		args    args
		name    string
		errMsg  string
		wantErr bool
	}{
		{
			name: "failed_if_size_is_0",
			args: args{
				query: &query.ListReelQuery{
					XrID: "xrid",
					PaginationQuery: pagination.PaginationQuery{
						Size: 0,
					},
				},
			},
			wantErr: true,
			errMsg:  "Key: 'ListReelQuery.PaginationQuery.Size' Error:Field validation",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := inmem.NewQueryRepository(nil, nil, nil, tt.args.defaultReels)
			dep := define.Dependency{QueryRepo: repo}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.query)
			if (err != nil) != tt.wantErr {
				t.Errorf("ListReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}
		})
	}
}

func TestListFeedUseCase_Execute(t *testing.T) {
	type args struct {
		query        *query.ListFeedQuery
		defaultReels map[string]*entity.Reel
	}
	tests := []struct {
		args    args
		name    string
		errMsg  string
		wantErr bool
	}{
		{
			name: "failed_if_categories_not_in_enum",
			args: args{
				query: &query.ListFeedQuery{
					UserXrID:   "xrid1",
					Categories: []string{"category1"},
					PaginationQuery: pagination.PaginationQuery{
						Size: 2,
					},
				},
			},
			wantErr: true,
			errMsg:  "feed category not found",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := inmem.NewQueryRepository(nil, nil, nil, tt.args.defaultReels)
			dep := define.Dependency{QueryRepo: repo}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.query)
			if (err != nil) != tt.wantErr {
				t.Errorf("ListFeedUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}
		})
	}
}
