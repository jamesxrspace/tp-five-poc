package define

import (
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/social/domain"
)

type Dependency struct {
	UnitOfWork unit_of_work.IUnitOfWork
	FollowRepo domain.IFollowRepository
	QueryRepo  IQueryRepository
}
