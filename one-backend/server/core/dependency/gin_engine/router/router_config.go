package router

import (
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/gin-gonic/gin"

	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/arch/port/gin/middleware"
	"xrspace.io/server/core/dependency/auth_service"
	"xrspace.io/server/core/dependency/database"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/settings"
)

type RouterGroup struct {
	ApiGroup       gin.IRouter
	PublicApiGroup gin.IRouter
	CmsGroup       gin.IRouter
}

type IRouter interface {
	RegisterRoutes(routerGroup RouterGroup)
}

type Dependencies struct {
	Config                  *settings.Config
	DB                      *docdb.DocDB
	AuthService             *auth_service.AuthService
	AwsSession              *session.Session
	AuthorizationMiddleware *middleware.AuthorizationMiddleware
	EventBus                eventbus.IEventBus
	Redis                   database.IRedis
}
