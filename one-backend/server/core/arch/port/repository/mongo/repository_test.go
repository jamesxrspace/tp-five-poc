package mongo

import (
	"context"
	"testing"

	"github.com/google/uuid"
	"github.com/stretchr/testify/suite"

	"xrspace.io/server/core/dependency/database/docdb"
)

func TestRepositoryTestSuite(t *testing.T) {
	suite.Run(t, new(RepositoryTestSuite))
}

type RepositoryTestSuite struct {
	docdb.InmemMongoBasicTestSuite
}

type document struct {
	ID   string `bson:"id" pk:"true"`
	Name string `bson:"name"`
	Type string `bson:"type"`
}

func getRepo(s *RepositoryTestSuite) *GenericMongoRepository[*document] {
	randomCollectionName := "test_" + uuid.New().String()
	return NewGenericMongoRepository[*document](s.DbDoc.Collection(randomCollectionName))
}
func (s *RepositoryTestSuite) Test_Get() {
	// arrange
	doc := &document{ID: "1", Name: "test", Type: "test"}
	ctx := context.Background()
	repo := getRepo(s)

	err := repo.Save(ctx, doc)

	// act
	get, err2 := repo.Get(ctx, "1")

	// assert
	s.NoError(err)
	s.NoError(err2)
	s.Equal(doc, get)
}

func (s *RepositoryTestSuite) Test_WhenGetNotExisted_ShouldNil() {
	// arrange
	doc := &document{ID: "1", Name: "test", Type: "test"}
	ctx := context.Background()
	repo := getRepo(s)

	err := repo.Save(ctx, doc)

	// act
	get, err2 := repo.Get(ctx, "99")

	// assert
	s.NoError(err)
	s.NoError(err2)
	s.Nil(get)
}
func (s *RepositoryTestSuite) Test_NonPointer_ShouldError() {
	// arrange
	s.Panics(func() {
		NewGenericMongoRepository[document](s.DbDoc.Collection("test"))
	})
}
