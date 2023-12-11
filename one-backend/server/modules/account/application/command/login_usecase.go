package command

import (
	"context"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/entity"
	"xrspace.io/server/modules/account/domain/proxy"
	"xrspace.io/server/modules/account/domain/value_object"

	"github.com/go-playground/validator/v10"
)

type LoginCommand struct {
	AccessToken     string `token:"access_token" validate:"required"`
	Issuer          string `token:"issuer" validate:"required"`
	ResourceOwnerID string `token:"resource_owner_id" validate:"required"`
}

func (c *LoginCommand) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

type LoginResponse struct {
	Message string `json:"message"`
}

type LoginUseCase struct {
	dep define.Dependency
}

func NewLoginUseCase(dep define.Dependency) *LoginUseCase {
	return &LoginUseCase{
		dep: dep,
	}
}

func (u *LoginUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*LoginCommand)

	account, err := u.dep.QueryRepo.GetAccountByResourceOwnerID(ctx, cmd.ResourceOwnerID)
	if err != nil {
		return nil, err
	}

	if account == nil {
		err = u.createAccount(ctx, cmd)
		if err != nil {
			return nil, err
		}
	}

	return &LoginResponse{
		Message: "OK",
	}, nil
}

func (u *LoginUseCase) createAccount(ctx context.Context, cmd *LoginCommand) error {
	xrID := u.dep.QueryRepo.GenXrID(ctx)

	profile, err := u.dep.AuthProxy.GetProfile(ctx, &proxy.GetProfileParams{
		AccessToken: cmd.AccessToken,
	})
	if err != nil {
		return err
	}

	if profile.Username == "" {
		return core_error.StackError("username is empty")
	}

	resourceOwnerIDs := value_object.NewResourceOwnerIDs(
		map[string]string{
			cmd.Issuer: cmd.ResourceOwnerID,
		},
	)

	newAccount := entity.NewAccount(&entity.AccountParams{
		XrID:                   xrID,
		IssuerResourceOwnerIDs: resourceOwnerIDs,
		Username:               profile.Username,
		Nickname:               profile.Nickname,
	})

	if err := u.dep.AccountRepo.Save(ctx, newAccount); err != nil {
		return err
	}

	return nil
}
