package command

import (
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/port/output/inmem"
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
)

const (
	newUser = "xrspacetest2000"
)

type TestCreateUserUsecaseTestSuite struct {
	suite.Suite
}

func TestCreateUser(t *testing.T) {
	suite.Run(t, new(TestCreateUserUsecaseTestSuite))
}

func (s *TestCreateUserUsecaseTestSuite) TestCreateUserUsecase_Execute() {
	type args struct {
		ctx     context.Context
		command *CreateUserCommand
	}

	type want struct {
		response *CreateUserResponse
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
				command: &CreateUserCommand{
					Email:    newUser + "@xrspace.io",
					Password: "password",
					Username: "new_username",
					Nickname: "new_nickname",
				},
			},
			want: want{
				response: &CreateUserResponse{
					Username: "new_username",
				},
			},
		},
		{
			name: "create user with same email should fail",
			args: args{
				ctx: context.Background(),
				command: &CreateUserCommand{
					Email:    testUser + "@xrspace.io",
					Password: "password",
					Username: "new_username",
					Nickname: "new_nickname",
				},
			},
			wantErr:    true,
			wantErrMsg: "email already exists",
		},
		{
			name: "failed without nickname",
			args: args{
				ctx: context.Background(),
				command: &CreateUserCommand{
					Email:    testUser + "@xrspace.io",
					Password: "password",
					Username: "new_username",
				},
			},
			wantErr:    true,
			wantErrMsg: "Field validation for 'Nickname' failed on the 'required' tag",
		},
		{
			name: "failed with none email",
			args: args{
				ctx: context.Background(),
				command: &CreateUserCommand{
					Email:    testUser,
					Password: "password",
					Username: "new_username",
					Nickname: "new_nickname",
				},
			},
			wantErr:    true,
			wantErrMsg: "Field validation for 'Email' failed on the 'email' tag",
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			jwtService := jwt.NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret)
			userRepo := inmem.NewUserRepository()
			uc := NewCreateUserUsecase(jwtService, userRepo)

			// act
			got, err := uc.Execute(tt.args.ctx, tt.args.command)

			// assert error
			if (err != nil) != tt.wantErr {
				s.T().Errorf("CreateUserUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				s.ErrorContains(err, tt.wantErrMsg)
				return
			}

			// assert
			s.Nil(err)
			s.Equal(tt.want.response.Username, got.Username)
			s.NotNil(got.UserID)
		})
	}
}
