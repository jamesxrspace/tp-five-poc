package command

import (
	"context"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/core/dependency/database"
	"xrspace.io/server/core/dependency/settings"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/proxy"
	authProxyInmem "xrspace.io/server/modules/account/port/output/auth_proxy/inmem"
)

const (
	testNickname         = "test_nickname"
	testUserID           = "test_user_id"
	testUsername         = "test_username"
	testUserPoolID       = "test_user_pool_id"
	testManagerToken     = "test_manager_token"
	testGuestEmailPrefix = "test_email_prefix"
	testGuestEmailSuffix = "test_email_suffix"
	testGuestCompany     = "test_company"
)

func TestCreateGuestUsecase_Execute(t *testing.T) {
	type args struct {
		cmd                  *CreateGuestCommand
		defaultUsers         map[string]*proxy.CreateUserResponse
		defalutManagerTokens map[string]*proxy.GetManagerTokenResponse
	}
	type want struct {
		response *CreateGuestResponse
	}
	tests := []struct {
		args    args
		want    want
		name    string
		ErrMsg  string
		wantErr bool
	}{
		{
			name: "create_guest_success",
			args: args{
				cmd: &CreateGuestCommand{
					Nickname: testNickname,
				},
				defaultUsers: map[string]*proxy.CreateUserResponse{
					testNickname: {
						UserID:   testUserID,
						Username: testUsername,
					},
				},
				defalutManagerTokens: map[string]*proxy.GetManagerTokenResponse{
					testUserPoolID: {
						AccessToken: testManagerToken,
						ExpiresIn:   7200,
					},
				},
			},
			want: want{
				response: &CreateGuestResponse{
					UserID:   testUserID,
					Username: testUsername,
					Nickname: testNickname,
				},
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			config := &settings.Config{
				OIDC: settings.OIDCConfig{
					PoolID: testUserPoolID,
					Guest: settings.Guest{
						EmailPrefix: testGuestEmailPrefix,
						EmailSuffix: testGuestEmailSuffix,
						Company:     testGuestCompany,
					},
					HttpTimeout: 30,
				},
			}

			inmemRedis := database.NewInmemCacheDB()
			authProxy := authProxyInmem.NewAuthProxy(nil, tt.args.defaultUsers, tt.args.defalutManagerTokens)
			dep := define.Dependency{
				Config:      config,
				RedisClient: inmemRedis,
				AuthProxy:   authProxy,
			}
			usecase := NewCreateGuestUseCase(dep)

			// act
			got, err := usecase.Execute(ctx, tt.args.cmd)

			if (err != nil) != tt.wantErr {
				t.Errorf("CreateGuestUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.ErrMsg)
				return
			}

			result := got.(*CreateGuestResponse)

			assert.Equal(t, tt.want.response.Nickname, result.Nickname)
			assert.Equal(t, tt.want.response.UserID, result.UserID)
			assert.Equal(t, tt.want.response.Username, result.Username)
		})
	}
}
