package mongo

import (
	"context"
	"errors"

	"github.com/rs/zerolog/log"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/domain/repository"
)

const (
	roomCollectionName = "room_info"
)

var _ repository.IRoomRepository = (*RoomRepo)(nil)

type RoomRepo struct {
	db             *docdb.DocDB
	roomCollection *mongo.Collection
}

func NewRoomRepo(db *docdb.DocDB) *RoomRepo {
	return &RoomRepo{
		db:             db,
		roomCollection: db.Collection(roomCollectionName),
	}
}

func (i *RoomRepo) Get(ctx context.Context, id string) (*entity.Room, error) {
	var room entity.Room
	filter := bson.M{
		"room_id": id,
	}

	if err := i.roomCollection.FindOne(ctx, filter).Decode(&room); err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			log.Warn().Msgf("[RoomRepo] room not found: %v", err)
			return nil, nil
		}
		return nil, core_error.NewInternalError(err)
	}

	return &room, nil
}

func (i *RoomRepo) Save(ctx context.Context, room *entity.Room) error {
	filter := bson.M{
		"room_id": room.ID,
	}

	update := bson.M{
		"$set": room,
	}

	opt := options.Update().SetUpsert(true)

	if _, err := i.roomCollection.UpdateOne(ctx, filter, update, opt); err != nil {
		return core_error.NewInternalError(err)
	}

	return nil
}

func (i *RoomRepo) AllRoomData(ctx context.Context) ([]*entity.Room, error) {
	var rooms []*entity.Room

	curr, err := i.roomCollection.Find(ctx, bson.D{})

	if err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) || errors.Is(err, mongo.ErrNilDocument) {
			log.Warn().Msgf("[RoomRepo] room not found: %v", err)
			return rooms, nil
		}
		return rooms, core_error.NewInternalError(err)
	}

	err = curr.All(ctx, &rooms)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	return rooms, nil
}

func (i *RoomRepo) GetByID(ctx context.Context, id string) (*entity.Room, error) {
	c := i.db.Collection(roomCollectionName).FindOne(ctx, bson.M{"room_id": id})
	if c.Err() != nil {
		return nil, core_error.NewInternalError(c.Err())
	}

	result := &entity.Room{}
	err := c.Decode(result)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	return result, nil
}
