package query

import "context"

type Total int64
type IQueryRepository interface {
	QueryDecorations(ctx context.Context, tags []string, offset, size int) ([]*DecorationItem, Total, error)
	QueryCategory(ctx context.Context, offset int, size int) ([]*CategoryItem, Total, error)
}
