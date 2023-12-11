package inmem

import (
	"context"

	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/domain/entity"
)

var _ define.IQueryRepository = (*QueryRepository)(nil)

type QueryRepository struct {
	Likes map[interface{}]*entity.Like
}

func NewQueryRepository(likes map[interface{}]*entity.Like) *QueryRepository {
	if likes == nil {
		likes = make(map[interface{}]*entity.Like)
	}

	return &QueryRepository{Likes: likes}
}

func (r *QueryRepository) GetLike(ctx context.Context, filter define.GetLikeFilter) (*entity.Like, error) {
	for _, like := range r.Likes {
		if like.XrID == filter.XrID && like.TargetID == filter.TargetID && like.TargetType == filter.TargetType {
			return like, nil
		}
	}
	return nil, nil
}

func (r *QueryRepository) GetLikeReaction(ctx context.Context, filter define.GetLikeFilter) (*define.GetLikeReactionResult, error) {
	isLiked := false
	result := make([]*entity.Like, 0, len(r.Likes))

	for _, like := range r.Likes {
		if like.TargetID == filter.TargetID && like.TargetType == filter.TargetType && like.IsLike() {
			result = append(result, like)

			if like.XrID == filter.XrID {
				isLiked = true
			}
		}
	}

	return &define.GetLikeReactionResult{
		IsLike: isLiked,
		Count:  len(result),
	}, nil
}

func (r *QueryRepository) IsFeedExist(ctx context.Context, feedID string) bool {
	return feedID == "feed_id1"
}
