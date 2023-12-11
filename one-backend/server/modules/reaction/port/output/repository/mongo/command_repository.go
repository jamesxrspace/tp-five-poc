package mongo

import (
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/repository"
)

func NewLikeRepository(db *docdb.DocDB) repository.ILikeRepository {
	return mongo.NewGenericMongoRepository[*entity.Like](db.Collection(LikeCollectionName))
}
