package query

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

func TestGetProfileUseCase_Execute(t *testing.T) {
	type args struct {
		query           *GetProfileQuery
		defaultAccounts map[interface{}]*entity.Account
		defaultProfiles map[string]*proxy.GetProfileResponse
	}
	tests := []struct {
		args    args
		want    *GetProfileResponse
		name    string
		ErrMsg  string
		wantErr bool
	}{
		{
			name: "success",
			args: args{
				query: &GetProfileQuery{
					AccessToken:     "test_access_token",
					ResourceOwnerID: "test_resource_owner_id",
				},
				defaultAccounts: map[interface{}]*entity.Account{
					"test_xrid": {
						XrID:     "test_xrid",
						Username: "test_username",
						Nickname: "test_nickname",
						IssuerResourceOwnerIDs: map[string]string{
							"test_access_token": "test_resource_owner_id",
						},
					},
				},
				defaultProfiles: map[string]*proxy.GetProfileResponse{
					"test_access_token": {
						Email:           "test_email",
						ResourceOwnerID: "test_resource_owner_id",
						Username:        "test_username",
						Nickname:        "test_nickname",
						IsEmailVerified: true,
					},
				},
			},
			want: &GetProfileResponse{
				Email: "test_email",
				XrID:  "test_xrid",
				IssuerResourceOwnerIDs: map[string]string{
					"test_access_token": "test_resource_owner_id",
				},
				Username:        "test_username",
				Nickname:        "test_nickname",
				IsEmailVerified: true,
			},
		},
		{
			name: "failed_if_account_not_exist",
			args: args{
				query: &GetProfileQuery{
					ResourceOwnerID: "test_resource_owner_id",
					AccessToken:     "test_access_token",
				},
			},
			wantErr: true,
			ErrMsg:  "account not found",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			accountRepo := inmem.NewAccountRepository(map[interface{}]*entity.Account{})
			queryRepo := inmem.NewQueryRepository(tt.args.defaultAccounts)
			authGateway := authProxyInmem.NewAuthProxy(tt.args.defaultProfiles, nil, nil)
			dep := define.Dependency{
				AccountRepo: accountRepo,
				AuthProxy:   authGateway,
				QueryRepo:   queryRepo,
			}
			u := NewGetProfileUseCase(dep)

			// act
			got, err := u.Execute(ctx, tt.args.query)

			// assert
			if (err != nil) != tt.wantErr {
				t.Errorf("GetProfileUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.ErrMsg)
				return
			}

			result := got.(*GetProfileResponse)

			assert.Equal(t, tt.want.Email, result.Email)
			assert.Equal(t, tt.want.XrID, result.XrID)
			assert.Equal(t, tt.want.IssuerResourceOwnerIDs, result.IssuerResourceOwnerIDs)
			assert.Equal(t, tt.want.Username, result.Username)
			assert.Equal(t, tt.want.Nickname, result.Nickname)
			assert.Equal(t, tt.want.IsEmailVerified, result.IsEmailVerified)
		})
	}
}
