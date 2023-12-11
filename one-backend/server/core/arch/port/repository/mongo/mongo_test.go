package mongo_test

import (
	"context"
	"fmt"
	"reflect"
	"testing"

	"github.com/google/uuid"
	"github.com/stretchr/testify/require"
	"github.com/stretchr/testify/suite"
	"go.mongodb.org/mongo-driver/bson"

	"xrspace.io/server/core/arch/port/pagination"
	mongoUtil "xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/core/dependency/database/docdb"
)

func TestMongoUtilTestSuite(t *testing.T) {
	suite.Run(t, new(MongoUtilTestSuite))
}

type MongoUtilTestSuite struct {
	suite.Suite
	docdb.InmemMongoBasicTestSuite
}

type doc struct {
	// set the pk of the document, only one filed in pk is supported
	ID   string `bson:"id" pk:"true"`
	Name string `bson:"name"`
	Type string `bson:"type"`
}

func (s *MongoUtilTestSuite) Test_SaveAndFind() {
	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	// act
	r1, err := mongoUtil.Save(ctx, col, &doc{"1", "test1", "type1"})
	r2, err2 := mongoUtil.Save(ctx, col, &doc{"2", "test2", "type1"})

	// find all
	paginationQuery := &pagination.PaginationQuery{Offset: 0, Size: 10}
	result, total, err3 := mongoUtil.FindList[*doc](ctx, col, bson.M{"type": "type1"}, paginationQuery, nil)

	// find by id
	d, err4 := mongoUtil.FindByID[*doc](ctx, col, "1")

	// assert
	s.Equal(2, len(result))
	s.Equal(2, total)

	s.Equal("1", d.ID)
	s.Equal("test1", d.Name)

	s.Equal(int64(1), r1.UpsertedCount)
	s.Equal(int64(1), r2.UpsertedCount)
	s.Nil(err)
	s.Nil(err2)
	s.Nil(err3)
	s.Nil(err4)
}

func (s *MongoUtilTestSuite) Test_SaveUpsert() {
	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	// insert
	d := &doc{"1", "test1", "type1"}
	r1, err := mongoUtil.Save(ctx, col, d)
	d, err4 := mongoUtil.FindByID[*doc](ctx, col, "1")

	// act - update
	d.Name = "test2"
	r2, _ := mongoUtil.Save(ctx, col, d)
	d2, _ := mongoUtil.FindByID[*doc](ctx, col, "1")

	// assert
	s.Equal("1", d2.ID)
	s.Equal("test2", d2.Name)

	s.Equal(int64(1), r1.UpsertedCount)
	s.Equal(int64(0), r2.UpsertedCount)
	s.Equal(int64(1), r2.ModifiedCount)
	s.Nil(err)
	s.Nil(err4)
}

func (s *MongoUtilTestSuite) Test_SaveAndFindT() {
	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	// act
	r1, err := mongoUtil.Save(ctx, col, &doc{"1", "test1", "type1"})
	r2, err2 := mongoUtil.Save(ctx, col, &doc{"2", "test2", "type1"})

	// find all
	result, total, err3 := mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type1"}, nil, nil)

	// find by id
	d, err4 := mongoUtil.FindByID[*doc](ctx, col, "1")

	// assert
	s.Equal(2, len(result))
	s.Equal(2, total)

	s.Equal("1", d.ID)
	s.Equal("test1", d.Name)
	s.Equal(int64(1), r1.UpsertedCount)
	s.Equal(int64(1), r2.UpsertedCount)

	s.Nil(err)
	s.Nil(err2)
	s.Nil(err3)
	s.Nil(err4)
}

func (s *MongoUtilTestSuite) Test_InsertMany() {
	d := []*doc{
		{"1", "test1", "type1"},
		{"2", "test2", "type1"},
		{"3", "test3", "type1"},
		{"4", "test4", "type1"},
	}

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	// act
	manyResult, err := mongoUtil.InsertMany(ctx, col, d)
	if err != nil {
		return
	}

	// assert
	s.Equal(4, len(manyResult.InsertedIDs))
}

func (s *MongoUtilTestSuite) Test_SaveMany() {
	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	d := []*doc{
		{"1", "test1", "type1"},
		{"2", "test2", "type1"},
		{"3", "test3", "type1"},
		{"4", "test4", "type1"},
	}

	_, _ = mongoUtil.SaveMany(ctx, col, d)

	// act
	d = []*doc{
		{"1", "test1", "type2"},
		{"2", "test2", "type2"},
		{"3", "test3", "type2"},
		{"4", "test4", "type2"},
	}

	var manyResult, err = mongoUtil.SaveMany(ctx, col, d)

	result, total, _ := mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type2"}, nil, nil)

	s.Equal(4, int(manyResult.ModifiedCount))
	s.Equal(4, len(result))
	s.Equal(4, total)

	s.Nil(err)
}

func (s *MongoUtilTestSuite) Test_FindAllAndPage() {
	d := []*doc{
		{"1", "test1", "type1"},
		{"2", "test2", "type1"},
		{"3", "test3", "type1"},
		{"4", "test4", "type1"},
	}

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	_, err := mongoUtil.InsertMany(ctx, col, d)
	if err != nil {
		return
	}

	// act
	var result, total, _ = mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type1"}, nil, nil)
	var result2, total2, _ = mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type1"}, &pagination.PaginationQuery{Offset: 1, Size: 1}, nil)

	// assert
	s.Equal(4, total)
	s.Equal(4, len(result))

	s.Equal(4, total2)
	s.Equal(1, len(result2))
	s.Equal("2", result2[0].ID)
}

