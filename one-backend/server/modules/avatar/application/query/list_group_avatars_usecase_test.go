package query

import (
	"context"
	"strconv"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/value_object"
	"xrspace.io/server/modules/avatar/port/output/repository/inmem"
)

const (
	lgaXrid1        = "test_xrid_1"
	lgaXrid2        = "test_xrid_2"
	lgaXrid3        = "test_xrid_3"
	lgaXridNoAvatar = "test_xrid_no_avatar"

	lgaAvatarID1 = "avatar1"
	lgaAvatarID2 = "avatar2"
	lgaAvatarID3 = "avatar3"
	lgaAvatarID4 = "avatar4"

	lgaPartyOnAppID = "party_on_app_id"

	lgaDefaultAvatarID = "default_avatar_id"
)

func TestListGroupAvatarsUseCase_Execute(t *testing.T) {
	type args struct {
		q *ListGroupAvatarsQuery
	}
	tests := []struct {
		args    args
		want    *ListGroupAvatarsResponse
		name    string
		errMsg  string
		wantErr bool
	}{
		{
			name: "Success_with_two_exist_avatars",
			args: args{
				q: &ListGroupAvatarsQuery{
					XrIDs: []value_object.XrID{lgaXrid1, lgaXrid2},
					AppID: lgaPartyOnAppID,
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   50,
					},
				},
			},
			want: &ListGroupAvatarsResponse{
				Items: []*entity.Avatar{
					{
						XrID:     lgaXrid1,
						AvatarID: lgaAvatarID1,
						AppID:    lgaPartyOnAppID,
					},
					{
						XrID:     lgaXrid2,
						AvatarID: lgaAvatarID2,
						AppID:    lgaPartyOnAppID,
					},
				},
			},
		},
		{
			name: "Success_if_avatar_not_exist_return_default_avatar",
			args: args{
				q: &ListGroupAvatarsQuery{
					XrIDs: []value_object.XrID{lgaXrid1, lgaXridNoAvatar},
					AppID: lgaPartyOnAppID,
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   50,
					},
				},
			},
			want: &ListGroupAvatarsResponse{
				Items: []*entity.Avatar{
					{
						XrID:     lgaXrid1,
						AvatarID: lgaAvatarID1,
						AppID:    lgaPartyOnAppID,
					},
					{
						XrID:     lgaXridNoAvatar,
						AvatarID: lgaDefaultAvatarID,
						AppID:    lgaPartyOnAppID,
					},
				},
			},
		},
		{
			name: "Failed_if_xrID_exceed_50",
			args: args{
				q: &ListGroupAvatarsQuery{
					XrIDs: fakeXrIDs(),
					AppID: lgaPartyOnAppID,
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   50,
					},
				},
			},
			wantErr: true,
			errMsg:  "'XrIDs' failed on the 'max'",
		},
		{
			name: "Failed_if_xrids_is_not_unique",
			args: args{
				q: &ListGroupAvatarsQuery{
					XrIDs: []value_object.XrID{lgaXrid1, lgaXrid1},
					AppID: lgaPartyOnAppID,
					PaginationQuery: pagination.PaginationQuery{
						Offset: 0,
						Size:   50,
					},
				},
			},
			wantErr: true,
			errMsg:  "'XrIDs' failed on the 'unique'",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repository := inmem.NewAvatarRepository(
				preparedAvatarsForListGroup(),
				preparedAvatarPlayerForXrID(),
			)

			useCase := NewListGroupAvatarsUseCase(repository)

			// act
			got, err := useCase.Execute(ctx, tt.args.q)
			if (err != nil) != tt.wantErr {
				t.Errorf("ListGroupAvatarsUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			// assert
			assert.Equal(t, len(tt.want.Items), len(got.Items))

			if len(tt.want.Items) != len(got.Items) {
				return
			}

			for i := range tt.want.Items {
				assert.Equal(t, tt.want.Items[i].XrID, got.Items[i].XrID)
				assert.Equal(t, tt.want.Items[i].AvatarID, got.Items[i].AvatarID)
				assert.Equal(t, tt.want.Items[i].AppID, got.Items[i].AppID)
			}
		})
	}
}

func fakeXrIDs() []value_object.XrID {
	xrids := []value_object.XrID{}
	for i := 0; i < 51; i++ {
		xrids = append(xrids, value_object.XrID("test_xrid_"+strconv.Itoa(i)))
	}
	return xrids
}

func preparedAvatarsForListGroup() map[value_object.AvatarID]*entity.Avatar {
	return map[value_object.AvatarID]*entity.Avatar{
		lgaAvatarID1: entity.NewAvatar(&entity.AvatarParams{
			AvatarID:     lgaAvatarID1,
			XrID:         lgaXrid1,
			AppID:        lgaPartyOnAppID,
			Type:         "type",
			AvatarFormat: map[string]any{"test": "test"},
			AvatarUrl:    "http://test.com/avatar_url.zip",
			Thumbnail: &entity.AvatarThumbnail{
				Head:      "head",
				UpperBody: "upper_body",
				FullBody:  "full_body",
			},
		}),
		lgaAvatarID2: entity.NewAvatar(&entity.AvatarParams{
			AvatarID:     lgaAvatarID2,
			XrID:         lgaXrid2,
			AppID:        lgaPartyOnAppID,
			Type:         "type",
			AvatarFormat: map[string]any{"test": "test"},
			AvatarUrl:    "http://test.com/avatar_url.zip",
			Thumbnail: &entity.AvatarThumbnail{
				Head:      "head",
				UpperBody: "upper_body",
				FullBody:  "full_body",
			},
		}),
		lgaAvatarID3: entity.NewAvatar(&entity.AvatarParams{
			AvatarID:     lgaAvatarID3,
			XrID:         lgaXrid3,
			AppID:        lgaPartyOnAppID,
			Type:         "type",
			AvatarFormat: map[string]any{"test": "test"},
			AvatarUrl:    "http://test.com/avatar_url.zip",
			Thumbnail: &entity.AvatarThumbnail{
				Head:      "head",
				UpperBody: "upper_body",
				FullBody:  "full_body",
			},
		}),
		lgaAvatarID4: entity.NewAvatar(&entity.AvatarParams{
			AvatarID:     lgaAvatarID4,
			XrID:         lgaXrid1,
			AppID:        lgaPartyOnAppID,
			Type:         "type",
			AvatarFormat: map[string]any{"test": "test"},
			AvatarUrl:    "http://test.com/avatar_url.zip",
			Thumbnail: &entity.AvatarThumbnail{
				Head:      "head",
				UpperBody: "upper_body",
				FullBody:  "full_body",
			},
		}),
	}
}

func preparedAvatarPlayerForXrID() map[value_object.XrID]*entity.AvatarPlayer {
	return map[value_object.XrID]*entity.AvatarPlayer{
		lgaXrid1: {
			XrID: lgaXrid1,
			AvatarID: map[value_object.AppID]value_object.AvatarID{
				lgaPartyOnAppID: lgaAvatarID1,
			},
		},
		lgaXrid2: {
			XrID: lgaXrid2,
			AvatarID: map[value_object.AppID]value_object.AvatarID{
				lgaPartyOnAppID: lgaAvatarID2,
			},
		},
	}
}
