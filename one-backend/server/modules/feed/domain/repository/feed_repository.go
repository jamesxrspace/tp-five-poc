package repository

import (
	"context"

	"xrspace.io/server/core/arch/domain/repository"
	"xrspace.io/server/modules/feed/domain/entity"
)

type IFeedRepository interface {
	Save(ctx context.Context, item *entity.Feed) error
	Get(ctx context.Context, id string) (*entity.Feed, error)
}

var _ repository.IDomainRepository[*entity.Feed] = (IFeedRepository)(nil)
