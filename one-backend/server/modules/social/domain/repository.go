package domain

import (
	"context"

	"xrspace.io/server/core/arch/domain/repository"
)

var _ repository.IDomainRepository[*Follow] = (IFollowRepository)(nil)

type IFollowRepository interface {
	Save(ctx context.Context, item *Follow) error
	Get(ctx context.Context, id string) (*Follow, error)
}
