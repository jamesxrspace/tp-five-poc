package mongo

import (
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/account/domain/entity"
	"xrspace.io/server/modules/account/domain/repository"
)

func NewAccountRepository(db *docdb.DocDB) repository.IAccountRepository {
	return mongo.NewGenericMongoRepository[*entity.Account](db.Collection(AccountCollectionName))
}
