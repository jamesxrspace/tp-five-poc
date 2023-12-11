package application_test

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/domain/event"
	portEventBus "xrspace.io/server/core/arch/port/eventbus"
	dblocal "xrspace.io/server/core/dependency/database/docdb/local"
	InMemEventBus "xrspace.io/server/core/dependency/eventbus/inmem"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	feedCommand "xrspace.io/server/modules/feed/application/command"

	"xrspace.io/server/modules/reel/application"
	"xrspace.io/server/modules/reel/application/command"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/application/query"
	"xrspace.io/server/modules/reel/domain/entity"
	"xrspace.io/server/modules/reel/domain/enum"
	"xrspace.io/server/modules/reel/port/output/repository/inmem"

	"xrspace.io/server/core/arch/port/pagination"
)

func TestCreateReelUseCaseExecute(t *testing.T) {
	type args struct {
		cmd               *command.CreateReelCommand
		defaultReels      map[string]*entity.Reel
		defaultCategories map[string]*assetEntity.CategoryItem
	}
	tests := []struct {
		args    args
		name    string
		errCode string
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
			errCode: core_error.ValidationErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			reelRepo := inmem.NewReelRepository(tt.args.defaultReels)
			queryRepo := inmem.NewQueryRepository(tt.args.defaultCategories, tt.args.defaultReels)
			dep := define.Dependency{ReelRepo: reelRepo, QueryRepo: queryRepo}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestDeleteReelUseCaseExecute(t *testing.T) {
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

func TestPublishReelUseCaseExecute(t *testing.T) {
	type args struct {
		cmd          *command.PublishReelCommand
		defaultReels map[string]*entity.Reel
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		wantErr bool
	}{
		{
			name: "failed_if_reel_id_is_empty",
			args: args{
				cmd: &command.PublishReelCommand{
					ReelID: "",
					XrID:   "xrid",
				},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_if_reel_has_no_thumbnail",
			args: args{
				cmd: &command.PublishReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
				defaultReels: map[string]*entity.Reel{
					"reel_id": {
						ID:     "reel_id",
						XrID:   "xrid",
						Status: enum.ReelStatusDraft,
					},
				},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_if_reel_has_no_tags",
			args: args{
				cmd: &command.PublishReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
				defaultReels: map[string]*entity.Reel{
					"reel_id": {
						ID:        "reel_id",
						XrID:      "xrid",
						Status:    enum.ReelStatusDraft,
						Thumbnail: "thumbnail",
					},
				},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			reelRepo := inmem.NewReelRepository(tt.args.defaultReels)
			queryRepo := inmem.NewQueryRepository(nil, tt.args.defaultReels)
			bus := InMemEventBus.NewInMemEventBus()
			dep := define.Dependency{
				UnitOfWork: dblocal.NewUnitOfWork(),
				ReelRepo:   reelRepo,
				QueryRepo:  queryRepo,
				EventBus:   bus,
			}
			f := application.NewFacade(dep)
			handler := portEventBus.HandlerFunc[*feedCommand.CreateFeedCommand](f)
			f.EventBus.Subscribe(event.ReelPublishedEvent, handler)

			// act
			_, err := f.Execute(ctx, tt.args.cmd)

			if (err != nil) != tt.wantErr {
				t.Errorf("PublishReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestListReelUseCaseExecute(t *testing.T) {
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
			repo := inmem.NewQueryRepository(nil, tt.args.defaultReels)
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
