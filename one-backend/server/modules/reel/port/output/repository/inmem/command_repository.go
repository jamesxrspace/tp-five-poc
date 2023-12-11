package inmem

import (
	"context"

	"xrspace.io/server/modules/reel/domain/entity"
)

func NewReelRepository(reels map[string]*entity.Reel) *ReelRepository {
	if reels == nil {
		reels = make(map[string]*entity.Reel)
	}
	return &ReelRepository{Reels: reels}
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
