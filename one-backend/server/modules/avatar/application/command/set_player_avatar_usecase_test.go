package command

import (
	"context"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/value_object"
	"xrspace.io/server/modules/avatar/port/output/repository/inmem"
)

const (
	spaPlayer    = "player"
	spaPlayer2   = "player2"
	spaAppID     = "party_on_app_id"
	spaAvatarID  = "abcde"
	spaAvatarID2 = "abcdefg"
)

func TestSetPlayerAvatarUseCase_Execute(t *testing.T) {
	type args struct {
		ctx context.Context
		cmd *SetPlayerAvatarCommand
	}
	type want struct {
		avatarID value_object.AvatarID
	}
	tests := []struct {
		args    args
		name    string
		want    want
		errMsg  string
		wantErr bool
	}{
		{
			name: "set player avatar",
			args: args{
				ctx: context.Background(),
				cmd: &SetPlayerAvatarCommand{
					XrID:     spaPlayer,
					AppID:    value_object.AppID(spaAppID),
					AvatarID: value_object.AvatarID(spaAvatarID),
				},
			},
			want: want{
				avatarID: value_object.AvatarID(spaAvatarID),
			},
			wantErr: false,
			errMsg:  "",
		},
		{
			name: "success_if_xrid_not_exist",
			args: args{
				ctx: context.Background(),
				cmd: &SetPlayerAvatarCommand{
					XrID:     spaPlayer2,
					AppID:    value_object.AppID(spaAppID),
					AvatarID: value_object.AvatarID(spaAvatarID2),
				},
			},
			want: want{
				avatarID: value_object.AvatarID(spaAvatarID2),
			},
		},
		{
			name: "failed without xrid",
			args: args{
				ctx: context.Background(),
				cmd: &SetPlayerAvatarCommand{
					AppID:    value_object.AppID(spaAppID),
					AvatarID: value_object.AvatarID(spaAvatarID),
				},
			},
			wantErr: true,
			errMsg:  "Error:Field validation for 'XrID'",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := inmem.NewAvatarRepository(
				preparedAvatars(),
				preparedAvatarPlayer(),
			)

			useCase := NewSetPlayerAvatarUseCase(repo)

			// act
			_, err := useCase.Execute(ctx, tt.args.cmd)

			// assert
			if (err != nil) != tt.wantErr {
				t.Errorf("SetPlayerAvatarUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			avatarPlayer, _ := repo.GetAvatarPlayer(ctx, tt.args.cmd.XrID)
			assert.Equal(t, tt.want.avatarID, avatarPlayer.AvatarID[tt.args.cmd.AppID])
		})
	}
}

func preparedAvatars() map[value_object.AvatarID]*entity.Avatar {
	return map[value_object.AvatarID]*entity.Avatar{
		spaAvatarID: {
			AvatarID: spaAvatarID,
			AppID:    spaAppID,
			Type:     value_object.AvatarType("xr_v2"),
			XrID:     spaPlayer,
			Thumbnail: &entity.AvatarThumbnail{
				Head:      "https://xrspace-dev.oss-cn-shenzhen.aliyuncs.com/avatar/2CC18/4F877989/xr_v2/fbx/head.png",
				UpperBody: "https://xrspace-dev.oss-cn-shenzhen.aliyuncs.com/avatar/2CC18/4F877989/xr_v2/fbx/body.png",
				FullBody:  "https://xrspace-dev.oss-cn-shenzhen.aliyuncs.com/avatar/2CC18/4F877989/xr_v2/fbx/leg.png",
			},
		},
		spaAvatarID2: {
			AvatarID: spaAvatarID2,
			AppID:    spaAppID,
			Type:     value_object.AvatarType("xr_v2"),
			XrID:     spaPlayer2,
			Thumbnail: &entity.AvatarThumbnail{
				Head:      "https://xrspace-dev.oss-cn-shenzhen.aliyuncs.com/avatar/2CC18/4F877989/xr_v2/fbx/head.png",
				UpperBody: "https://xrspace-dev.oss-cn-shenzhen.aliyuncs.com/avatar/2CC18/4F877989/xr_v2/fbx/body.png",
				FullBody:  "https://xrspace-dev.oss-cn-shenzhen.aliyuncs.com/avatar/2CC18/4F877989/xr_v2/fbx/leg.png",
			},
		},
	}
}

func preparedAvatarPlayer() map[value_object.XrID]*entity.AvatarPlayer {
	return map[value_object.XrID]*entity.AvatarPlayer{
		spaPlayer: entity.NewAvatarPlayer(
			&entity.AvatarPlayerParams{
				XrID: spaPlayer,
				AvatarID: map[value_object.AppID]value_object.AvatarID{
					spaAppID: spaAvatarID,
				},
			},
		),
	}
}
