package query

import (
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/port/output/inmem"
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
)

const (
	testUser  = "xrspacetest1000"
	wrongUser = "xrspacetest1001"
)

type TestGetProfileUsecaseTestSuite struct {
	suite.Suite
}

func TestGetProfile(t *testing.T) {
	suite.Run(t, new(TestGetProfileUsecaseTestSuite))
}

func (s *TestGetProfileUsecaseTestSuite) TestGetProfileUsecase_Execute() {
	type args struct {
		ctx   context.Context
		query *GetProfileQuery
	}

	type want struct {
		response *GetProfileResponse
	}

	jwtService := jwt.NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret)
	token, _ := jwtService.GenAccessToken(testUser, 60)
	userRepo := inmem.NewUserRepository()
	userRepo.Users[testUser].AccessToken = token

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
				query: &GetProfileQuery{
					UserID:      testUser,
					AccessToken: token,
				},
			},
			want: want{
				response: &GetProfileResponse{
					Username:      testUser,
					Nickname:      testUser + "_nickname",
					Email:         testUser + "@xrspace.io",
					Sub:           testUser,
					EmailVerified: true,
				},
			},
		},
		{
			name: "failed with wrong access token",
			args: args{
				ctx: context.Background(),
				query: &GetProfileQuery{
					UserID:      testUser,
					AccessToken: token + "wrong",
				},
			},
			wantErr:    true,
			wantErrMsg: "invalid token",
		},
		{
			name: "failed with access token not match user id",
			args: args{
				ctx: context.Background(),
				query: &GetProfileQuery{
					UserID:      wrongUser,
					AccessToken: token,
				},
			},
			wantErr:    true,
			wantErrMsg: "invalid token",
		},
		{
			name: "failed without access token",
			args: args{
				ctx: context.Background(),
				query: &GetProfileQuery{
					UserID: testUser,
				},
			},
			wantErr:    true,
			wantErrMsg: "'GetProfileQuery.AccessToken' Error:Field validation for 'AccessToken' failed on the 'required' tag",
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			uc := NewGetProfileUsecase(jwtService, userRepo)

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
			s.Equal(tt.want.response.Sub, got.Sub)
			s.Equal(tt.want.response.Username, got.Username)
			s.Equal(tt.want.response.Nickname, got.Nickname)
			s.Equal(tt.want.response.Email, got.Email)
			s.Equal(tt.want.response.EmailVerified, got.EmailVerified)
		})
	}
}
