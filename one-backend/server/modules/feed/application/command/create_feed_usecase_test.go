package command

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/enum"
	"xrspace.io/server/modules/feed/port/output/repository/inmem"
)

func TestCreateFeedUseCase_Execute(t *testing.T) {
	type args struct {
		cmd               *CreateFeedCommand
		defaultFeeds      map[string]*entity.Feed
		defaultCategories map[string]*assetEntity.CategoryItem
	}
	type want struct {
		createFeedResponse *CreateFeedResponse
		feedCount          int
	}
	tests := []struct {
		args    args
		name    string
		want    want
		errMsg  string
		wantErr bool
	}{
		{
			name: "success_create_reel_feed",
			args: args{
				cmd: &CreateFeedCommand{
					XrID:         "xrid",
					Type:         enum.FeedTypeReel,
					RefID:        "ref_id",
					Categories:   []string{"music", "culture"},
					ThumbnailUrl: "thumbnail_url",
				},
				defaultFeeds: nil,
				defaultCategories: map[string]*assetEntity.CategoryItem{
					"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
					"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
				},
			},
			want: want{
				createFeedResponse: &CreateFeedResponse{
					Feed: &entity.Feed{
						XrID:         "xrid",
						RefID:        "ref_id",
						Type:         enum.FeedTypeReel,
						Categories:   []string{"music", "culture"},
						ThumbnailUrl: "thumbnail_url",
						Status:       enum.FeedStatusActive,
					},
				},
				feedCount: 1,
			},
			wantErr: false,
		},
		{
			name: "failed_when_feed_with_reference_content_already_exists",
			args: args{
				cmd: &CreateFeedCommand{
					XrID:         "xrid",
					Type:         enum.FeedTypeReel,
					RefID:        "ref_id",
					Categories:   []string{"music", "dance"},
					ThumbnailUrl: "thumbnail_url",
				},
				defaultFeeds: map[string]*entity.Feed{
					"feed_id": {
						ID:           "feed_id",
						XrID:         "xrid",
						Type:         enum.FeedTypeReel,
						RefID:        "ref_id",
						Categories:   []string{"music", "dance"},
						ThumbnailUrl: "thumbnail_url",
						Status:       enum.FeedStatusActive,
					},
				},
				defaultCategories: map[string]*assetEntity.CategoryItem{
					"music":   {ID: "music", Type: assetEnum.CateFeed, TitleI18n: "music"},
					"culture": {ID: "culture", Type: assetEnum.CateFeed, TitleI18n: "culture"},
				},
			},
			wantErr: true,
			errMsg:  "feed already exists",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			feedRepo := inmem.NewFeedRepository(tt.args.defaultFeeds)
			queryRepo := inmem.NewQueryRepository(nil, tt.args.defaultCategories, tt.args.defaultFeeds, nil)
			dep := define.Dependency{FeedRepo: feedRepo, QueryRepo: queryRepo}
			u := NewCreateFeedUseCase(dep)

			// act
			got, err := u.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateFeedUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}
			result := got.(*CreateFeedResponse)

			// assert
			assert.Equal(t, tt.want.createFeedResponse.Feed.RefID, result.Feed.RefID)
			assert.Equal(t, tt.want.createFeedResponse.Feed.Type, result.Feed.Type)
			assert.Equal(t, tt.want.createFeedResponse.Feed.Status, result.Feed.Status)
			assert.Equal(t, tt.want.createFeedResponse.Feed.Categories, result.Feed.Categories)
			assert.Equal(t, tt.want.createFeedResponse.Feed.ThumbnailUrl, result.Feed.ThumbnailUrl)
			assert.Equal(t, tt.want.feedCount, len(feedRepo.Feeds))
		})
	}
}
