package controller

import (
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/modules/reel/application"
	"xrspace.io/server/modules/reel/application/command"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/application/query"
	"xrspace.io/server/modules/reel/port/output/repository/mongo"
)

var _ router.IRouter = (*ReelRouter)(nil)

type ReelRouter struct {
	dependencies *router.Dependencies
	facade       *application.Facade
}

func NewReelRouter(dependencies *router.Dependencies) *ReelRouter {
	dep := define.Dependency{
		EventBus:   dependencies.EventBus,
		UnitOfWork: docdb.NewUnitOfWork(*dependencies.DB),
		ReelRepo:   mongo.NewReelRepository(dependencies.DB),
		QueryRepo:  mongo.NewQueryRepository(dependencies.DB),
	}

	return &ReelRouter{
		facade:       application.NewFacade(dep),
		dependencies: dependencies,
	}
}

func (r *ReelRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")
	reel := v1.Group("/reel")
	reel.POST("/create", portGin.HandlerFunc[command.CreateReelCommand](r.facade))
	reel.POST("/publish/:reel_id", portGin.HandlerFunc[command.PublishReelCommand](r.facade))
	reel.DELETE("/:reel_id", portGin.HandlerFunc[command.DeleteReelCommand](r.facade))
	reel.GET("/list", portGin.HandlerFunc[query.ListReelQuery](r.facade))
}
