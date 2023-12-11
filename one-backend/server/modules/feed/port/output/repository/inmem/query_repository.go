package inmem

import (
	"context"
	"sort"

	"xrspace.io/server/core/arch/port/pagination"
	accountEntity "xrspace.io/server/modules/account/domain/entity"
	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
)

var _ define.IQueryRepository = (*QueryRepository)(nil)

type QueryRepository struct {
	Accounts   map[string]*accountEntity.Account
	Categories map[string]*assetEntity.CategoryItem
	Feeds      map[string]*entity.Feed
	Reels      map[string]*entity.Reel
}

func NewQueryRepository(
	accounts map[string]*accountEntity.Account,
	categories map[string]*assetEntity.CategoryItem,
	feeds map[string]*entity.Feed,
	reels map[string]*entity.Reel,
) *QueryRepository {
	if accounts == nil {
		accounts = make(map[string]*accountEntity.Account)
	}
	if categories == nil {
		categories = make(map[string]*assetEntity.CategoryItem)
	}
	if feeds == nil {
		feeds = make(map[string]*entity.Feed)
	}
	if reels == nil {
		reels = make(map[string]*entity.Reel)
	}
	return &QueryRepository{
		Accounts:   accounts,
		Categories: categories,
		Feeds:      feeds,
		Reels:      reels,
	}
}

func (r *QueryRepository) GetFeedByRef(_ context.Context, feedType string, refID string) (*entity.Feed, error) {
	for _, feed := range r.Feeds {
		if feed.Type != feedType {
			continue
		}
		if feed.RefID != refID {
			continue
		}
		return feed, nil
	}
	return nil, nil
}

func (r *QueryRepository) GetNicknames(ctx context.Context, xrIds []string) (map[string]string, error) {
	nicknames := make(map[string]string, len(xrIds))
	for _, xrID := range xrIds {
		account, ok := r.Accounts[xrID]
		if !ok {
			continue
		}
		nicknames[xrID] = account.Nickname
	}
	return nicknames, nil
}

func (r *QueryRepository) GetDemoXrids(ctx context.Context, xrID string) []string {
	relations := map[string][]string{
		"xrid1": {"xrid2", "xrid3"},
		"xrid2": {"xrid1", "xrid3"},
		"xrid3": {"xrid1", "xrid2"},
	}

	return relations[xrID]
}

func (r *QueryRepository) ListFeedCategory(_ context.Context, filter *define.ListFeedCategoryFilter) ([]*assetEntity.CategoryItem, error) {
	var categories []*assetEntity.CategoryItem
	for _, category := range r.Categories {
		if category.Type != filter.Type {
			continue
		}
		if len(filter.TitleI18ns) > 0 {
			var found bool
			for _, titleI18n := range filter.TitleI18ns {
				if category.TitleI18n == string(titleI18n) {
					found = true
					break
				}
			}
			if !found {
				continue
			}
		}
		categories = append(categories, category)
	}
	return categories, nil
}

func (r *QueryRepository) ListFeed(ctx context.Context, filter *define.ListFeedFilter) (*define.ListFeedResult, error) {
	filteredFeeds := make(map[string]*entity.Feed, len(r.Feeds))
	for _, feed := range r.Feeds {
		filteredFeeds[feed.ID] = feed
	}

	if filter != nil {
		filterFeeds(filteredFeeds, filter)
	}

	result := sortFeeds(filteredFeeds)

	return &define.ListFeedResult{
		Items: getPaginated(result, &filter.PaginationQuery),
		Total: len(result),
	}, nil
}

func (r *QueryRepository) ListReel(_ context.Context, filter *define.ListReelFilter) (*define.ListReelResult, error) {
	var reels []*entity.Reel
	for _, reel := range r.Reels {
		if filter != nil {
			if filter.ReelID != "" && reel.ID != filter.ReelID {
				continue
			}
			if filter.XrID != "" && reel.XrID != filter.XrID {
				continue
			}
			if filter.Status != "" && reel.Status != filter.Status {
				continue
			}
		}

		reels = append(reels, reel)
	}
	return &define.ListReelResult{
		Items: reels,
	}, nil
}

func filterFeeds(feeds map[string]*entity.Feed, filter *define.ListFeedFilter) map[string]*entity.Feed {
	if len(filter.XrIDs) > 0 {
		feeds = filterXrIDs(feeds, filter.XrIDs)
	}

	if len(filter.Categories) > 0 {
		feeds = filterCategories(feeds, filter.Categories)
	}

	if filter.Status != "" {
		for _, feed := range feeds {
			if feed.Status != filter.Status {
				delete(feeds, feed.ID)
			}
		}
	}
	return feeds
}

func filterXrIDs(feeds map[string]*entity.Feed, xrIDs []string) map[string]*entity.Feed {
	for _, feed := range feeds {
		var found bool
		for _, xrID := range xrIDs {
			if feed.XrID == xrID {
				found = true
				break
			}
		}
		if !found {
			delete(feeds, feed.ID)
		}
	}
	return feeds
}

func filterCategories(feeds map[string]*entity.Feed, categories []string) map[string]*entity.Feed {
	for _, feed := range feeds {
		var found bool
		for _, category := range categories {
			for _, feedCategory := range feed.Categories {
				if feedCategory == category {
					found = true
					break
				}
			}
			if found {
				break
			}
		}
		if !found {
			delete(feeds, feed.ID)
		}
	}
	return feeds
}

func sortFeeds(feeds map[string]*entity.Feed) []*entity.Feed {
	result := make([]*entity.Feed, 0, len(feeds))

	for _, feed := range feeds {
		result = append(result, feed)
	}

	sort.Slice(result, func(i, j int) bool {
		return result[i].UpdatedAt.After(result[j].UpdatedAt)
	})

	return result
}

func getPaginated(feeds []*entity.Feed, pagination *pagination.PaginationQuery) []*entity.Feed {
	if pagination == nil {
		return feeds
	}

	start := pagination.Skip()
	end := start + pagination.Limit()

	if end > len(feeds) {
		end = len(feeds)
	}

	return feeds[start:end]
}
