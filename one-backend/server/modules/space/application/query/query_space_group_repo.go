package query

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
)

type IQuerySpaceGroupRepository interface {
	List(ctx context.Context, paginationParams pagination.PaginationQuery, filterParams ListSpaceGroupFilter) (*ListSpaceGroupResponse, error)
}
