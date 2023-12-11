package query

import (
	"context"
	"slices"
	"time"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/enum"
)

var _ application.IUseCase = (*ListFeedUseCase)(nil)

type ListFeedQuery struct {
	UserXrID   string   `form:"user_xrid" validate:"required" token:"xrid"`
	Categories []string `form:"categories"`
	pagination.PaginationQuery
}

type ResponseFeed struct {
	UpdatedAt     time.Time `json:"updated_at"`
	Content       *Content  `json:"content"`
	ID            string    `json:"id"`
	OwnerXrID     string    `json:"owner_xrid"`
	OwnerNickname string    `json:"owner_nickname"`
	Type          string    `json:"type"`
	Categories    []string  `json:"categories"`
}

type Content struct {
	RefID        string `json:"ref_id"`
	ThumbnailUrl string `json:"thumbnail_url"`
}

type ListFeedResponse struct {
	pagination.PaginationResponse[ResponseFeed]
}

type ListFeedUseCase struct {
	dep define.Dependency
}

func NewListFeedUseCase(dep define.Dependency) *ListFeedUseCase {
	return &ListFeedUseCase{
		dep: dep,
	}
}

func (u *ListFeedUseCase) Execute(ctx context.Context, query any) (any, error) {
	q := query.(*ListFeedQuery)

	err := u.validateCategories(ctx, q.Categories)
	if err != nil {
		return nil, core_error.NewCodeError(core_error.ValidationErrCode, err)
	}

	result, err := u.dep.QueryRepo.ListFeed(ctx, &define.ListFeedFilter{
		XrIDs:           u.getDemoXrids(ctx, q.UserXrID),
		Categories:      q.Categories,
		Status:          enum.FeedStatusActive,
		PaginationQuery: q.PaginationQuery,
	})
	if err != nil {
		return nil, err
	}

	if result == nil {
		return pagination.PaginationResponse[*ResponseFeed]{
			Items: []*ResponseFeed{},
			Total: 0,
		}, nil
	}

	items, err := u.genResponseFeeds(ctx, result.Items)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	return pagination.PaginationResponse[*ResponseFeed]{
		Items: items,
		Total: result.Total,
	}, nil
}

func (u *ListFeedUseCase) validateCategories(ctx context.Context, categories []string) error {
	result, err := u.dep.QueryRepo.ListFeedCategory(ctx, &define.ListFeedCategoryFilter{
		Type:       assetEnum.CateFeed,
		TitleI18ns: categories,
	})

	if len(result) == 0 {
		return core_error.StackError("feed category not found")
	}

	return err
}

func (u *ListFeedUseCase) getDemoXrids(ctx context.Context, xrID string) []string {
	return u.dep.QueryRepo.GetDemoXrids(ctx, xrID)
}

func (u *ListFeedUseCase) genResponseFeeds(ctx context.Context, feeds []*entity.Feed) ([]*ResponseFeed, error) {
	nicknames, err := u.getNicknames(ctx, feeds)
	if err != nil {
		return nil, err
	}

	result := make([]*ResponseFeed, 0, len(feeds))

	for _, feed := range feeds {
		result = append(result, &ResponseFeed{
			ID:            feed.ID,
			OwnerXrID:     feed.XrID,
			OwnerNickname: nicknames[feed.XrID],
			Type:          feed.Type,
			Content: &Content{
				RefID:        feed.RefID,
				ThumbnailUrl: feed.ThumbnailUrl,
			},
			Categories: feed.Categories,
			UpdatedAt:  feed.UpdatedAt,
		})
	}

	return result, nil
}

func (u *ListFeedUseCase) getNicknames(ctx context.Context, feeds []*entity.Feed) (map[string]string, error) {
	uniqueXrIDs := getFeedsXrIDs(feeds)

	nicknames, err := u.dep.QueryRepo.GetNicknames(ctx, uniqueXrIDs)
	if err != nil {
		return nil, err
	}

	return nicknames, nil
}

func getFeedsXrIDs(feeds []*entity.Feed) []string {
	xrIDs := make([]string, 0, len(feeds))

	for _, feed := range feeds {
		xrIDs = append(xrIDs, feed.XrID)
	}

	return slices.Compact(xrIDs)
}
