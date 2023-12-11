package repository

import (
	"context"

	"xrspace.io/server/modules/space/domain/entity"
)

type ISpaceGroupRepository interface {
	Create(ctx context.Context, spaceGroup *entity.SpaceGroup) (*entity.SpaceGroup, error)
	FindById(ctx context.Context, spaceGroupId string) (*entity.SpaceGroup, error)
	Save(ctx context.Context, spaceGroup *entity.SpaceGroup) error
}
