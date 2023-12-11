package define

import (
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/room/domain"
	"xrspace.io/server/modules/room/domain/repository"
)

type Dependency struct {
	Repo           repository.IRoomRepository
	QueryRepo      IQueryRepository
	Bus            eventbus.IEventBus
	UnitOfWork     unit_of_work.IUnitOfWork
	TokenValidator domain.IPhotonAccessTokenValidator
}
