package command

import (
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/domain/entity"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"auth_platform/modules/authorization/domain/repository"
	"context"
	"errors"
	"strings"

	"github.com/go-playground/validator/v10"
	"github.com/google/uuid"
	"golang.org/x/crypto/bcrypt"
)

type CreateUserCommand struct {
	Username      string `form:"username" json:"username"`
	Nickname      string `form:"nickname" json:"nickname" validate:"required"`
	Email         string `form:"email" json:"email" validate:"required,email"`
	Password      string `form:"password" json:"password" validate:"required"`
	Company       string `form:"company" json:"company"`
	EmailVerified bool   `form:"emailVerified" json:"emailVerified"`
	ExpireSecs    int    `form:"expire_secs" json:"expire_secs"`
}

type CreateUserResponse struct {
	UserID   string `json:"userId"`
	Username string `json:"username"`
}

type CreateUserUsecase struct {
	jwtService *jwt.JwtService
	repo       repository.IUserRepository
}

func NewCreateUserUsecase(s *jwt.JwtService, repo repository.IUserRepository) *CreateUserUsecase {
	return &CreateUserUsecase{
		jwtService: s,
		repo:       repo,
	}
}

func (c *CreateUserCommand) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

func (uc CreateUserUsecase) Execute(ctx context.Context, c *CreateUserCommand) (*CreateUserResponse, error) {
	if err := c.Validate(); err != nil {
		return nil, auth_errors.NewErrInvalidArguments(err, c)
	}

	userExists, _ := uc.repo.FindUserByEmail(c.Email)
	if userExists != nil {
		return &CreateUserResponse{}, auth_errors.NewErrEmailDuplicated(errors.New("email already exists"), c.Email)
	}

	id := uuid.NewString()
	if c.Username == "" {
		c.Username = genRandomUsername()
	}
	user := entity.NewUser(id, c.Username, c.Nickname, c.Password, c.Email, c.Company, c.EmailVerified, bcrypt.DefaultCost)
	err := uc.repo.Save(user)
	if err != nil {
		return &CreateUserResponse{}, auth_errors.NewErrCreateUser(err, c)
	}

	return &CreateUserResponse{
		UserID:   user.ID,
		Username: user.Username,
	}, nil
}

func genRandomUsername() string {
	randomString := strings.ToUpper(strings.ReplaceAll(uuid.NewString(), "-", "")[:12])

	return "XRA" + randomString
}
