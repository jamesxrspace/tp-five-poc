package repository

import (
	"context"

	"xrspace.io/server/core/arch/domain/repository"
	"xrspace.io/server/modules/reel/domain/entity"
)

type IReelRepository interface {
	Save(ctx context.Context, item *entity.Reel) error
	Get(ctx context.Context, id string) (*entity.Reel, error)
}

var _ repository.IDomainRepository[*entity.Reel] = (IReelRepository)(nil)
