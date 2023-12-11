package repository

import (
	"context"

	"xrspace.io/server/core/arch/domain/repository"
	"xrspace.io/server/modules/account/domain/entity"
)

var _ repository.IDomainRepository[*entity.Account] = (IAccountRepository)(nil)

type IAccountRepository interface {
	Save(ctx context.Context, account *entity.Account) error
	Get(ctx context.Context, id string) (*entity.Account, error)
}
