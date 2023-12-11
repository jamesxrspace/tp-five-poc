package define

import (
	"context"

	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/enum"
)

type GetLikeFilter struct {
	XrID       string
	TargetType enum.TargetType
	TargetID   string
}

type GetLikeReactionResult struct {
	IsLike bool `bson:"is_like"`
	Count  int  `bson:"count"`
}

type IQueryRepository interface {
	GetLike(ctx context.Context, filter GetLikeFilter) (*entity.Like, error)
	GetLikeReaction(ctx context.Context, filter GetLikeFilter) (*GetLikeReactionResult, error)
	IsFeedExist(ctx context.Context, feedID string) bool
}
