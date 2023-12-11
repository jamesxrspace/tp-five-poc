package adapter

import (
	"context"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/account/application/define"
)

type AccountAdapter struct {
	repository define.IAccountRepository
}

func NewAccountAdapter(repository define.IAccountRepository) *AccountAdapter {
	return &AccountAdapter{
		repository: repository,
	}
}

func (a *AccountAdapter) GetXrID(ctx context.Context, resourceOwnerID string) (string, error) {
	account, err := a.repository.GetAccountByResourceOwnerID(ctx, resourceOwnerID)
	if err != nil {
		return "", err
	}

	if account == nil {
		return "", core_error.NewEntityNotFoundError("account", resourceOwnerID)
	}

	return account.XrID, nil
}
