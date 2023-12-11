package docdb

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"xrspace.io/server/core/arch/port/pagination"
)

type MongoPagination[T any] struct {
	ctx             context.Context
	collections     *mongo.Collection
	filter          bson.M
	sort            bson.D
	project         bson.D
	lookup          bson.D
	paginationQuery pagination.PaginationQuery
}

func NewMongoPagination[T any](ctx context.Context, collections *mongo.Collection, filter bson.M, sort bson.D, project bson.D, lookup bson.D, paginationQuery pagination.PaginationQuery) *MongoPagination[T] {
	if filter == nil {
		filter = bson.M{}
	}
	if sort == nil {
		sort = bson.D{
			{Key: "_id", Value: 1},
		}
	}
	if project == nil {
		// will return all fields if project not given
		project = bson.D{
			{Key: "_field_should_not_exist", Value: 0},
		}
	}
	return &MongoPagination[T]{
		ctx:             ctx,
		filter:          filter,
		sort:            sort,
		project:         project,
		lookup:          lookup,
		paginationQuery: paginationQuery,
		collections:     collections,
	}
}

func (p *MongoPagination[T]) Decode(resp *pagination.PaginationResponse[T]) error {
	cur, err := p.collections.Aggregate(p.ctx, GetPaginateAggregation(
		p.paginationQuery.Skip(),
		p.paginationQuery.Limit(),
		p.filter,
		p.sort,
		p.project,
		p.lookup,
	))

	if err != nil {
		return err
	}
	if cur.Next(p.ctx) {
		if err := cur.Decode(&resp); err != nil {
			return err
		}
	}
	return nil
}
