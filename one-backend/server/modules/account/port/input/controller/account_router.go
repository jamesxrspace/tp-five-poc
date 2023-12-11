package controller

import (
	"fmt"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog"
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/modules/account/application"
	"xrspace.io/server/modules/account/application/command"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/application/query"
	"xrspace.io/server/modules/account/port/output/auth_proxy/auth"
	"xrspace.io/server/modules/account/port/output/repository/mongo"
)

var _ router.IRouter = (*AccountRouter)(nil)

type AccountRouter struct {
	dependencies *router.Dependencies
	facade       *application.Facade
}

func NewAccountRouter(dependencies *router.Dependencies) *AccountRouter {
	dep := define.Dependency{
		AuthProxy:   auth.NewAuthProxy(dependencies.AuthService),
		Config:      dependencies.Config,
		RedisClient: dependencies.Redis,
		AccountRepo: mongo.NewAccountRepository(dependencies.DB),
		QueryRepo:   mongo.NewQueryRepository(dependencies.DB),
	}

	return &AccountRouter{
		dependencies: dependencies,
		facade:       application.NewFacade(dep),
	}
}

func (r *AccountRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.PublicApiGroup.Group("/v1")
	auth := v1.Group("/auth")
	auth.GET("/authing/authn/:os/:client_id", r.redirect)
	auth.GET("/authing/oidc/:package_id", r.redirectAuthCode)
	auth.GET("/authing/oidc/authtest", r.redirectAuthTest)
	auth.POST("/authing/guest", portGin.HandlerFunc[command.CreateGuestCommand](r.facade))

	account := v1.Group("/account")
	account.Use(r.dependencies.AuthorizationMiddleware.ValidateAccessToken())
	account.POST("/login", portGin.HandlerFunc[command.LoginCommand](r.facade))

	accountLoggedIn := account.Group("")
	accountLoggedIn.Use(r.dependencies.AuthorizationMiddleware.SetXrID())
	accountLoggedIn.GET("/profile", portGin.HandlerFunc[query.GetProfileQuery](r.facade))
}

func (r *AccountRouter) redirect(ctx *gin.Context) {
	log := zerolog.Ctx(ctx.Request.Context())
	log.Info().Msg("Auth_redirect_url: " + ctx.Request.URL.Path)
	os := ctx.Param("os")
	clientID := ctx.Param("client_id")
	rawQuery := ctx.Request.URL.RawQuery
	ctx.Redirect(http.StatusFound, fmt.Sprintf(r.dependencies.Config.OIDC.RedirectUrl, os, clientID, rawQuery))
}

func (r *AccountRouter) redirectAuthCode(ctx *gin.Context) {
	log := zerolog.Ctx(ctx.Request.Context())
	log.Info().Msg("Auth_code_redirect_url: " + ctx.Request.URL.Path)
	packageID := ctx.Param("package_id")
	rawQuery := ctx.Request.URL.RawQuery
	ctx.Redirect(http.StatusFound, fmt.Sprintf(r.dependencies.Config.OIDC.AuthCodeRedirectUrl, packageID, rawQuery))
}

func (r *AccountRouter) redirectAuthTest(ctx *gin.Context) {
	log := zerolog.Ctx(ctx.Request.Context())
	log.Info().Msg("redirectAuthTest: " + ctx.Request.URL.Path)
	ctx.Redirect(http.StatusFound, "xrspace://xrspace.auth.com")
}
