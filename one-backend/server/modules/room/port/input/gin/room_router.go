package gin

import (
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/modules/room/application"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/application/query"
	"xrspace.io/server/modules/room/port/output/repo/mongo"
	roomProxy "xrspace.io/server/modules/room/port/output/token_validator/proxy"
)

type RoomRouter struct {
	dependencies *router.Dependencies
	facade       *application.Facade
}

func NewRoomRouter(dependencies *router.Dependencies) *RoomRouter {
	return &RoomRouter{
		dependencies: dependencies,
	}
}

var _ router.IRouter = (*RoomRouter)(nil)

func (r *RoomRouter) initHandler(conf *router.Dependencies) *GinInputPort {
	validator := roomProxy.NewPhotonTokenValidator(conf.AuthService)

	roomRepo := mongo.NewRoomRepo(conf.DB)

	unitOfWork := docdb.NewUnitOfWork(*conf.DB)

	queryRepo := mongo.NewRoomQueryRepo(conf.DB)

	dep := &define.Dependency{
		Repo:           roomRepo,
		Bus:            conf.EventBus,
		UnitOfWork:     unitOfWork,
		TokenValidator: validator,
		QueryRepo:      queryRepo,
	}
	facade := application.NewFacade(dep)
	r.facade = facade

	handler := NewGinInputPort(facade)

	return handler
}

func (r *RoomRouter) RegisterRoutes(g router.RouterGroup) {
	handler := r.initHandler(r.dependencies)

	v1 := g.PublicApiGroup.Group("/v1")
	photon := v1.Group("/photon")
	photon.POST("/fusion_custom_auth", handler.FusionCustomAuth)

	room := g.ApiGroup.Group("/v1/room")
	room.POST("/join", handler.JoinRoom)
	room.POST("/leave", handler.LeaveRoom)
	room.GET("/all", handler.AllRoomData)
	room.GET("/:id", handler.GetRoomByID)

	cms := g.CmsGroup.Group("/v1/room").Use(r.dependencies.AuthorizationMiddleware.ValidateAccessToken())
	cms.GET("/list", portGin.HandlerFunc[query.ListRoomBySpaceIDQuery](r.facade))
}
