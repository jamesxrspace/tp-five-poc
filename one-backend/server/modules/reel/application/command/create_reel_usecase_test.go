package command

import (
	"context"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/domain/entity"
	"xrspace.io/server/modules/reel/domain/enum"
	"xrspace.io/server/modules/reel/domain/repository"
	"xrspace.io/server/modules/reel/port/output/repository/inmem"
	reelMongo "xrspace.io/server/modules/reel/port/output/repository/mongo"
)

func TestCreateReelUseCaseExecute(t *testing.T) {
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
			queryRepo := inmem.NewQueryRepository(tt.args.defaultCategories, tt.args.defaultReels)
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

func testReels() map[string]*entity.Reel {
	return map[string]*entity.Reel{
		"reel1": {
			ID:               "parent_reel_id",
			XrID:             "xrid1",
			Description:      "description1",
			Thumbnail:        "thumbnail1",
			Video:            "video1",
			Xrs:              "xrs1",
			Categories:       []string{"music", "culture"},
			JoinMode:         enum.ReelJoinModeMe,
			MusicToMotionUrl: "http://music.com/motion",
			Status:           enum.ReelStatusPublished,
			RootReelID:       "root_reel_id_1",
			ParentReelIDs:    []string{},
			UpdatedAt:        time.Date(2023, time.November, 29, 9, 0, 0, 0, time.UTC),
		},
		"reel2": {
			ID:               "reel2",
			XrID:             "xrid2",
			Description:      "description2",
			Thumbnail:        "thumbnail2",
			Video:            "video2",
			Xrs:              "xrs2",
			Categories:       []string{"music"},
			JoinMode:         enum.ReelJoinModeMe,
			MusicToMotionUrl: "http://music.com/motion",
			Status:           enum.ReelStatusDraft,
			RootReelID:       "root_reel_id_2",
			ParentReelIDs:    []string{"parent_reel_id_1", "parent_reel_id_2"},
			UpdatedAt:        time.Date(2023, time.November, 29, 10, 0, 0, 0, time.UTC),
		},
		"reel3": {
			ID:               "reel3",
			XrID:             "xrid3",
			Description:      "description3",
			Thumbnail:        "thumbnail3",
			Video:            "video3",
			Xrs:              "xrs3",
			Categories:       []string{"culture"},
			JoinMode:         enum.ReelJoinModeMe,
			MusicToMotionUrl: "http://music.com/motion",
			Status:           enum.ReelStatusDraft,
			RootReelID:       "root_reel_id_3",
			ParentReelIDs:    []string{"parent_reel_id_3", "parent_reel_id_4"},
			UpdatedAt:        time.Date(2023, time.November, 29, 11, 0, 0, 0, time.UTC),
		},
	}
}

func testCategories() map[string]*assetEntity.CategoryItem {
	return map[string]*assetEntity.CategoryItem{
		"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
		"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
	}
}

func TestCreateReelUseCase(t *testing.T) {
	suite.Run(t, new(CreateReelTestSuite))
}

type CreateReelTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
	reelRepo     repository.IReelRepository
	categoryRepo *mongo.GenericMongoRepository[*assetEntity.CategoryItem]
	queryRepo    define.IQueryRepository
}

func (s *CreateReelTestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	s.reelRepo = reelMongo.NewReelRepository(s.DbDoc)
	s.categoryRepo = mongo.NewGenericMongoRepository[*assetEntity.CategoryItem](s.DbDoc.Collection("category"))

	categories := testCategories()
	for _, category := range categories {
		s.categoryRepo.Save(context.Background(), category)
	}

	reels := testReels()
	for _, reel := range reels {
		s.reelRepo.Save(context.Background(), reel)
	}

	s.queryRepo = reelMongo.NewQueryRepository(s.DbDoc)
}

func (s *CreateReelTestSuite) TestCreateReelUseCaseExecute() {
	type args struct {
		cmd *CreateReelCommand
	}
	type want struct {
		createReelResponse *CreateReelResponse
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
						RootReelID:       "root_reel_id_1",
						Categories:       []string{"music", "culture"},
						JoinMode:         enum.ReelJoinModeMe,
						MusicToMotionUrl: "http://music.com/motion",
					},
				},
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
					ParentReelID:     "parent_reel_id_5",
					Categories:       []string{"music", "culture"},
					JoinMode:         enum.ReelJoinModeMe,
					MusicToMotionUrl: "http://music.com/motion",
				},
			},
			wantErr: true,
			errCode: core_error.EntityNotFoundErrCode,
		},
	}
	for _, tt := range tests {
		s.T().Run(tt.name, func(t *testing.T) {
			// arrange
			dep := define.Dependency{ReelRepo: s.reelRepo, QueryRepo: s.queryRepo}
			u := NewCreateReelUseCase(dep)

			// act
			got, err := u.Execute(context.Background(), tt.args.cmd)
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
		})
	}
}
