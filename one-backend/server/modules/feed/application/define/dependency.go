package define

import (
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/feed/domain/repository"
)

type Dependency struct {
	UnitOfWork unit_of_work.IUnitOfWork
	FeedRepo   repository.IFeedRepository
	ReelRepo   repository.IReelRepository
	QueryRepo  IQueryRepository
	EventBus   eventbus.IEventBus
}
