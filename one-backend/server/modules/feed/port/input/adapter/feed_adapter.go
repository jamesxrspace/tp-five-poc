package adapter

import (
	"context"

	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/repository"
)

type FeedAdapter struct {
	repository repository.IFeedRepository
}

func NewFeedAdapter(repository repository.IFeedRepository) *FeedAdapter {
	return &FeedAdapter{
		repository: repository,
	}
}

func (a *FeedAdapter) GetFeed(ctx context.Context, feedID string) (*entity.Feed, error) {
	feed, err := a.repository.Get(ctx, feedID)
	if err != nil {
		return nil, err
	}

	return feed, nil
}
