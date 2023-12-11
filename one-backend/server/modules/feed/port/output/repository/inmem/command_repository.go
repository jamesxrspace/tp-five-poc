package inmem

import (
	"context"

	"xrspace.io/server/modules/feed/domain/entity"
)

func NewFeedRepository(feeds map[string]*entity.Feed) *FeedRepository {
	if feeds == nil {
		feeds = make(map[string]*entity.Feed)
	}
	return &FeedRepository{Feeds: feeds}
}

func NewReelRepository(reels map[string]*entity.Reel) *ReelRepository {
	if reels == nil {
		reels = make(map[string]*entity.Reel)
	}
	return &ReelRepository{Reels: reels}
}

type FeedRepository struct {
	Feeds map[string]*entity.Feed
}

func (r *FeedRepository) Get(ctx context.Context, id string) (*entity.Feed, error) {
	return r.Feeds[id], nil
}

func (r *FeedRepository) Save(ctx context.Context, item *entity.Feed) error {
	r.Feeds[item.ID] = item
	return nil
}

type ReelRepository struct {
	Reels map[string]*entity.Reel
}

func (r *ReelRepository) Get(ctx context.Context, id string) (*entity.Reel, error) {
	return r.Reels[id], nil
}

func (r *ReelRepository) Save(ctx context.Context, item *entity.Reel) error {
	r.Reels[item.ID] = item
	return nil
}
