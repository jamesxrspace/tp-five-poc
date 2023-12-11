package repository

import (
	"context"

	"xrspace.io/server/modules/space/domain/entity"
)

type ISpaceRepository interface {
	Create(ctx context.Context, space *entity.Space) (*entity.Space, error)
	FindById(ctx context.Context, spaceId string) (*entity.Space, error)
	FindByIds(ctx context.Context, spaceId []string) ([]*entity.Space, error)
	Save(ctx context.Context, space *entity.Space) error
	SaveMany(ctx context.Context, space []*entity.Space) error
	FindBySpaceGroupId(ctx context.Context, spaceGroupId string) ([]*entity.Space, error)
}
