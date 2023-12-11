package query

import (
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/port/output/inmem"
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
)

const (
	privateKeyPath = "../../../../.ssh/private_key_for_test.pem"
	mockAppID      = "TP_Five_APP_ID"
	mockPooID      = "TP_Five_POOL_ID"
	mockSecret     = "TP_Five_SECRET"
)

type TestGetTokenUsecaseTestSuite struct {
	suite.Suite
}

func TestGetToken(t *testing.T) {
	suite.Run(t, new(TestGetTokenUsecaseTestSuite))
}

func (s *TestGetTokenUsecaseTestSuite) TestGetTokenUsecase_Execute() {
	type args struct {
		ctx   context.Context
		query *GetTokenQuery
	}

	type want struct {
		response *GetTokenResponse
	}

	tests := []struct {
		args       args
		want       want
		name       string
		wantErrMsg string
		wantErr    bool
	}{
		{
			name: "success",
			args: args{
				ctx: context.Background(),
				query: &GetTokenQuery{
					GrantType: "password",
					Email:     "xrspacetest1000@xrspace.io",
					Password:  "1qazXSW@",
				},
			},
			want: want{
				response: &GetTokenResponse{
					Scope:        "openid email profile phone",
					TokenType:    "Bearer",
					AccessToken:  "token",
					ExpiresIn:    86400,
					IDToken:      "ID_TOKEN",
					RefreshToken: "REFRESH_TOKEN",
				},
			},
		},
		{
			name: "failed with wrong password",
			args: args{
				ctx: context.Background(),
				query: &GetTokenQuery{
					GrantType: "password",
					Email:     "xrspacetest1000@xrspace.io",
					Password:  "1a2b3c4d",
				},
			},
			wantErr:    true,
			wantErrMsg: "invalid email or password",
		},
		{
			name: "failed with not existing user",
			args: args{
				ctx: context.Background(),
				query: &GetTokenQuery{
					GrantType: "password",
					Email:     "xrtest000@xrspace.io",
					Password:  "1qazXSW@",
				},
			},
			wantErr:    true,
			wantErrMsg: "invalid email or password",
		},
		{
			name: "failed without email",
			args: args{
				ctx: context.Background(),
				query: &GetTokenQuery{
					GrantType: "password",
					Password:  "1qazXSW@",
				},
			},
			wantErr:    true,
			wantErrMsg: "'GetTokenQuery.Email' Error:Field validation for 'Email' failed on the 'required' tag",
		},
		{
			name: "failed without password",
			args: args{
				ctx: context.Background(),
				query: &GetTokenQuery{
					GrantType: "password",
					Email:     "xrspacetest1000@xrspace.io",
				},
			},
			wantErr:    true,
			wantErrMsg: "Key: 'GetTokenQuery.Password' Error:Field validation for 'Password' failed on the 'required' tag",
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			jwtService := jwt.NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret)
			userRepo := inmem.NewUserRepository()
			uc := NewGetTokenUsecase(jwtService, userRepo)

			// act
			got, err := uc.Execute(tt.args.ctx, tt.args.query)

			// assert error
			if (err != nil) != tt.wantErr {
				s.T().Errorf("GetTokenUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				s.ErrorContains(err, tt.wantErrMsg)
				return
			}

			// assert
			s.Nil(err)
			s.Equal(tt.want.response.Scope, got.Scope)
			s.Equal(tt.want.response.TokenType, got.TokenType)
			s.Equal(tt.want.response.ExpiresIn, got.ExpiresIn)
			s.Equal(tt.want.response.IDToken, got.IDToken)
			s.NotNil(got.RefreshToken)
		})
	}
}