func (s *MongoUtilTestSuite) Test_FindAllAndsort() {
	d := []*doc{
		{"1", "test1", "type1"},
		{"2", "test2", "type1"},
		{"3", "test3", "type1"},
		{"4", "test4", "type1"},
	}

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	_, err := mongoUtil.InsertMany(ctx, col, d)
	if err != nil {
		return
	}

	// act
	var result, total, _ = mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type1"}, &pagination.PaginationQuery{Offset: 0, Size: 1}, bson.M{"id": -1})

	// assert
	s.Equal(4, total)
	s.Equal(1, len(result))
	s.Equal("4", result[0].ID)
}

func (s *MongoUtilTestSuite) Test_InsertOne() {
	d := &doc{"1", "test1", "type1"}

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	_, _ = mongoUtil.InsertOne(ctx, col, d)

	// act
	var err error
	result, total, err := mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type1"}, nil, nil)

	// assert
	s.Equal(1, total)
	s.Equal(1, len(result))

	s.Nil(err)
}

func (s *MongoUtilTestSuite) Test_Update() {
	d := &doc{"1", "test1", "type1"}

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	_, _ = mongoUtil.InsertOne(ctx, col, d)

	// act
	d.Name = "test2"
	ret, _ := mongoUtil.Update(ctx, col, d)

	// assert
	result, total, err := mongoUtil.FindList[doc](ctx, col, bson.M{"type": "type1"}, nil, nil)

	s.Equal(1, total)
	s.Equal(1, len(result))
	s.Equal(d.Name, result[0].Name)
	s.Equal(int64(1), ret.ModifiedCount)

	s.Nil(err)
}

func (s *MongoUtilTestSuite) Test_FindByID_ReturnNil() {

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	// act
	result, total, err := mongoUtil.FindList[*doc](ctx, col, nil, nil, nil)
	result2, total2, err2 := mongoUtil.FindList[doc](ctx, col, nil, nil, nil)

	// assert
	s.Len(result, 0)
	s.Nil(err)
	s.Empty(total)

	// it will return an empty object which the fields are all zero value
	s.Len(result2, 0)
	s.Nil(err2)
	s.Empty(total2)
}

func (s *MongoUtilTestSuite) Test_FindByID_WhenEmpty_ShouldReturnNil() {

	randomCollectionName := uuid.NewString()
	col := s.DbDoc.Collection(randomCollectionName)
	ctx := context.Background()

	// act
	result, err := mongoUtil.FindByID[*doc](ctx, col, "1")
	result2, err2 := mongoUtil.FindByID[doc](ctx, col, "1")

	// assert
	s.Nil(result)
	s.Nil(err)

	// it will return an empty object which the fields are all zero value
	// the behaviour is not same as when we give *doc to FindByID
	s.Empty(result2.Type)
	s.Empty(result2.ID)
	s.Empty(result2.Name)
	s.Error(err2, "type must be a pointer")
}

func (s *MongoUtilTestSuite) Test_FindByID_Reflect() {
	var d *doc

	reflected := reflect.ValueOf(d)
	ptrVal := reflect.TypeOf(d)

	ptrVal.Kind()

	fmt.Printf("reflected: %v\n", reflected)
}

func Test_ClassifyModifications(t *testing.T) {
	type Entity struct {
		ID   string `pk:"true"`
		Name string
	}

	originalEntities := []*Entity{
		{ID: "1", Name: "John"},
		{ID: "2", Name: "Jane"},
		{ID: "3", Name: "Bob"}, // delete
	}

	updateEntities := []*Entity{
		{ID: "1", Name: "Johnny"}, // update
		{ID: "2", Name: "Jane"},   // no change still in updateEntities
		{ID: "", Name: "Alice"},   // create
	}

	created, updated, removed, err := mongoUtil.ClassifyModifications[*Entity](originalEntities, updateEntities)
	require.NoError(t, err)

	expectedCreated := []*Entity{
		{ID: "", Name: "Alice"},
	}

	expectedUpdated := []*Entity{
		{ID: "1", Name: "Johnny"},
		{ID: "2", Name: "Jane"},
	}

	expectedRemoved := []*Entity{
		{ID: "3", Name: "Bob"},
	}

	require.Equal(t, expectedCreated, created)
	require.Equal(t, expectedUpdated, updated)
	require.Equal(t, expectedRemoved, removed)
}

func Test_ClassifyModificationsWithEmptyOriginalEntities(t *testing.T) {
	type Entity struct {
		ID   string `pk:"true"`
		Name string
	}

	originalEntities := []*Entity{}

	updateEntities := []*Entity{
		{ID: "1", Name: "Johnny"}, // update
		{ID: "", Name: "Alice"},   // create
	}

	created, updated, removed, err := mongoUtil.ClassifyModifications[*Entity](originalEntities, updateEntities)
	require.NoError(t, err)

	expectedCreated := []*Entity{
		{ID: "", Name: "Alice"},
	}
	expectedUpdated := []*Entity{
		{ID: "1", Name: "Johnny"},
	}
	expectedRemoved := []*Entity{}

	require.Equal(t, expectedCreated, created)
	require.Equal(t, expectedUpdated, updated)
	require.Equal(t, expectedRemoved, removed)
}
