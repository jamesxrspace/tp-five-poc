package inmem

import (
	"xrspace.io/server/core/arch/port/repository/inmem"
	"xrspace.io/server/modules/account/domain/entity"
	"xrspace.io/server/modules/account/domain/repository"
)

func NewAccountRepository(storage map[interface{}]*entity.Account) repository.IAccountRepository {
	return inmem.NewInMemGenericRepository[*entity.Account](storage)
}
