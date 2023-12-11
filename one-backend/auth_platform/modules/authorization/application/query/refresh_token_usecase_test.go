package query

import (
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/port/output/inmem"
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
)

type TestRefreshTokenUsecaseTestSuite struct {
	suite.Suite
}

func TestRefreshToken(t *testing.T) {
	suite.Run(t, new(TestRefreshTokenUsecaseTestSuite))
}

func (s *TestRefreshTokenUsecaseTestSuite) TestRefreshTokenUsecase_Execute() {
	type args struct {
		ctx   context.Context
		query *RefreshTokenQuery
	}

	type want struct {
		response *RefreshTokenResponse
	}

	jwtService := jwt.NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret)
	userRepo := inmem.NewUserRepository()
	uc := NewGetTokenUsecase(jwtService, userRepo)
	resp, _ := uc.Execute(context.Background(), &GetTokenQuery{
		GrantType: "password",
		Email:     "xrspacetest1000@xrspace.io",
		Password:  "1qazXSW@",
	})
	token := resp.RefreshToken

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
				query: &RefreshTokenQuery{
					GrantType:    "refresh_token",
					ClientId:     "string",
					RefreshToken: token,
				},
			},
			want: want{
				response: &RefreshTokenResponse{
					Scope:        "openid email profile phone",
					TokenType:    "Bearer",
					AccessToken:  "ACCESS_TOKEN",
					ExpiresIn:    86400,
					IDToken:      "ID_TOKEN",
					RefreshToken: "REFRESH_TOKEN",
				},
			},
		},
		{
			name: "failed with wrong refresh token",
			args: args{
				ctx: context.Background(),
				query: &RefreshTokenQuery{
					GrantType:    "refresh_token",
					ClientId:     "string",
					RefreshToken: "wrong_token",
				},
			},
			wantErr:    true,
			wantErrMsg: "invalid token",
		},
		{
			name: "failed without grant type",
			args: args{
				ctx: context.Background(),
				query: &RefreshTokenQuery{
					ClientId:     "string",
					RefreshToken: token,
				},
			},
			wantErr:    true,
			wantErrMsg: "'RefreshTokenQuery.GrantType' Error:Field validation for 'GrantType' failed on the 'required' tag",
		},
		{
			name: "failed without client id",
			args: args{
				ctx: context.Background(),
				query: &RefreshTokenQuery{
					GrantType:    "refresh_token",
					RefreshToken: token,
				},
			},
			wantErr:    true,
			wantErrMsg: "'RefreshTokenQuery.ClientId' Error:Field validation for 'ClientId' failed on the 'required' tag",
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			uc := NewRefreshTokenUsecase(jwtService, userRepo)

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
			s.NotNil(got.AccessToken)
			s.NotNil(got.RefreshToken)
		})
	}
}
