package command

import (
	"context"
	"time"

	"github.com/go-playground/validator/v10"
	"github.com/rs/zerolog"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/entity"
	"xrspace.io/server/modules/account/domain/proxy"
)

const (
	managerTokenKey = "auth_manager_token"
	bufferSecs      = 7000
)

type CreateGuestCommand struct {
	Nickname string `json:"nickname" validate:"required"`
}

func (c *CreateGuestCommand) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

type CreateGuestResponse struct {
	UserID   string `json:"userId"`
	Email    string `json:"email"`
	Password string `json:"password"`
	Username string `json:"username"`
	Nickname string `json:"nickname"`
}

type CreateGuestUseCase struct {
	dep define.Dependency
}

func NewCreateGuestUseCase(dep define.Dependency) *CreateGuestUseCase {
	return &CreateGuestUseCase{
		dep: dep,
	}
}

func (u *CreateGuestUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*CreateGuestCommand)

	token := u.getManagerToken(ctx)

	if token == "" {
		return nil, core_error.StackError("failed to get manager token")
	}

	guest := entity.NewGuest(&u.dep.Config.OIDC.Guest, cmd.Nickname)

	resp, err := u.dep.AuthProxy.CreateAccount(ctx, u.dep.Config.OIDC.PoolID, token, &proxy.CreateUserParams{
		Nickname:      guest.Nickname,
		Email:         guest.Email,
		Password:      guest.Password,
		EmailVerified: true,
		Company:       guest.Company,
	})
	if err != nil {
		return nil, err
	}

	return &CreateGuestResponse{
		UserID:   resp.UserID,
		Username: resp.Username,
		Nickname: string(cmd.Nickname),
		Email:    guest.Email,
		Password: guest.Password,
	}, nil
}

func (u *CreateGuestUseCase) getManagerToken(ctx context.Context) string {
	cachedToken := u.dep.RedisClient.Get(ctx, managerTokenKey)

	if cachedToken == "" || cachedToken == "null" {
		result, err := u.dep.AuthProxy.GetManagerToken(ctx, u.dep.Config.OIDC.PoolID, &proxy.GetManagerTokenParams{
			PoolID: u.dep.Config.OIDC.PoolID,
			Secret: u.dep.Config.OIDC.Secret,
		})
		if err != nil {
			log := zerolog.Ctx(ctx)
			log.Warn().Msgf("[CreateGuest] failed to get manager token: %s", u.dep.Config.OIDC.PoolID)
			return ""
		}
		cachedToken = string(result.AccessToken)
		expiration := (time.Duration(result.ExpiresIn) - bufferSecs) * time.Second
		_, err = u.dep.RedisClient.Set(ctx, managerTokenKey, cachedToken, expiration)
		if err != nil {
			return ""
		}
	}

	return cachedToken
}
