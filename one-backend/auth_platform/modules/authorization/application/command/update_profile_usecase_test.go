package command

import (
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/port/output/inmem"
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
)

const (
	testUser       = "xrspacetest1000"
	wrongUser      = "xrspacetest1001"
	privateKeyPath = "../../../../.ssh/private_key_for_test.pem"
	mockAppID      = "TP_Five_APP_ID"
	mockPooID      = "TP_Five_POOL_ID"
	mockSecret     = "TP_Five_SECRET"
)

type TestUpdateProfileUsecaseTestSuite struct {
	suite.Suite
}

func TestUpdateProfile(t *testing.T) {
	suite.Run(t, new(TestUpdateProfileUsecaseTestSuite))
}

func (s *TestUpdateProfileUsecaseTestSuite) TestUpdateProfileUsecase_Execute() {
	type args struct {
		ctx     context.Context
		command *UpdateProfileCommand
	}

	type want struct {
		response *UpdateProfileResponse
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
				command: &UpdateProfileCommand{
					UserID:      testUser,
					AccessToken: token,
					Username:    "new_username",
					Nickname:    "new_nickname",
				},
			},
			want: want{
				response: &UpdateProfileResponse{
					Username:      "new_username",
					Nickname:      "new_nickname",
					Email:         testUser + "@xrspace.io",
					EmailVerified: true,
				},
			},
		},
		{
			name: "update email success and email_verified became false",
			args: args{
				ctx: context.Background(),
				command: &UpdateProfileCommand{
					UserID:      testUser,
					AccessToken: token,
					Email:       "new_email@xrspace.io",
				},
			},
			want: want{
				response: &UpdateProfileResponse{
					Username:      "new_username",
					Nickname:      "new_nickname",
					Email:         "new_email@xrspace.io",
					EmailVerified: false,
				},
			},
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			uc := NewUpdateProfileUsecase(jwtService, userRepo)

			// act
			got, err := uc.Execute(tt.args.ctx, tt.args.command)

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
			s.Equal(tt.want.response.Username, got.Username)
			s.Equal(tt.want.response.Nickname, got.Nickname)
			s.Equal(tt.want.response.Email, got.Email)
			s.Equal(tt.want.response.EmailVerified, got.EmailVerified)
		})
	}
}
