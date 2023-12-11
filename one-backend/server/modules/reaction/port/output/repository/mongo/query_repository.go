package mongo

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"xrspace.io/server/core/arch/core_error"
	mongoUtil "xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/feed/port/input/adapter"
	feedMongo "xrspace.io/server/modules/feed/port/output/repository/mongo"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/enum"
)

const LikeCollectionName string = "like"

var _ define.IQueryRepository = (*QueryRepository)(nil)

type QueryRepository struct {
	db             *docdb.DocDB
	likeCollection *mongo.Collection
	feedAdapter    *adapter.FeedAdapter
}

func NewQueryRepository(db *docdb.DocDB) *QueryRepository {
	feedRepository := feedMongo.NewFeedRepository(db)

	return &QueryRepository{
		db:             db,
		likeCollection: db.Collection(LikeCollectionName),
		feedAdapter:    adapter.NewFeedAdapter(feedRepository),
	}
}

func (r *QueryRepository) GetLike(ctx context.Context, filter define.GetLikeFilter) (*entity.Like, error) {
	result, err := mongoUtil.FindByFilter[*entity.Like](
		ctx,
		r.likeCollection,
		bson.M{
			"xrid":        filter.XrID,
			"target_type": filter.TargetType,
			"target_id":   filter.TargetID,
		})

	if err != nil {
		return nil, err
	}

	return result, nil
}

func (r *QueryRepository) GetLikeReaction(ctx context.Context, filter define.GetLikeFilter) (*define.GetLikeReactionResult, error) {
	var result *define.GetLikeReactionResult

	curr, err := r.likeCollection.Aggregate(ctx, bson.A{
		bson.M{
			"$match": bson.M{
				"target_type": filter.TargetType,
				"target_id":   filter.TargetID,
				"status":      enum.LikeStatusLiked,
			},
		},
		bson.M{
			"$facet": bson.M{
				"count": bson.A{
					bson.M{
						"$count": "count",
					},
				},
				"is_like": bson.A{
					bson.M{
						"$match": bson.M{
							"xrid": filter.XrID,
						},
					},
				},
			},
		},
		bson.M{
			"$unwind": bson.M{
				"path": "$count",
			},
		},
		bson.M{
			"$project": bson.M{
				"count":   "$count.count",
				"is_like": bson.M{"$size": "$is_like"},
			},
		},
	})
	if err != nil {
		return nil, core_error.StackError(err)
	}
	defer curr.Close(ctx)
	if curr.Next(ctx) {
		if err := curr.Decode(&result); err != nil {
			return nil, core_error.StackError(err)
		}
	}

	return result, nil
}

func (r *QueryRepository) IsFeedExist(ctx context.Context, feedID string) bool {
	feed, err := r.feedAdapter.GetFeed(ctx, feedID)
	if err != nil || feed == nil {
		return false
	}
	return true
}

func (r *QueryRepository) InitIndex(ctx context.Context) error {
	for _, item := range []string{"target_id", "xrid"} {
		if err := r.db.PopulateIndex(ctx, LikeCollectionName, item, 1, true); err != nil {
			return err
		}
	}
	return nil
}
