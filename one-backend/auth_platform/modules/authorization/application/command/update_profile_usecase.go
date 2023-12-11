package command

import (
	"auth_platform/core/service/jwt"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"auth_platform/modules/authorization/domain/repository"
	"context"
	"errors"
	"time"

	"github.com/go-playground/validator/v10"
)

type UpdateProfileCommand struct {
	UserID      string `form:"user_id" validate:"required"`
	AccessToken string `form:"access_token" validate:"required"`
	Username    string `form:"username"`
	Nickname    string `form:"nickname"`
	Email       string `form:"email"`
}

type UpdateProfileResponse struct {
	UpdatedAt     time.Time `json:"updated_at"`
	UserID        string    `json:"user_id"`
	Email         string    `json:"email"`
	Username      string    `json:"username"`
	Nickname      string    `json:"nickname"`
	EmailVerified bool      `json:"email_verified"`
}

type UpdateProfileUsecase struct {
	jwtService *jwt.JwtService
	repo       repository.IUserRepository
}

func NewUpdateProfileUsecase(s *jwt.JwtService, repo repository.IUserRepository) *UpdateProfileUsecase {
	return &UpdateProfileUsecase{
		jwtService: s,
		repo:       repo,
	}
}

func (c *UpdateProfileCommand) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

func (uc UpdateProfileUsecase) Execute(ctx context.Context, c *UpdateProfileCommand) (*UpdateProfileResponse, error) {
	if err := c.Validate(); err != nil {
		return nil, auth_errors.NewErrInvalidArguments(err, c)
	}

	user, err := uc.repo.FindUserByID(c.UserID)
	if err != nil || user == nil {
		return &UpdateProfileResponse{}, auth_errors.NewErrInvalidToken(err, c.AccessToken)
	}

	if user.AccessToken != c.AccessToken {
		return &UpdateProfileResponse{}, auth_errors.NewErrInvalidToken(err, c.AccessToken)
	}

	if c.Username != "" {
		user.Username = c.Username
	}

	if c.Nickname != "" {
		user.Nickname = c.Nickname
	}

	if c.Email != "" {
		userExists, _ := uc.repo.FindUserByEmail(c.Email)
		if userExists != nil {
			return &UpdateProfileResponse{}, auth_errors.NewErrEmailDuplicated(errors.New("email already exists"), c.Email)
		}

		user.Email = c.Email
		user.EmailVerified = false
	}

	user.UpdatedAt = time.Now()
	err = uc.repo.Save(user)
	if err != nil {
		return &UpdateProfileResponse{}, auth_errors.NewErrUpdateProfile(err, c)
	}

	return &UpdateProfileResponse{
		UserID:        user.ID,
		Email:         user.Email,
		Username:      user.Username,
		Nickname:      user.Nickname,
		EmailVerified: user.EmailVerified,
		UpdatedAt:     user.UpdatedAt,
	}, nil
}
