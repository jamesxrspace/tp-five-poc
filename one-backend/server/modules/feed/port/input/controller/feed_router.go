package controller

import (
	"xrspace.io/server/core/arch/domain/event"
	portEventBus "xrspace.io/server/core/arch/port/eventbus"
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/modules/feed/application"
	"xrspace.io/server/modules/feed/application/command"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/application/query"
	"xrspace.io/server/modules/feed/port/output/repository/mongo"
)

var _ router.IRouter = (*FeedRouter)(nil)

type FeedRouter struct {
	dependencies *router.Dependencies
	facade       *application.Facade
}

func NewFeedRouter(dependencies *router.Dependencies) *FeedRouter {
	dep := define.Dependency{
		EventBus:   dependencies.EventBus,
		UnitOfWork: docdb.NewUnitOfWork(*dependencies.DB),
		FeedRepo:   mongo.NewFeedRepository(dependencies.DB),
		QueryRepo:  mongo.NewQueryRepository(dependencies.DB),
	}

	return &FeedRouter{
		facade:       application.NewFacade(dep),
		dependencies: dependencies,
	}
}

func (r *FeedRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")

	feed := v1.Group("/feed")
	feed.Use(
		r.dependencies.AuthorizationMiddleware.ValidateAccessToken(),
		r.dependencies.AuthorizationMiddleware.SetXrID(),
		r.dependencies.AuthorizationMiddleware.SetAppID(),
	)
	feed.POST("/create", portGin.HandlerFunc[command.CreateFeedCommand](r.facade))
	feed.GET("/lobby", portGin.HandlerFunc[query.ListFeedQuery](r.facade))

	err := r.facade.EventBus.Subscribe(event.ReelPublishedEvent, portEventBus.HandlerFunc[*command.CreateFeedCommand](r.facade))

	if err != nil {
		panic(err)
	}
}
