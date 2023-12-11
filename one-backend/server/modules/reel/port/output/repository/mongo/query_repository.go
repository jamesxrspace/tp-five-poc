package mongo

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	mongoUtil "xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	feedMongo "xrspace.io/server/modules/feed/port/output/repository/mongo"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/domain/entity"
)

var _ define.IQueryRepository = (*QueryRepository)(nil)

const (
	categoryCollection string = feedMongo.CategoryCollectionName
	ReelCollectionName string = "feed_reel"
)

type ReelDoc struct {
	*entity.Reel `bson:",inline"`
}

type QueryRepository struct {
	docDB              *docdb.DocDB
	categoryCollection *mongo.Collection
	feedReelCollection *mongo.Collection
}

func NewQueryRepository(dependencies *docdb.DocDB) *QueryRepository {
	return &QueryRepository{
		docDB:              dependencies,
		categoryCollection: dependencies.Collection(categoryCollection),
		feedReelCollection: dependencies.Collection(ReelCollectionName),
	}
}

func (r *QueryRepository) ListReel(ctx context.Context, filter *define.ListReelFilter) (*define.ListReelResult, error) {
	var result define.ListReelResult
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

	findResult, total, err := mongoUtil.FindList[*entity.Reel](ctx, r.feedReelCollection, listFilter, &pagination.PaginationQuery{
		Offset: filter.Skip(),
		Size:   filter.Limit(),
	}, bson.M{
		"created_at": -1,
	})

	if err != nil {
		return nil, err
	}

	result.Items = findResult
	result.Total = total

	return &result, nil
}

func (r *QueryRepository) InitIndex(ctx context.Context) error {
	if err := r.docDB.PopulateIndex(ctx, ReelCollectionName, "id", 1, true); err != nil {
		return core_error.StackError(err)
	}

	if err := r.docDB.PopulateIndex(ctx, ReelCollectionName, "xrid", 1, false); err != nil {
		return core_error.StackError(err)
	}

	return nil
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
