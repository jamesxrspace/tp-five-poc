package application_test

import (
	"context"
	"testing"

	"github.com/alecthomas/assert/v2"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/reaction/application"
	"xrspace.io/server/modules/reaction/application/command"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/application/query"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/port/output/repository/inmem"
)

func TestLikeUseCaseExecute(t *testing.T) {
	type args struct {
		cmd          *command.LikeCommand
		defaultLikes map[interface{}]*entity.Like
	}
	tests := []struct {
		name    string
		args    args
		wantErr bool
		errCode string
	}{
		{
			name: "failed_without_target",
			args: args{
				cmd: &command.LikeCommand{
					XrID:       "xrid_1",
					TargetType: "feed",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_without_xrid",
			args: args{
				cmd: &command.LikeCommand{
					TargetID:   "feed_id1",
					TargetType: "feed",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_without_type",
			args: args{
				cmd: &command.LikeCommand{
					XrID:     "xrid_1",
					TargetID: "feed_id1",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			dep := define.Dependency{
				LikeRepo: inmem.NewLikeRepository(tt.args.defaultLikes),
			}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("LikeUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			// assert
			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestGetLikeUseCaseExecute(t *testing.T) {
	type args struct {
		query        *query.GetLikeQuery
		defaultLikes map[interface{}]*entity.Like
	}
	tests := []struct {
		name    string
		args    args
		wantErr bool
		errCode string
	}{
		{
			name: "failed_without_target",
			args: args{
				query: &query.GetLikeQuery{
					XrID: "xrid_1",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
		{
			name: "failed_without_xrid",
			args: args{
				query: &query.GetLikeQuery{
					TargetID: "feed_id1",
				},
				defaultLikes: map[interface{}]*entity.Like{},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			dep := define.Dependency{
				QueryRepo: inmem.NewQueryRepository(tt.args.defaultLikes),
			}
			f := application.NewFacade(dep)

			// act
			_, err := f.Execute(ctx, tt.args.query)
			if (err != nil) != tt.wantErr {
				t.Errorf("GetLikeUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			// assert
			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}
