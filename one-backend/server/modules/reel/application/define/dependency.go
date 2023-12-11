package define

import (
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/reel/domain/repository"
)

type Dependency struct {
	UnitOfWork unit_of_work.IUnitOfWork
	ReelRepo   repository.IReelRepository
	QueryRepo  IQueryRepository
	EventBus   eventbus.IEventBus
}
