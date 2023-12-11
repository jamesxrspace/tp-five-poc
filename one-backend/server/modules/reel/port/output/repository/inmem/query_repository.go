package inmem

import (
	"context"

	assetEntity "xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/domain/entity"
)

var _ define.IQueryRepository = (*QueryRepository)(nil)

type QueryRepository struct {
	Categories map[string]*assetEntity.CategoryItem
	Reels      map[string]*entity.Reel
}

func NewQueryRepository(
	categories map[string]*assetEntity.CategoryItem,
	reels map[string]*entity.Reel,
) *QueryRepository {
	if categories == nil {
		categories = make(map[string]*assetEntity.CategoryItem)
	}
	if reels == nil {
		reels = make(map[string]*entity.Reel)
	}
	return &QueryRepository{
		Categories: categories,
		Reels:      reels,
	}
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
