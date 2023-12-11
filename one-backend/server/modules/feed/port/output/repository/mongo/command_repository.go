package mongo

import (
	"xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/repository"
)

func NewFeedRepository(db *docdb.DocDB) repository.IFeedRepository {
	return mongo.NewGenericMongoRepository[*entity.Feed](db.Collection(FeedCollectionName))
}

func NewReelRepository(db *docdb.DocDB) repository.IReelRepository {
	return mongo.NewGenericMongoRepository[*entity.Reel](db.Collection(ReelCollectionName))
}
