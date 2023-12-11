package command_test

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"xrspace.io/server/core/arch/core_error"
	dblocal "xrspace.io/server/core/dependency/database/docdb/local"
	InMemEventBus "xrspace.io/server/core/dependency/eventbus/inmem"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/reel/application/command"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/domain/entity"
	"xrspace.io/server/modules/reel/domain/enum"
	"xrspace.io/server/modules/reel/port/output/repository/inmem"
)

func TestPublishReelUseCaseExecute(t *testing.T) {
	type args struct {
		cmd               *command.PublishReelCommand
		defaultReels      map[string]*entity.Reel
		defaultCategories map[string]*assetEntity.CategoryItem
	}
	type want struct {
		publishReelResponse *command.PublishReelResponse
		reelStatus          string
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		want    want
		wantErr bool
	}{
		{
			name: "success_publish_reel",
			args: args{
				cmd: &command.PublishReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
				defaultReels: map[string]*entity.Reel{
					"reel_id": {
						ID:         "reel_id",
						XrID:       "xrid",
						Status:     enum.ReelStatusDraft,
						Thumbnail:  "http://thumbnail",
						Categories: []string{"music", "culture"},
					},
				},
				defaultCategories: map[string]*assetEntity.CategoryItem{
					"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
					"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
				},
			},
			want: want{
				publishReelResponse: &command.PublishReelResponse{},
				reelStatus:          enum.ReelStatusPublished,
			},
		},
		{
			name: "success_if_reel_already_published",
			args: args{
				cmd: &command.PublishReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
				defaultReels: map[string]*entity.Reel{
					"reel_id": {
						ID:     "reel_id",
						XrID:   "xrid",
						Status: enum.ReelStatusPublished,
					},
				},
			},
			want: want{
				publishReelResponse: &command.PublishReelResponse{},
				reelStatus:          enum.ReelStatusPublished,
			},
		},
		{
			name: "failed_if_reel_not_found",
			args: args{
				cmd: &command.PublishReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
			},
			wantErr: true,
			errCode: core_error.EntityNotFoundErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			reelRepo := inmem.NewReelRepository(tt.args.defaultReels)
			queryRepo := inmem.NewQueryRepository(tt.args.defaultCategories, tt.args.defaultReels)
			bus := InMemEventBus.NewInMemEventBus()
			dep := define.Dependency{
				UnitOfWork: dblocal.NewUnitOfWork(),
				ReelRepo:   reelRepo,
				QueryRepo:  queryRepo,
				EventBus:   bus,
			}
			u := command.NewPublishReelUseCase(dep)

			// act
			got, err := u.Execute(ctx, tt.args.cmd)

			if (err != nil) != tt.wantErr {
				t.Errorf("PublishReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}

			// assert
			assert.Equal(t, tt.want.publishReelResponse, got)
		})
	}
}
