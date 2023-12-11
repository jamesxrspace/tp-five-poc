package application_test

import (
	"auth_platform/modules/authorization/application/query"
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/account/application"
	"xrspace.io/server/modules/account/application/command"
	"xrspace.io/server/modules/account/application/define"
)

func TestLoginUseCase_Execute(t *testing.T) {
	type args struct {
		cmd *command.LoginCommand
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		wantErr bool
	}{
		{
			name: "login_failed_if_validate_error",
			args: args{
				cmd: &command.LoginCommand{
					AccessToken:     "",
					Issuer:          "",
					ResourceOwnerID: "",
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
			f := application.NewFacade(define.Dependency{})

			// act
			_, err := f.Execute(ctx, tt.args.cmd)

			// assert
			if (err != nil) != tt.wantErr {
				t.Errorf("LoginUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestCreateGuestUseCase_Execute(t *testing.T) {
	type args struct {
		cmd *command.CreateGuestCommand
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		wantErr bool
	}{
		{
			name: "create_guest_fail_without_nickname",
			args: args{
				cmd: &command.CreateGuestCommand{},
			},
			wantErr: true,
			errCode: core_error.ValidationErrCode,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			f := application.NewFacade(define.Dependency{})

			// act
			_, err := f.Execute(ctx, tt.args.cmd)

			// assert
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateGuestUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}

func TestGetProfileUseCase_Execute(t *testing.T) {
	type args struct {
		query *query.GetProfileQuery
	}
	tests := []struct {
		args    args
		name    string
		errCode string
		wantErr bool
	}{
		{
			name: "failed_if_query_is_invalid",
			args: args{
				query: &query.GetProfileQuery{
					AccessToken: "",
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
			f := application.NewFacade(define.Dependency{})

			// act
			_, err := f.Execute(ctx, tt.args.query)

			// assert
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateGuestUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.(*core_error.CodeError).ErrorCode, tt.errCode)
				return
			}
		})
	}
}
