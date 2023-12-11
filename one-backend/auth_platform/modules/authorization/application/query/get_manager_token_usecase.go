package query

import (
	"auth_platform/core/service/jwt"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"context"
	"time"

	"github.com/go-playground/validator/v10"
)

type GetManagerTokenQuery struct {
	AccessKeyID     string `json:"accessKeyId" validate:"required"`
	AccessKeySecret string `json:"accessKeySecret" validate:"required"`
	ExpireSecs      int    `json:"expire_secs"`
}

type GetManagerTokenResponse struct {
	AccessToken string `json:"access_token"`
	ExpiresIn   int    `json:"expires_in"`
}

type GetManagerTokenUsecase struct {
	jwtService *jwt.JwtService
}

func NewGetManagerTokenUsecase(s *jwt.JwtService) *GetManagerTokenUsecase {
	return &GetManagerTokenUsecase{
		jwtService: s,
	}
}

func (q *GetManagerTokenQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

func (uc *GetManagerTokenUsecase) Execute(ctx context.Context, q *GetManagerTokenQuery) (*GetManagerTokenResponse, error) {
	if err := q.Validate(); err != nil {
		return nil, auth_errors.NewErrInvalidArguments(err, q)
	}

	ExpireSecs := q.ExpireSecs
	if ExpireSecs == 0 {
		ExpireSecs = 60 * 60 * 2
	}

	token, err := uc.jwtService.GenManagerToken(q.AccessKeyID, q.AccessKeySecret, ExpireSecs)
	if err != nil {
		return nil, err
	}

	return &GetManagerTokenResponse{
		AccessToken: token,
		ExpiresIn:   int(time.Nanosecond) * ExpireSecs,
	}, nil
}
