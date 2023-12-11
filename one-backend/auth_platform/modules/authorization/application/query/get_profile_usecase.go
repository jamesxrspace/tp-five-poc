package query

import (
	"auth_platform/core/service/jwt"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"auth_platform/modules/authorization/domain/repository"
	"context"

	"github.com/go-playground/validator/v10"
)

type GetProfileQuery struct {
	UserID      string `form:"user_id" validate:"required"`
	AccessToken string `form:"access_token" validate:"required"`
}

type GetProfileResponse struct {
	Username      string `json:"username"`
	Nickname      string `json:"nickname"`
	Email         string `json:"email"`
	Sub           string `json:"sub"`
	EmailVerified bool   `json:"email_verified"`
}

type GetProfileUsecase struct {
	jwtService *jwt.JwtService
	repo       repository.IUserRepository
}

func NewGetProfileUsecase(s *jwt.JwtService, repo repository.IUserRepository) *GetProfileUsecase {
	return &GetProfileUsecase{
		jwtService: s,
		repo:       repo,
	}
}

func (q *GetProfileQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

func (uc *GetProfileUsecase) Execute(ctx context.Context, q *GetProfileQuery) (*GetProfileResponse, error) {
	if err := q.Validate(); err != nil {
		return nil, auth_errors.NewErrInvalidArguments(err, q)
	}

	user, err := uc.repo.FindUserByID(q.UserID)
	if err != nil || user == nil {
		return &GetProfileResponse{}, auth_errors.NewErrInvalidToken(err, q.AccessToken)
	}

	if user.AccessToken != q.AccessToken {
		return &GetProfileResponse{}, auth_errors.NewErrInvalidToken(err, q.AccessToken)
	}

	return &GetProfileResponse{
		Username:      user.Username,
		Nickname:      user.Nickname,
		Email:         user.Email,
		Sub:           user.ID,
		EmailVerified: user.EmailVerified,
	}, nil
}
