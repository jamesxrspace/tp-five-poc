package define

import (
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/reaction/domain/repository"
)

type Dependency struct {
	UnitOfWork unit_of_work.IUnitOfWork
	LikeRepo   repository.ILikeRepository
	QueryRepo  IQueryRepository
}
