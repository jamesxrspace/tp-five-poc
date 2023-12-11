package define

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/domain/entity"
)

type ListFeedCategoryFilter struct {
	Type       enum.CategoryType
	TitleI18ns []string
}

type ListFeedFilter struct {
	Status     string   `bson:"status"`
	XrIDs      []string `bson:"xrid"`
	Categories []string `bson:"categories"`
	pagination.PaginationQuery
}

type ListFeedResult struct {
	Items []*entity.Feed `bson:"items"`
	Total int            `bson:"total"`
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
	GetFeedByRef(_ context.Context, feedType string, refID string) (*entity.Feed, error)
	ListFeedCategory(_ context.Context, filter *ListFeedCategoryFilter) ([]*assetEntity.CategoryItem, error)
	ListReel(_ context.Context, filter *ListReelFilter) (*ListReelResult, error)
	ListFeed(ctx context.Context, filter *ListFeedFilter) (*ListFeedResult, error)
	GetNicknames(ctx context.Context, xrIds []string) (map[string]string, error)
	GetDemoXrids(ctx context.Context, xrID string) []string
}
