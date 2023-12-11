package mongo

import (
	"context"
	"errors"

	"github.com/rs/zerolog/log"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"

	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/settings"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/enum"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

const (
	avatarCollection       = "avatar"
	avatarPlayerCollection = "avatar_player"
)

var _ repository.IAvatarRepository = (*AvatarRepository)(nil)

type AvatarRepository struct {
	db                     *docdb.DocDB
	avatarCollection       *mongo.Collection
	avatarPlayerCollection *mongo.Collection
	avatarConfig           *settings.AvatarConfig
}

func NewAvatarRepository(docDB *docdb.DocDB, avatarConfig *settings.AvatarConfig) *AvatarRepository {
	return &AvatarRepository{
		db:                     docDB,
		avatarCollection:       docDB.Collection(avatarCollection),
		avatarPlayerCollection: docDB.Collection(avatarPlayerCollection),
		avatarConfig:           avatarConfig,
	}
}

func (r *AvatarRepository) InitIndex(ctx context.Context) error {
	if err := r.db.PopulateIndex(ctx, avatarPlayerCollection, "xrid", 1, true); err != nil {
		return err
	}
	return r.db.PopulateIndex(ctx, avatarCollection, "avatar_id", 1, true)
}

func (r *AvatarRepository) SaveAvatarPlayer(ctx context.Context, player *entity.AvatarPlayer) error {
	filter := bson.M{
		"xrid": player.XrID,
	}
	update := bson.M{
		"$set": player,
	}

	opt := options.Update().SetUpsert(true)

	if _, err := r.avatarPlayerCollection.UpdateOne(ctx, filter, update, opt); err != nil {
		return err
	}

	return nil
}

func (r *AvatarRepository) SaveAvatar(ctx context.Context, avatar *entity.Avatar) error {
	filter := bson.M{
		"avatar_id": avatar.AvatarID,
	}

	update := bson.M{
		"$set": avatar,
	}

	opt := options.Update().SetUpsert(true)

	if _, err := r.avatarCollection.UpdateOne(ctx, filter, update, opt); err != nil {
		return err
	}

	return nil
}

func (r *AvatarRepository) GetAvatarPlayer(ctx context.Context, xrID value_object.XrID) (*entity.AvatarPlayer, error) {
	var player entity.AvatarPlayer
	filter := bson.M{
		"xrid": xrID,
	}
	if err := r.avatarPlayerCollection.FindOne(ctx, filter).Decode(&player); err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			log.Warn().Interface("xrid", xrID).Msg("[AvatarPlayer] player not found")
			return nil, nil
		}
		return nil, err
	}

	return &player, nil
}

func (r *AvatarRepository) GetAvatar(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error) {
	var avatar entity.Avatar

	filter := bson.M{
		"avatar_id": avatarID,
	}
	if err := r.avatarCollection.FindOne(ctx, filter).Decode(&avatar); err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			log.Warn().Interface("avatar_id", avatarID).Msg("[AvatarPlayer] avatar not found")
			return nil, nil
		}
		return nil, err
	}

	return &avatar, nil
}

func (r *AvatarRepository) ListAvatarPlayersByXrIDs(ctx context.Context, xrIDs []value_object.XrID, offset, size int) (*repository.ListAvatarPlayersByXrIDsResult, error) {
	filter := bson.M{}
	filter["xrid"] = bson.M{"$in": xrIDs}

	var result repository.ListAvatarPlayersByXrIDsResult
	result.Items = make([]*entity.AvatarPlayer, 0, size)

	curr, err := r.avatarPlayerCollection.Aggregate(ctx, docdb.GetPaginateAggregation(
		offset*size,
		size,
		filter,
		bson.D{
			{Key: "created_at", Value: 1},
		},
		bson.D{
			{Key: "_id", Value: 0},
		},
		nil,
	))
	if err != nil {
		return nil, err
	}

	if curr.Next(ctx) {
		if err := curr.Decode(&result); err != nil {
			return nil, err
		}
	}

	return &result, nil
}

func (r *AvatarRepository) ListAvatarByAvatarIDs(ctx context.Context, avatarIDs []value_object.AvatarID, offset, size int) (*repository.ListAvatarByAvatarIDsResult, error) {
	filter := bson.M{}
	filter["avatar_id"] = bson.M{"$in": avatarIDs}

	var result repository.ListAvatarByAvatarIDsResult
	result.Items = make([]*entity.Avatar, 0, size)

	curr, err := avatarCollectionPaginate(ctx, r, offset, size, filter)
	if err != nil {
		return nil, err
	}

	if curr.Next(ctx) {
		if err := curr.Decode(&result); err != nil {
			return nil, err
		}
	}

	return &result, nil
}

func (r *AvatarRepository) GetDefaultAvatar(ctx context.Context, appID value_object.AppID) (*entity.Avatar, error) {
	defaultAvatar := &entity.Avatar{
		AvatarID: value_object.AvatarID("default_avatar_id"),
		AppID:    appID,
		Type:     enum.AvatarTypeXrV2,
		AvatarFormat: map[string]any{
			"vrm_id": "P_M_C01_02",
		},
		AvatarUrl: value_object.AssetUrl(r.avatarConfig.DefaultAvatar.AvatarUrl),
		Thumbnail: &entity.AvatarThumbnail{
			Head:      value_object.AssetUrl(r.avatarConfig.DefaultAvatar.Thumbnail.Head),
			UpperBody: value_object.AssetUrl(r.avatarConfig.DefaultAvatar.Thumbnail.UpperBody),
			FullBody:  value_object.AssetUrl(r.avatarConfig.DefaultAvatar.Thumbnail.FullBody),
		},
	}

	return defaultAvatar, nil
}

func avatarCollectionPaginate(ctx context.Context, r *AvatarRepository, offset int, size int, filter bson.M) (*mongo.Cursor, error) {
	curr, err := r.avatarCollection.Aggregate(ctx, docdb.GetPaginateAggregation(
		offset*size,
		size,
		filter,
		bson.D{
			{Key: "created_at", Value: 1},
		},
		bson.D{
			{Key: "_id", Value: 0},
		},
		nil,
	))
	return curr, err
}
