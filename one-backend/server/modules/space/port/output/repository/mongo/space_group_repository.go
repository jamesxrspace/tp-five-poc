package mongo

import (
	"context"
	"errors"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/space/application/query"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/repository"
)

const (
	SpaceGroupCollectionName = "space_group_info"
)

type SpaceGroupRepository struct {
	db                   *docdb.DocDB
	spaceGroupCollection *mongo.Collection
}

var _ repository.ISpaceGroupRepository = (*SpaceGroupRepository)(nil)

var _ query.IQuerySpaceGroupRepository = (*SpaceGroupRepository)(nil)

func NewSpaceGroupRepository(db *docdb.DocDB) *SpaceGroupRepository {
	return &SpaceGroupRepository{
		db:                   db,
		spaceGroupCollection: db.Collection(SpaceGroupCollectionName),
	}
}

var ErrIdentifyFieldEmpty = core_error.NewInternalError("identify field space_group_id is empty")

func (s *SpaceGroupRepository) Create(ctx context.Context, spaceGroup *entity.SpaceGroup) (*entity.SpaceGroup, error) {
	if spaceGroup.SpaceGroupId == "" {
		return nil, ErrIdentifyFieldEmpty
	}
	_, err := s.spaceGroupCollection.InsertOne(ctx, spaceGroup)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}
	return spaceGroup, nil
}

func (s *SpaceGroupRepository) FindById(ctx context.Context, identify string) (*entity.SpaceGroup, error) {
	ent := &entity.SpaceGroup{}
	if err := s.spaceGroupCollection.FindOne(ctx, bson.M{"space_group_id": identify}).Decode(ent); err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			return nil, nil
		}
		return nil, core_error.NewInternalError(err)
	}
	return ent, nil
}

func (s *SpaceGroupRepository) List(ctx context.Context, paginationParams pagination.PaginationQuery, filterParams query.ListSpaceGroupFilter) (*query.ListSpaceGroupResponse, error) {
	f := bson.M{"archived_at": bson.M{"$exists": false}}
	if filterParams.Archive {
		f = bson.M{"archived_at": bson.M{"$exists": true}}
	}

	lookup := bson.D{
		{Key: "from", Value: SpaceCollectionName},
		{Key: "localField", Value: "space_group_id"},
		{Key: "foreignField", Value: "space_group_id"},
		{Key: "as", Value: "spaces"},
	}

	mongoPagination := docdb.NewMongoPagination[query.SpaceGroupResponse](ctx, s.spaceGroupCollection, f, nil, nil, lookup, paginationParams)
	response := &query.ListSpaceGroupResponse{
		PaginationResponse: pagination.PaginationResponse[query.SpaceGroupResponse]{
			Items: make([]query.SpaceGroupResponse, 0),
		},
	}
	err := mongoPagination.Decode(&response.PaginationResponse)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}
	return response, nil
}

func (s *SpaceGroupRepository) Save(ctx context.Context, spaceGroup *entity.SpaceGroup) error {
	filter := bson.M{
		"space_group_id": spaceGroup.SpaceGroupId,
	}
	update := bson.M{
		"$set": &spaceGroup,
	}
	if _, err := s.spaceGroupCollection.UpdateOne(ctx, filter, update); err != nil {
		return core_error.NewInternalError(err)
	}
	return nil
}
