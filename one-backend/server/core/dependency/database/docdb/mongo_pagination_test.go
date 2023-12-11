package docdb

import (
	"context"
	"testing"

	"github.com/stretchr/testify/suite"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"xrspace.io/server/core/arch/port/pagination"
)

func TestMongoPagination(t *testing.T) {
	s := new(TestSuite)
	suite.Run(t, s)
}

type TestSuite struct {
	suite.Suite
	InmemMongoBasicTestSuite
	collection *mongo.Collection
}

type mockItem struct {
	Identify string `bson:"_id,omitempty"`
	Name     string `bson:"name"`
	Age      int    `bson:"age"`
}

func (s *TestSuite) SetupTest() {
	s.InmemMongoBasicTestSuite.SetupTest()
	items := []interface{}{
		mockItem{Name: "test1", Age: 1},
		mockItem{Name: "test2", Age: 2},
		mockItem{Name: "test3", Age: 3},
		mockItem{Name: "test4", Age: 4},
		mockItem{Name: "test5", Age: 5},
	}
	s.collection = s.InmemMongoBasicTestSuite.DbDoc.Collection("pagination_test")
	s.collection.InsertMany(context.TODO(), items)

}

func (s *TestSuite) TestMongoPaginationDecode() {
	// arrange
	var result pagination.PaginationResponse[mockItem]
	result.Items = make([]mockItem, 0, 10)

	// act
	paginationQuery := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}
	p := NewMongoPagination[mockItem](context.TODO(), s.collection, nil, nil, nil, nil, paginationQuery)
	_ = p.Decode(&result)

	// assert
	s.Equal(2, len(result.Items))
	s.Equal(5, int(result.Total))

	// act
	paginationQuery = pagination.PaginationQuery{
		Offset: 2,
		Size:   2,
	}
	p = NewMongoPagination[mockItem](context.TODO(), s.collection, nil, nil, nil, nil, paginationQuery)
	_ = p.Decode(&result)

	// assert
	s.Equal(1, len(result.Items))
	s.Equal(5, int(result.Total))

	// act
	paginationQuery = pagination.PaginationQuery{
		Offset: 3,
		Size:   2,
	}
	p = NewMongoPagination[mockItem](context.TODO(), s.collection, nil, nil, nil, nil, paginationQuery)
	_ = p.Decode(&result)

	// assert
	s.Equal(0, len(result.Items))
	s.Equal(5, int(result.Total))

}

func (s *TestSuite) TestMongoPaginationDecodeWithFilter() {
	// arrange
	var result pagination.PaginationResponse[mockItem]
	result.Items = make([]mockItem, 0, 10)
	filter := bson.M{"age": bson.M{"$gt": 2}}

	// act
	paginationQuery := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}
	p := NewMongoPagination[mockItem](context.TODO(), s.collection, filter, nil, nil, nil, paginationQuery)
	_ = p.Decode(&result)

	// assert
	s.Equal(2, len(result.Items))
	s.Equal(3, int(result.Total))

	// act
	paginationQuery = pagination.PaginationQuery{
		Offset: 1,
		Size:   2,
	}
	p = NewMongoPagination[mockItem](context.TODO(), s.collection, filter, nil, nil, nil, paginationQuery)
	_ = p.Decode(&result)

	// assert
	s.Equal(1, len(result.Items))
	s.Equal(3, int(result.Total))

}

func (s *TestSuite) TestMongoPaginationDecodeWithSort() {
	// arrange
	var result pagination.PaginationResponse[mockItem]
	result.Items = make([]mockItem, 0, 10)
	sort := bson.D{
		{Key: "age", Value: -1},
	}

	// act
	paginationQuery := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}
	pagination := NewMongoPagination[mockItem](context.TODO(), s.collection, nil, sort, nil, nil, paginationQuery)
	_ = pagination.Decode(&result)

	// assert
	s.Equal(2, len(result.Items))
	s.Equal(5, int(result.Total))
	s.Equal(5, int(result.Items[0].Age))
	s.Equal(4, int(result.Items[1].Age))

}

func (s *TestSuite) TestMongoPaginationWithProject() {
	// arrange
	var result pagination.PaginationResponse[mockItem]
	result.Items = make([]mockItem, 0, 10)
	project := bson.D{
		{Key: "_id", Value: 0},
		{Key: "name", Value: 1},
	}

	// act
	paginationQuery := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}
	p := NewMongoPagination[mockItem](context.TODO(), s.collection, nil, nil, nil, nil, paginationQuery)
	_ = p.Decode(&result)

	// assert
	s.NotEmpty(result.Items[0].Identify)
	s.NotEmpty(result.Items[0].Name)
	s.NotEmpty(result.Items[0].Age)

	// act
	paginationQuery = pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}
	p = NewMongoPagination[mockItem](context.TODO(), s.collection, nil, nil, project, nil, paginationQuery)
	_ = p.Decode(&result)

	s.Empty(result.Items[0].Identify)
	s.NotEmpty(result.Items[0].Name)
	s.Empty(result.Items[0].Age)

}
