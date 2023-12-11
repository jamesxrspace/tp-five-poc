package query

import (
	"auth_platform/core/service/jwt"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"auth_platform/modules/authorization/domain/repository"
	"context"
	"errors"
	"time"

	"github.com/go-playground/validator/v10"
)

type GetTokenQuery struct {
	GrantType  string `form:"grant_type" json:"grant_type" validate:"required"`
	Email      string `form:"email" json:"email" validate:"required,email"`
	Password   string `form:"password" json:"password" validate:"required"`
	ExpireSecs int    `form:"expire_secs" json:"expire_secs"`
}

type GetTokenResponse struct {
	Scope        string `json:"scope"`
	TokenType    string `json:"token_type"`
	AccessToken  string `json:"access_token"`
	IDToken      string `json:"id_token"`
	RefreshToken string `json:"refresh_token"`
	ExpiresIn    int    `json:"expires_in"`
}

type GetTokenUsecase struct {
	jwtService *jwt.JwtService
	repo       repository.IUserRepository
}

func NewGetTokenUsecase(s *jwt.JwtService, repo repository.IUserRepository) *GetTokenUsecase {
	return &GetTokenUsecase{
		jwtService: s,
		repo:       repo,
	}
}

func (q *GetTokenQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

func (uc *GetTokenUsecase) Execute(ctx context.Context, q *GetTokenQuery) (*GetTokenResponse, error) {
	if err := q.Validate(); err != nil {
		return nil, auth_errors.NewErrInvalidArguments(err, q)
	}

	user, err := uc.repo.FindUserByEmail(q.Email)
	if err != nil || user == nil {
		return &GetTokenResponse{}, auth_errors.NewErrInvalidEmailOrPassword(errors.New("invalid email or password"), q.Email)
	}

	if err := user.CheckPassword(user.Password, q.Password); err != nil {
		return &GetTokenResponse{}, auth_errors.NewErrInvalidEmailOrPassword(errors.New("invalid email or password"), q)
	}

	if q.ExpireSecs == 0 {
		q.ExpireSecs = 60 * 60 * 24
	}

	accessToken, err := uc.jwtService.GenAccessToken(user.ID, q.ExpireSecs)
	if err != nil {
		return &GetTokenResponse{}, auth_errors.NewErrGenToken(err, user.ID)
	}

	refreshToken, err := uc.jwtService.GenRefreshToken(user.ID)
	if err != nil {
		return &GetTokenResponse{}, auth_errors.NewErrRefreshToken(err, user.ID)
	}
	user.AccessToken = accessToken
	user.RefreshToken = refreshToken
	user.UpdatedAt = time.Now()
	err = uc.repo.Save(user)
	if err != nil {
		return &GetTokenResponse{}, auth_errors.NewErrSaveUser(err, user)
	}

	return &GetTokenResponse{
		Scope:        "openid email profile phone",
		TokenType:    "Bearer",
		AccessToken:  accessToken,
		ExpiresIn:    int(time.Nanosecond) * q.ExpireSecs,
		IDToken:      "ID_TOKEN",
		RefreshToken: refreshToken,
	}, nil
}
