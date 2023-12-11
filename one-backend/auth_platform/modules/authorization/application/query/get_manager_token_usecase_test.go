package query

import (
	service "auth_platform/core/service/jwt"
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
)

type TestGetManagerTokenUsecaseTestSuite struct {
	suite.Suite
}

func TestGetManagerToken(t *testing.T) {
	suite.Run(t, new(TestGetManagerTokenUsecaseTestSuite))
}

func (s *TestGetManagerTokenUsecaseTestSuite) TestGetManagerTokenUsecase_Execute() {
	type args struct {
		ctx   context.Context
		query *GetManagerTokenQuery
	}

	type want struct {
		response *GetManagerTokenResponse
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
				query: &GetManagerTokenQuery{
					AccessKeyID:     mockPooID,
					AccessKeySecret: mockSecret,
				},
			},
			want: want{
				response: &GetManagerTokenResponse{
					AccessToken: "token",
					ExpiresIn:   7200,
				},
			},
		},
		{
			name: "failed with wrong secret",
			args: args{
				ctx: context.Background(),
				query: &GetManagerTokenQuery{
					AccessKeyID:     mockPooID,
					AccessKeySecret: "wrong_secret",
				},
			},
			wantErr:    true,
			wantErrMsg: "invalid access key id or secret",
		},
		{
			name: "failed without access key id",
			args: args{
				ctx: context.Background(),
				query: &GetManagerTokenQuery{
					AccessKeySecret: mockSecret,
				},
			},
			wantErr:    true,
			wantErrMsg: "Field validation for 'AccessKeyID' failed on the 'required' tag",
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			jwtService := service.NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret)
			uc := NewGetManagerTokenUsecase(jwtService)

			// act
			got, err := uc.Execute(tt.args.ctx, tt.args.query)

			// assert error
			if (err != nil) != tt.wantErr {
				s.T().Errorf("GetManagerTokenUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				s.ErrorContains(err, tt.wantErrMsg)
				return
			}

			// assert
			s.Nil(err)
			s.Equal(tt.want.response.ExpiresIn, got.ExpiresIn)
			s.NotNil(got.AccessToken)
		})
	}
}
