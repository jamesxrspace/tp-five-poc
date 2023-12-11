package mongo

import (
	"context"
	"fmt"
	"reflect"

	"go.mongodb.org/mongo-driver/mongo"

	"xrspace.io/server/core/arch/core_error"
)

type GenericMongoRepository[T any] struct {
	collection *mongo.Collection
}

func NewGenericMongoRepository[T any](collection *mongo.Collection) *GenericMongoRepository[T] {
	var t T
	if reflect.TypeOf(t).Kind() != reflect.Ptr {
		panic(fmt.Sprintf("%s type must be a pointer", reflect.TypeOf(t).Name()))
	}

	return &GenericMongoRepository[T]{collection: collection}
}

func (g *GenericMongoRepository[T]) Save(ctx context.Context, item T) error {
	_, err := Save(ctx, g.collection, item)

	if err != nil {
		return core_error.StackError(err)
	}

	return nil
}

func (g *GenericMongoRepository[T]) Get(ctx context.Context, id string) (result T, err error) {
	result, err = FindByID[T](ctx, g.collection, id)

	if err != nil {
		var r T
		return r, core_error.StackError(err)
	}

	return result, nil
}
