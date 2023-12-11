package controller

import (
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/modules/reaction/application"
	"xrspace.io/server/modules/reaction/application/command"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/application/query"
	"xrspace.io/server/modules/reaction/port/output/repository/mongo"
)

var _ router.IRouter = (*ReactionRouter)(nil)

type ReactionRouter struct {
	dependencies *router.Dependencies
	facade       *application.Facade
}

func NewReactionRouter(dependencies *router.Dependencies) *ReactionRouter {
	dep := define.Dependency{
		UnitOfWork: docdb.NewUnitOfWork(*dependencies.DB),
		LikeRepo:   mongo.NewLikeRepository(dependencies.DB),
		QueryRepo:  mongo.NewQueryRepository(dependencies.DB),
	}
	return &ReactionRouter{
		dependencies: dependencies,
		facade:       application.NewFacade(dep),
	}
}

func (r *ReactionRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")
	reaction := v1.Group("/reaction")
	reaction.Use(
		r.dependencies.AuthorizationMiddleware.ValidateAccessToken(),
		r.dependencies.AuthorizationMiddleware.SetXrID(),
		r.dependencies.AuthorizationMiddleware.SetAppID(),
	)

	reaction.POST("/like/:target_type/:target_id", portGin.HandlerFunc[command.LikeCommand](r.facade))
	reaction.GET("/like/:target_type/:target_id", portGin.HandlerFunc[query.GetLikeQuery](r.facade))
}
