package define

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/reel/domain/entity"
)

type ListFeedCategoryFilter struct {
	Type       enum.CategoryType
	TitleI18ns []string
}

type ListReelFilter struct {
	ReelID string
	XrID   string
	Status string
	pagination.PaginationQuery
}

type ListReelResult struct {
	Items []*entity.Reel `bson:"items"`
	Total int            `bson:"total"`
}

type IQueryRepository interface {
	ListFeedCategory(ctx context.Context, filter *ListFeedCategoryFilter) ([]*assetEntity.CategoryItem, error)
	ListReel(ctx context.Context, filter *ListReelFilter) (*ListReelResult, error)
}
