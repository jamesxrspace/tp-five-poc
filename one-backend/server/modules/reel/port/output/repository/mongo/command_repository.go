package mongo

import (
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/reel/domain/entity"
	"xrspace.io/server/modules/reel/domain/repository"
)

func NewReelRepository(db *docdb.DocDB) repository.IReelRepository {
	return mongo.NewGenericMongoRepository[*entity.Reel](db.Collection(ReelCollectionName))
}
