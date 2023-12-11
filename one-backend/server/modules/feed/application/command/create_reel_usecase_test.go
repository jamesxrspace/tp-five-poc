package command

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"xrspace.io/server/core/arch/core_error"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/enum"
	"xrspace.io/server/modules/feed/port/output/repository/inmem"
)

func TestCreateReelUseCase_Execute(t *testing.T) {
	type args struct {
		cmd               *CreateReelCommand
		defaultReels      map[string]*entity.Reel
		defaultCategories map[string]*assetEntity.CategoryItem
	}
	type want struct {
		createReelResponse *CreateReelResponse
		saveReelTimes      int
	}
	tests := []struct {
		args    args
		name    string
		want    want
		errCode string
		wantErr bool
	}{
		{
			name: "success",
			args: args{
				cmd: &CreateReelCommand{
					Description:      "description",
					Thumbnail:        "thumbnail",
					Video:            "video",
					Xrs:              "xrs",
					XrID:             "xrid",
					ParentReelID:     "parent_reel_id",
					Categories:       []string{"music", "culture"},
					JoinMode:         enum.ReelJoinModeMe,
					MusicToMotionUrl: "http://music.com/motion",
				},
				defaultReels: map[string]*entity.Reel{
					"parent_reel_id": {
						ID:         "parent_reel_id",
						Status:     enum.ReelStatusPublished,
						RootReelID: "root_reel_id",
					},
				},
				defaultCategories: map[string]*assetEntity.CategoryItem{
					"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
					"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
				},
			},
			want: want{
				createReelResponse: &CreateReelResponse{
					Reel: &entity.Reel{
						ID:               "reel_id",
						Description:      "description",
						Thumbnail:        "thumbnail",
						Video:            "video",
						Xrs:              "xrs",
						XrID:             "xrid",
						Status:           enum.ReelStatusDraft,
						ParentReelIDs:    []string{"parent_reel_id"},
						RootReelID:       "root_reel_id",
						Categories:       []string{"music", "culture"},
						JoinMode:         enum.ReelJoinModeMe,
						MusicToMotionUrl: "http://music.com/motion",
					},
				},
				saveReelTimes: 1,
			},
		},
		{
			name: "failed_if_parent_reel_is_not_exist_and_status_is_not_published",
			args: args{
				cmd: &CreateReelCommand{
					Description:      "description",
					Thumbnail:        "thumbnail",
					Video:            "video",
					Xrs:              "xrs",
					XrID:             "xrid",
					ParentReelID:     "parent_reel_id",
					Categories:       []string{"music", "culture"},
					JoinMode:         enum.ReelJoinModeMe,
					MusicToMotionUrl: "http://music.com/motion",
				},
				defaultCategories: map[string]*assetEntity.CategoryItem{
					"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
					"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
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
			repo := newMockCreateReelRepository(tt.args.defaultReels)
			queryRepo := inmem.NewQueryRepository(nil, tt.args.defaultCategories, nil, tt.args.defaultReels)
			dep := define.Dependency{ReelRepo: repo, QueryRepo: queryRepo}
			u := NewCreateReelUseCase(dep)

			// act
			got, err := u.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}

			result := got.(*CreateReelResponse)

			// assert
			assert.Equal(t, tt.want.createReelResponse.Reel.Description, result.Reel.Description)
			assert.Equal(t, tt.want.createReelResponse.Reel.Thumbnail, result.Reel.Thumbnail)
			assert.Equal(t, tt.want.createReelResponse.Reel.Video, result.Reel.Video)
			assert.Equal(t, tt.want.createReelResponse.Reel.Xrs, result.Reel.Xrs)
			assert.Equal(t, tt.want.createReelResponse.Reel.XrID, result.Reel.XrID)
			assert.Equal(t, tt.want.createReelResponse.Reel.Status, result.Reel.Status)
			assert.Equal(t, tt.want.createReelResponse.Reel.ParentReelIDs, result.Reel.ParentReelIDs)
			assert.Equal(t, tt.want.createReelResponse.Reel.RootReelID, result.Reel.RootReelID)
			assert.Equal(t, tt.want.saveReelTimes, repo.saveReelTimes)
		})
	}
}

type mockCreateReelRepository struct {
	*inmem.ReelRepository
	saveReelTimes int
}

func newMockCreateReelRepository(defaultReels map[string]*entity.Reel) *mockCreateReelRepository {
	return &mockCreateReelRepository{
		ReelRepository: inmem.NewReelRepository(defaultReels),
	}
}

func (m *mockCreateReelRepository) Save(ctx context.Context, reel *entity.Reel) error {
	m.saveReelTimes++
	return m.ReelRepository.Save(ctx, reel)
}
