package query

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
)

type IQuerySpaceRepository interface {
	List(ctx context.Context, paginationParams pagination.PaginationQuery, filterParams ListSpaceFilter) (*ListSpaceResponse, error)
}
