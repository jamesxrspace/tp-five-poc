package mongo

import (
	"context"
	"errors"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	mongoUtil "xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/space/application/query"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/repository"
)

const (
	SpaceCollectionName = "space"
)

type SpaceRepository struct {
	db              *docdb.DocDB
	spaceCollection *mongo.Collection
}

var _ repository.ISpaceRepository = (*SpaceRepository)(nil)

var _ query.IQuerySpaceRepository = (*SpaceRepository)(nil)

func NewSpaceRepository(db *docdb.DocDB) *SpaceRepository {
	return &SpaceRepository{
		db:              db,
		spaceCollection: db.Collection(SpaceCollectionName),
	}
}

var ErrSpaceIdEmpty = core_error.StackError("identify field space_id is empty")

func (s *SpaceRepository) Create(ctx context.Context, space *entity.Space) (*entity.Space, error) {
	if space.SpaceId == "" {
		return nil, ErrSpaceIdEmpty
	}
	_, err := s.spaceCollection.InsertOne(ctx, space)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	return space, nil
}

func (s *SpaceRepository) FindById(ctx context.Context, identify string) (*entity.Space, error) {
	ent := &entity.Space{}
	if err := s.spaceCollection.FindOne(ctx, bson.M{"space_id": identify}).Decode(ent); err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			return nil, nil
		}
		return nil, core_error.StackError(err)
	}
	return ent, nil
}

func (s *SpaceRepository) Save(ctx context.Context, space *entity.Space) error {
	filter := bson.M{
		"space_id": space.SpaceId,
	}
	update := bson.M{
		"$set": &space,
	}
	if _, err := s.spaceCollection.UpdateOne(ctx, filter, update); err != nil {
		return core_error.StackError(err)
	}
	return nil
}

func (s *SpaceRepository) SaveMany(ctx context.Context, spaces []*entity.Space) error {
	_, err := mongoUtil.SaveMany[*entity.Space](ctx, s.spaceCollection, spaces)
	if err != nil {
		return err
	}
	return nil
}

func (s *SpaceRepository) List(ctx context.Context, paginationParams pagination.PaginationQuery, filterParams query.ListSpaceFilter) (*query.ListSpaceResponse, error) {
	f := bson.M{"archived_at": bson.M{"$exists": false}}
	if filterParams.Archive {
		f = bson.M{"archived_at": bson.M{"$exists": true}}
	}
	if filterParams.SpaceGroupId != "" {
		f["space_group_id"] = filterParams.SpaceGroupId
	}

	mongoPagination := docdb.NewMongoPagination[entity.Space](ctx, s.spaceCollection, f, nil, nil, nil, paginationParams)
	response := &query.ListSpaceResponse{
		PaginationResponse: pagination.PaginationResponse[entity.Space]{
			Items: make([]entity.Space, 0),
		},
	}
	err := mongoPagination.Decode(&response.PaginationResponse)
	return response, err
}

func (s *SpaceRepository) FindBySpaceGroupId(ctx context.Context, spaceGroupId string) ([]*entity.Space, error) {
	var spaces []*entity.Space
	filter := bson.M{
		"space_group_id": spaceGroupId,
	}
	curr, err := s.spaceCollection.Find(ctx, filter)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	if err := curr.All(ctx, &spaces); err != nil {
		return nil, core_error.StackError(err)
	}
	return spaces, nil
}

func (s *SpaceRepository) FindByIds(ctx context.Context, identify []string) ([]*entity.Space, error) {
	if len(identify) == 0 {
		return nil, nil
	}
	spaces := make([]*entity.Space, 0, len(identify))
	filter := bson.M{
		"space_id": bson.M{"$in": identify},
	}
	curr, err := s.spaceCollection.Find(ctx, filter)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	if err := curr.All(ctx, &spaces); err != nil {
		return nil, core_error.StackError(err)
	}
	return spaces, nil
}
