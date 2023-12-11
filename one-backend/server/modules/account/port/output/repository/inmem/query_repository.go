package inmem

import (
	"context"

	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/entity"

	"github.com/google/uuid"
)

var _ define.IAccountRepository = (*QueryRepository)(nil)

type QueryRepository struct {
	accounts map[interface{}]*entity.Account
}

func NewQueryRepository(accounts map[interface{}]*entity.Account) *QueryRepository {
	return &QueryRepository{
		accounts: accounts,
	}
}

func (r *QueryRepository) GenXrID(ctx context.Context) (xrID string) {
	return uuid.NewString()
}

func (r *QueryRepository) GetAccountByResourceOwnerID(ctx context.Context, resourceOwnerID string) (*entity.Account, error) {
	for _, account := range r.accounts {
		for _, v := range account.IssuerResourceOwnerIDs {
			if v == resourceOwnerID {
				return account, nil
			}
		}
	}

	return nil, nil
}
