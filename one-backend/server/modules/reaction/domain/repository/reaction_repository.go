package repository

import (
	"context"

	"xrspace.io/server/core/arch/domain/repository"
	"xrspace.io/server/modules/reaction/domain/entity"
)

var _ repository.IDomainRepository[*entity.Like] = (ILikeRepository)(nil)

type ILikeRepository interface {
	Save(ctx context.Context, item *entity.Like) error
	Get(ctx context.Context, id string) (*entity.Like, error)
}
