package query

import (
	"auth_platform/core/service/jwt"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"auth_platform/modules/authorization/domain/repository"
	"context"
	"time"

	"github.com/go-playground/validator/v10"
)

type RefreshTokenQuery struct {
	GrantType    string `form:"grant_type" json:"grant_type" validate:"required"`
	ClientId     string `form:"client_id" json:"client_id" validate:"required"`
	RefreshToken string `form:"refresh_token" json:"refresh_token" validate:"required"`
	ExpireSecs   int    `form:"expire_secs" json:"expire_secs"`
}

type RefreshTokenResponse struct {
	RefreshToken string `json:"refresh_token"`
	Scope        string `json:"scope"`
	TokenType    string `json:"token_type"`
	AccessToken  string `json:"access_token"`
	IDToken      string `json:"id_token"`
	ExpiresIn    int    `json:"expires_in"`
}

type RefreshTokenUsecase struct {
	jwtService *jwt.JwtService
	repo       repository.IUserRepository
}

func NewRefreshTokenUsecase(s *jwt.JwtService, repo repository.IUserRepository) *RefreshTokenUsecase {
	return &RefreshTokenUsecase{
		jwtService: s,
		repo:       repo,
	}
}

func (q *RefreshTokenQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

func (uc *RefreshTokenUsecase) Execute(ctx context.Context, q *RefreshTokenQuery) (*RefreshTokenResponse, error) {
	if err := q.Validate(); err != nil {
		return nil, auth_errors.NewErrInvalidArguments(err, q)
	}

	user, err := uc.repo.FindUserByRefreshToken(q.RefreshToken)
	if err != nil || user == nil {
		return &RefreshTokenResponse{}, auth_errors.NewErrInvalidToken(err, q.RefreshToken)
	}

	if q.ExpireSecs == 0 {
		q.ExpireSecs = 60 * 60 * 24
	}

	token, err := uc.jwtService.GenAccessToken(user.ID, q.ExpireSecs)
	if err != nil {
		return &RefreshTokenResponse{}, auth_errors.NewErrGenToken(err, user.ID)
	}

	refreshToken, err := uc.jwtService.GenRefreshToken(user.ID)
	if err != nil {
		return &RefreshTokenResponse{}, auth_errors.NewErrRefreshToken(err, user.ID)
	}
	user.AccessToken = token
	user.RefreshToken = refreshToken
	user.UpdatedAt = time.Now()
	err = uc.repo.Save(user)
	if err != nil {
		return &RefreshTokenResponse{}, auth_errors.NewErrSaveUser(err, user)
	}

	return &RefreshTokenResponse{
		RefreshToken: refreshToken,
		Scope:        "openid email profile phone",
		TokenType:    "Bearer",
		AccessToken:  token,
		ExpiresIn:    int(time.Nanosecond) * q.ExpireSecs,
		IDToken:      "ID_TOKEN",
	}, nil
}
