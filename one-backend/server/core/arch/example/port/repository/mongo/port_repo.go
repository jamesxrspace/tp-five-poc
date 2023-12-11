package mongo

import (
	"xrspace.io/server/core/arch/example/domain/entity"
	"xrspace.io/server/core/arch/example/domain/repository"
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
)

func NewRoomRepository(db *docdb.DocDB) repository.IExampleRoomRepository {
	return mongo.NewGenericMongoRepository[*entity.ExampleRoom](db.Collection(docdb.MEMGO_DEFAULT_DB))
}
