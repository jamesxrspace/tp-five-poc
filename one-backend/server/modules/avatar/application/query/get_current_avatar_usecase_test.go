package query

import (
	"context"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/value_object"
	"xrspace.io/server/modules/avatar/port/output/repository/inmem"
)

func TestGetCurrentAvatarUseCase_Execute(t *testing.T) {
	type args struct {
		q                    *GetCurrentAvatarQuery
		defaultAvatarPlayers map[value_object.XrID]*entity.AvatarPlayer
		defaultAvatars       map[value_object.AvatarID]*entity.Avatar
	}
	tests := []struct {
		name    string
		args    args
		want    *GetCurrentAvatarResponse
		wantErr bool
		errMsg  string
	}{
		{
			name: "success",
			args: args{
				q: &GetCurrentAvatarQuery{
					XrID:  "test_xrid",
					AppID: "test_app_id",
				},
				defaultAvatarPlayers: map[value_object.XrID]*entity.AvatarPlayer{
					"test_xrid": entity.NewAvatarPlayer(&entity.AvatarPlayerParams{
						XrID: "test_xrid",
						AvatarID: map[value_object.AppID]value_object.AvatarID{
							"test_app_id": "test_avatar_id",
						},
					}),
				},
				defaultAvatars: map[value_object.AvatarID]*entity.Avatar{
					"test_avatar_id": entity.NewAvatar(&entity.AvatarParams{
						AvatarID: "test_avatar_id",
						XrID:     "test_xrid",
						AppID:    "test_app_id",
					}),
				},
			},
			want: &GetCurrentAvatarResponse{
				Avatar: entity.NewAvatar(&entity.AvatarParams{
					AvatarID: "test_avatar_id",
				}),
			},
		},
		{
			name: "failed_if_player_not_found",
			args: args{
				q: &GetCurrentAvatarQuery{
					XrID:  "test_xrid",
					AppID: "test_app_id",
				},
				defaultAvatarPlayers: map[value_object.XrID]*entity.AvatarPlayer{},
				defaultAvatars:       map[value_object.AvatarID]*entity.Avatar{},
			},
			wantErr: true,
			errMsg:  "player does not exist for xrid",
		},
		{
			name: "failed_if_player_does_not_have_avatar_for_this_app",
			args: args{
				q: &GetCurrentAvatarQuery{
					XrID:  "test_xrid",
					AppID: "test_app_id",
				},
				defaultAvatarPlayers: map[value_object.XrID]*entity.AvatarPlayer{
					"test_xrid": entity.NewAvatarPlayer(&entity.AvatarPlayerParams{
						XrID: "test_xrid",
						AvatarID: map[value_object.AppID]value_object.AvatarID{
							"test_app_id_2": "test_avatar_id",
						},
					}),
				},
				defaultAvatars: map[value_object.AvatarID]*entity.Avatar{
					"test_avatar_id": entity.NewAvatar(&entity.AvatarParams{
						AvatarID: "test_avatar_id",
						XrID:     "test_xrid",
						AppID:    "test_app_id",
					}),
				},
			},
			wantErr: true,
			errMsg:  "player does not have avatar for this app",
		},
		{
			name: "failed_if_player_should_set_avatar_first",
			args: args{
				q: &GetCurrentAvatarQuery{
					XrID:  "test_xrid",
					AppID: "test_app_id",
				},
				defaultAvatarPlayers: map[value_object.XrID]*entity.AvatarPlayer{
					"test_xrid": entity.NewAvatarPlayer(&entity.AvatarPlayerParams{
						XrID: "test_xrid",
						AvatarID: map[value_object.AppID]value_object.AvatarID{
							"test_app_id": "",
						},
					}),
				},
			},
			wantErr: true,
			errMsg:  "player should set avatar first",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := inmem.NewAvatarRepository(
				tt.args.defaultAvatars,
				tt.args.defaultAvatarPlayers,
			)
			u := NewGetCurrentAvatarUseCase(
				repo,
			)

			// act
			got, err := u.Execute(ctx, tt.args.q)
			if (err != nil) != tt.wantErr {
				t.Errorf("GetCurrentAvatarUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			// assert
			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			assert.Equal(t, tt.want.Avatar.AvatarID, got.Avatar.AvatarID)
		})
	}
}
