package command

import (
	"context"
	"testing"

	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/entity"
	"xrspace.io/server/modules/account/domain/proxy"
	"xrspace.io/server/modules/account/port/output/repository/inmem"

	authProxyInmem "xrspace.io/server/modules/account/port/output/auth_proxy/inmem"

	"gopkg.in/stretchr/testify.v1/assert"
)

func TestLoginUseCase_Execute(t *testing.T) {
	type args struct {
		cmd             *LoginCommand
		defaultAccounts map[interface{}]*entity.Account
		defaultProfiles map[string]*proxy.GetProfileResponse
	}
	type want struct {
		wantAccount     *entity.Account
		getAccountTimes int
		getProfileTimes int
	}
	tests := []struct {
		args    args
		name    string
		ErrMsg  string
		want    want
		wantErr bool
	}{
		{
			name: "login_success_if_account_not_exist",
			args: args{
				defaultAccounts: map[interface{}]*entity.Account{},
				defaultProfiles: map[string]*proxy.GetProfileResponse{
					"access_token": {
						Email:           "test_email",
						ResourceOwnerID: "resource_owner_id",
						Username:        "test_username",
						Nickname:        "test_nickname",
						IsEmailVerified: true,
					},
				},
				cmd: &LoginCommand{
					AccessToken:     "access_token",
					Issuer:          "test_issuer",
					ResourceOwnerID: "test_resource_owner_id",
				},
			},
			want: want{
				wantAccount: &entity.Account{
					Username: "test_username",
					Nickname: "test_nickname",
					IssuerResourceOwnerIDs: map[string]string{
						"test_issuer": "test_resource_owner_id",
					},
				},
				getAccountTimes: 1,
				getProfileTimes: 1,
			},
		},
		{
			name: "login_success_if_account_exist",
			args: args{
				defaultAccounts: map[interface{}]*entity.Account{
					"test_xr_id": {
						Username: "test_username",
						Nickname: "test_nickname",
						IssuerResourceOwnerIDs: map[string]string{
							"test_issuer": "test_resource_owner_id",
						},
					},
				},
				cmd: &LoginCommand{
					AccessToken:     "access_token",
					Issuer:          "test_issuer",
					ResourceOwnerID: "test_resource_owner_id",
				},
			},
			want: want{
				wantAccount: &entity.Account{
					Username: "test_username",
					Nickname: "test_nickname",
					IssuerResourceOwnerIDs: map[string]string{
						"test_issuer": "test_resource_owner_id",
					},
				},
				getAccountTimes: 1,
				getProfileTimes: 0,
			},
		},
		{
			name: "login_failed_if_username_is_empty",
			args: args{
				defaultAccounts: map[interface{}]*entity.Account{},
				defaultProfiles: map[string]*proxy.GetProfileResponse{
					"access_token": {
						Email:           "test_email",
						ResourceOwnerID: "resource_owner_id",
						Username:        "",
						Nickname:        "test_nickname",
						IsEmailVerified: true,
					},
				},
				cmd: &LoginCommand{
					AccessToken:     "access_token",
					Issuer:          "test_issuer",
					ResourceOwnerID: "test_resource_owner_id",
				},
			},
			wantErr: true,
			ErrMsg:  "username is empty",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			// use mock instead of inmem
			queryRepo := NewMockAccountRepository(tt.args.defaultAccounts)
			authGateway := NewMockAuthGateway(tt.args.defaultProfiles, nil, nil)
			accountRepo := inmem.NewAccountRepository(tt.args.defaultAccounts)
			dep := define.Dependency{
				AccountRepo: accountRepo,
				AuthProxy:   authGateway,
				QueryRepo:   queryRepo,
			}
			useCase := NewLoginUseCase(dep)

			// act
			_, err := useCase.Execute(ctx, tt.args.cmd)

			// assert
			if (err != nil) != tt.wantErr {
				t.Errorf("LoginUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.ErrMsg)
				return
			}

			assert.Equal(t, tt.want.getAccountTimes, queryRepo.getAccountTimes)
			assert.Equal(t, tt.want.getProfileTimes, authGateway.getProfileTimes)

			account, err := queryRepo.GetAccountByResourceOwnerID(ctx, tt.args.cmd.ResourceOwnerID)
			assert.NoError(t, err)

			assert.Equal(t, tt.want.wantAccount.IssuerResourceOwnerIDs, account.IssuerResourceOwnerIDs)
			assert.Equal(t, tt.want.wantAccount.Username, account.Username)
			assert.Equal(t, tt.want.wantAccount.Nickname, account.Nickname)
		})
	}
}

type MockAccountRepository struct {
	*inmem.QueryRepository
	getAccountTimes int
}

func NewMockAccountRepository(defaultAccounts map[interface{}]*entity.Account) *MockAccountRepository {
	return &MockAccountRepository{
		QueryRepository: inmem.NewQueryRepository(defaultAccounts),
	}
}

func (r *MockAccountRepository) GetAccountByResourceOwnerID(ctx context.Context, resourceOwnerID string) (*entity.Account, error) {
	r.getAccountTimes++
	return r.QueryRepository.GetAccountByResourceOwnerID(ctx, resourceOwnerID)
}

type MockAuthGateway struct {
	*authProxyInmem.AuthProxy
	getProfileTimes int
}

func NewMockAuthGateway(
	defaultProfiles map[string]*proxy.GetProfileResponse,
	defaultUsers map[string]*proxy.CreateUserResponse,
	defaultManagerTokens map[string]*proxy.GetManagerTokenResponse) *MockAuthGateway {
	return &MockAuthGateway{
		AuthProxy: authProxyInmem.NewAuthProxy(defaultProfiles, defaultUsers, defaultManagerTokens),
	}
}

func (g *MockAuthGateway) GetProfile(ctx context.Context, params *proxy.GetProfileParams) (*proxy.GetProfileResponse, error) {
	g.getProfileTimes++
	return g.AuthProxy.GetProfile(ctx, params)
}
