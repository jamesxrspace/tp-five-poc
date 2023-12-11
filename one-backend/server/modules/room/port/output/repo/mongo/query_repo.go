package mongo

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"xrspace.io/server/core/arch/port/pagination"
	mongoUtil "xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
)

type RoomQueryRepo struct {
	collection *mongo.Collection
}

var _ define.IQueryRepository = (*RoomQueryRepo)(nil)

func NewRoomQueryRepo(db *docdb.DocDB) *RoomQueryRepo {
	return &RoomQueryRepo{
		collection: db.Collection(roomCollectionName),
	}
}

func (r *RoomQueryRepo) ListRoomBySpaceID(ctx context.Context, spaceID string, page *pagination.PaginationQuery) (roomList []*entity.Room, total int, err error) {
	return mongoUtil.FindList[*entity.Room](ctx, r.collection, bson.M{"space_id": spaceID}, page, nil)
}
