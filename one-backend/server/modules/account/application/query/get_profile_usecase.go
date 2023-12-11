package query

import (
	"context"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/proxy"
	"xrspace.io/server/modules/account/domain/value_object"

	"github.com/go-playground/validator/v10"
)

type GetProfileQuery struct {
	ResourceOwnerID string `token:"resource_owner_id" validate:"required"`
	AccessToken     string `token:"access_token" validate:"required"`
}

func (q *GetProfileQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

type GetProfileResponse struct {
	XrID                   string                        `json:"xr_id"`
	Email                  string                        `json:"email"`
	IssuerResourceOwnerIDs value_object.ResourceOwnerIDs `json:"issuer_resource_owner_ids"`
	Username               string                        `json:"username"`
	Nickname               string                        `json:"nickname"`
	IsEmailVerified        bool                          `json:"is_email_verified"`
}

type GetProfileUseCase struct {
	dep define.Dependency
}

func NewGetProfileUseCase(dep define.Dependency) *GetProfileUseCase {
	return &GetProfileUseCase{
		dep: dep,
	}
}

func (u *GetProfileUseCase) Execute(ctx context.Context, q any) (any, error) {
	query := q.(*GetProfileQuery)

	account, err := u.dep.QueryRepo.GetAccountByResourceOwnerID(ctx, query.ResourceOwnerID)
	if err != nil {
		return nil, err
	}

	if account == nil {
		return nil, core_error.StackError("account not found")
	}

	profile, err := u.dep.AuthProxy.GetProfile(ctx, &proxy.GetProfileParams{
		AccessToken: query.AccessToken,
	})
	if err != nil {
		return nil, err
	}

	return &GetProfileResponse{
		Email:                  profile.Email,
		XrID:                   account.XrID,
		IssuerResourceOwnerIDs: account.IssuerResourceOwnerIDs,
		Username:               account.Username,
		Nickname:               account.Nickname,
		IsEmailVerified:        profile.IsEmailVerified,
	}, nil
}
