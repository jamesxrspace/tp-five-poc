package mongo

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	accountEntity "xrspace.io/server/modules/account/domain/entity"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
)

var _ define.IQueryRepository = (*QueryRepository)(nil)

const (
	AccountCollectionName  string = "account"
	CategoryCollectionName string = "category"
	FeedCollectionName     string = "feed"
	ReelCollectionName     string = "feed_reel"
)

type AccountDoc struct {
	*accountEntity.Account `bson:",inline"`
}

type FeedDoc struct {
	*entity.Feed `bson:",inline"`
}

type ReelDoc struct {
	*entity.Reel `bson:",inline"`
}

type QueryRepository struct {
	docDB              *docdb.DocDB
	accountCollection  *mongo.Collection
	categoryCollection *mongo.Collection
	feedCollection     *mongo.Collection
	feedReelCollection *mongo.Collection
}

func NewQueryRepository(dependencies *docdb.DocDB) *QueryRepository {
	return &QueryRepository{
		docDB:              dependencies,
		accountCollection:  dependencies.Collection(AccountCollectionName),
		categoryCollection: dependencies.Collection(CategoryCollectionName),
		feedCollection:     dependencies.Collection(FeedCollectionName),
		feedReelCollection: dependencies.Collection(ReelCollectionName),
	}
}

func (r *QueryRepository) ListReel(ctx context.Context, filter *define.ListReelFilter) (*define.ListReelResult, error) {
	var result define.ListReelResult
	result.Items = make([]*entity.Reel, 0, filter.Size)

	listFilter := bson.M{}

	if filter.ReelID != "" {
		listFilter["id"] = filter.ReelID
	}

	if filter.XrID != "" {
		listFilter["xrid"] = filter.XrID
	}

	if filter.Status != "" {
		listFilter["status"] = filter.Status
	}

	curr, err := r.feedReelCollection.Aggregate(ctx, docdb.GetPaginateAggregation(
		filter.Skip(),
		filter.Limit(),
		listFilter,
		bson.D{
			{Key: "created_at", Value: -1},
		},
		bson.D{
			{Key: "_id", Value: 0},
		},
		nil,
	))
	if err != nil {
		return nil, core_error.StackError(err)
	}
	if curr.Next(ctx) {
		if err := curr.Decode(&result); err != nil {
			return nil, core_error.StackError(err)
		}
	}

	return &result, nil
}

func (r *QueryRepository) InitIndex(ctx context.Context) error {
	if err := r.docDB.PopulateIndex(ctx, ReelCollectionName, "id", 1, true); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, ReelCollectionName, "xrid", 1, false); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, FeedCollectionName, "id", 1, true); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, FeedCollectionName, "ref_id", 1, false); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, FeedCollectionName, "xrid", 1, false); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, FeedCollectionName, "created_at", -1, false); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, FeedCollectionName, "type", 1, false); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, FeedCollectionName, "categories", 1, false); err != nil {
		return core_error.StackError(err)
	}

	return r.docDB.PopulateIndex(ctx, FeedCollectionName, "status", 1, false)
}

func (r *QueryRepository) GetFeedByRef(ctx context.Context, feedType string, refID string) (*entity.Feed, error) {
	filter := bson.M{
		"type":   feedType,
		"ref_id": refID,
	}
	var feedDoc FeedDoc
	if err := r.feedCollection.FindOne(context.Background(), filter).Decode(&feedDoc); err != nil {
		if err == mongo.ErrNoDocuments {
			return nil, nil
		}
		return nil, core_error.StackError(err)
	}
	return feedDoc.Feed, nil
}

func (r *QueryRepository) ListFeed(ctx context.Context, filter *define.ListFeedFilter) (*define.ListFeedResult, error) {
	var result *define.ListFeedResult

	curr, err := r.feedCollection.Aggregate(ctx, bson.A{
		bson.M{
			"$match": genListFeedFilter(filter),
		},
		bson.M{
			"$facet": bson.M{
				"items": bson.A{
					bson.M{
						"$sort": bson.M{
							"updated_at": -1,
						},
					},
					bson.M{
						"$skip": filter.Skip(),
					},
					bson.M{
						"$limit": filter.Limit(),
					},
				},
				"total": bson.A{
					bson.M{
						"$count": "total",
					},
				},
			},
		},
		bson.M{
			"$unwind": bson.M{
				"path": "$total",
			},
		},
		bson.M{
			"$project": bson.M{
				"items": 1,
				"total": "$total.total",
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

func genListFeedFilter(filter *define.ListFeedFilter) bson.M {
	listFilter := bson.M{}

	if len(filter.XrIDs) > 0 {
		listFilter["xrid"] = bson.M{
			"$in": filter.XrIDs,
		}
	}

	if len(filter.Categories) > 0 {
		listFilter["categories"] = bson.M{
			"$in": filter.Categories,
		}
	}

	if filter.Status != "" {
		listFilter["status"] = filter.Status
	}

	return listFilter
}

func (r *QueryRepository) GetNicknames(ctx context.Context, xrIds []string) (map[string]string, error) {
	var accountDocs []AccountDoc

	filter := bson.M{
		"xrid": bson.M{
			"$in": xrIds,
		},
	}

	curr, err := r.accountCollection.Find(ctx, filter)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	if err := curr.All(ctx, &accountDocs); err != nil {
		return nil, core_error.StackError(err)
	}

	result := make(map[string]string, len(accountDocs))

	for _, doc := range accountDocs {
		result[doc.XrID] = doc.Nickname
	}

	return result, nil
}

func (r *QueryRepository) GetDemoXrids(ctx context.Context, xrID string) []string {
	// for demo purpose
	// only get those feed that create from those 10 demo user
	// and exclude self
	xridForDemo := []string{
		"91c1101d-c21c-41a2-b191-17b5fb606842",
		"0e319304-70cb-4500-810f-7434b9f71af7",
		"4374cb7b-c2d4-4f90-b1c8-821e93aaa9ff",
		"f0656106-f795-442b-bce2-842e02e202d9",
		"86240268-9fc6-4d34-8b00-6ac0f30be852",
		"0970a53e-c2cf-418c-8995-45766f9f7a8b",
		"53082f0c-243e-44a9-92e1-05826fdea05a",
		"dac3cb18-5f9f-468b-8342-2203b1332661",
		"ad29db73-a8cb-44bf-b50d-fa491c261935",
		"dd357514-f5e0-4b65-8282-5ac3a731cf9f",
	}

	// exclude the xrid itself
	for i, xrid := range xridForDemo {
		if xrid == xrID {
			xridForDemo = append(xridForDemo[:i], xridForDemo[i+1:]...)
			break
		}
	}

	return xridForDemo
}

func (r *QueryRepository) ListFeedCategory(ctx context.Context, filter *define.ListFeedCategoryFilter) ([]*assetEntity.CategoryItem, error) {
	var result []*assetEntity.CategoryItem

	listFilter := bson.M{
		"type": filter.Type,
	}

	if len(filter.TitleI18ns) > 0 {
		listFilter["title_i18n"] = bson.M{
			"$in": filter.TitleI18ns,
		}
	}

	curr, err := r.categoryCollection.Find(ctx, listFilter)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	defer curr.Close(ctx)

	if err := curr.All(ctx, &result); err != nil {
		return result, core_error.StackError(err)
	}

	return result, nil
}
