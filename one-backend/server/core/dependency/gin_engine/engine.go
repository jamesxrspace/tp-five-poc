package gin_engine

import (
	"context"
	"net/http"
	"os"
	"reflect"
	"time"

	"github.com/aws/aws-sdk-go/aws/session"
	sentrygin "github.com/getsentry/sentry-go/gin"
	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog/log"
	"go.opentelemetry.io/contrib/instrumentation/github.com/gin-gonic/gin/otelgin"

	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/core/arch/port/gin/middleware"
	"xrspace.io/server/core/dependency/auth_service"
	"xrspace.io/server/core/dependency/database"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/core/dependency/settings"
	accountInput "xrspace.io/server/modules/account/port/input/controller"
	aigcInput "xrspace.io/server/modules/aigc/port/input/router"
	assetInput "xrspace.io/server/modules/asset/port/input/controller"
	avatarInput "xrspace.io/server/modules/avatar/port/input/controller"
	dailyBuildInput "xrspace.io/server/modules/daily_build/port/input/gin"
	feedInput "xrspace.io/server/modules/feed/port/input/controller"
	reactionInput "xrspace.io/server/modules/reaction/port/input/controller"
	reelInput "xrspace.io/server/modules/reel/port/input/controller"
	roomInput "xrspace.io/server/modules/room/port/input/gin"
	spaceInput "xrspace.io/server/modules/space/port/input/gin"
	streamingInput "xrspace.io/server/modules/streaming/port/input/controller"
)

type GinEngine struct {
	*gin.Engine
	dependencies *router.Dependencies
}

func NewGinEngine(
	config *settings.Config,
	db *docdb.DocDB,
	authService *auth_service.AuthService,
	awsSession *session.Session,
	eventBus eventbus.IEventBus,
	redis database.IRedis,
) *GinEngine {
	// create the customized gin engine
	engine := gin.New()
	engine.Use(
		// recover from any panics and return 500
		gin.Recovery(),
	)

	return &GinEngine{
		Engine: engine,
		dependencies: &router.Dependencies{
			Config:      config,
			DB:          db,
			AuthService: authService,
			AwsSession:  awsSession,
			EventBus:    eventBus,
			AuthorizationMiddleware: middleware.NewAuthorizationMiddleware(
				authService,
				db,
				config.App,
			),
			Redis: redis,
		},
	}
}

func (e *GinEngine) setMiddleware() {
	// setup the sentry middleware
	e.Use(sentrygin.New(sentrygin.Options{
		Repanic: true,
	}))

	// setup otel gin middleware
	e.Use(otelgin.Middleware(e.dependencies.Config.Otel.AppName))

	e.Use(middleware.SetApplicationLogContext())

	// setup the cors middleware in local dev
	if e.dependencies.Config.Env == "local" {
		e.Use(cors.New(e.corsConfig()))
	}
}

func (e *GinEngine) corsConfig() cors.Config {
	corsConf := cors.DefaultConfig()
	corsConf.AllowAllOrigins = true
	corsConf.AllowMethods = []string{"*"}
	corsConf.AllowHeaders = []string{"*"}
	corsConf.AllowCredentials = true
	return corsConf
}

func (e *GinEngine) Serve(ctx context.Context) error {
	e.setMiddleware()
	// set up routes here
	e.registerBasicEndpoint()
	e.registerRouters([]router.IRouter{
		accountInput.NewAccountRouter(e.dependencies),
		avatarInput.NewAvatarRouter(e.dependencies),
		feedInput.NewFeedRouter(e.dependencies),
		roomInput.NewRoomRouter(e.dependencies),
		assetInput.NewAssetRouter(e.dependencies),
		aigcInput.NewAvatarRouter(e.dependencies),
		streamingInput.NewStreamingRouter(e.dependencies),
		spaceInput.NewSpaceRouter(e.dependencies),
		dailyBuildInput.NewDailyBuildRouter(e.dependencies),
		reactionInput.NewReactionRouter(e.dependencies),
		reelInput.NewReelRouter(e.dependencies),
	})

	srv := &http.Server{
		Addr:    e.dependencies.Config.App.Port,
		Handler: e.Engine,
	}

	// start server
	go func() {
		if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Error().Err(err).Msg("failed to start server")
			os.Exit(1)
		}
	}()

	log.Info().Str("port", e.dependencies.Config.App.Port).Msg("server started")

	<-ctx.Done()

	shutdownCtx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	if err := srv.Shutdown(shutdownCtx); err != nil {
		srv.Close()
		return err
	}

	log.Info().Msg("server gracefully stopped")
	return nil
}

func (e *GinEngine) registerRouters(routers []router.IRouter) {
	if len(routers) == 0 {
		log.Warn().Msgf("[CustomRouter] custom module is empty")
		return
	}

	routerGroup := router.RouterGroup{
		ApiGroup:       e.Group("/api"),
		PublicApiGroup: e.Group("/api"),
		CmsGroup:       e.Group("/_cms"),
	}
	// setup auth access tocken middleware for endpoint under api group
	authMiddleware := middleware.NewAuthorizationMiddleware(
		e.dependencies.AuthService,
		e.dependencies.DB,
		e.dependencies.Config.App,
	)

	routerGroup.ApiGroup.Use(authMiddleware.ValidateAccessToken())
	routerGroup.ApiGroup.Use(authMiddleware.SetXrID())
	routerGroup.ApiGroup.Use(authMiddleware.SetAppID())

	routerGroup.CmsGroup.Use(authMiddleware.ValidateAccessToken())
	routerGroup.CmsGroup.Use(authMiddleware.SetXrID())
	routerGroup.CmsGroup.Use(authMiddleware.SetAppID())

	for _, r := range routers {
		r.RegisterRoutes(routerGroup)
		log.Info().Msgf("[CustomRouter] register module %s successfully", reflect.TypeOf(r))
	}
}

// register the basic API endpoint, like /readyz and /livez
// ref: https://kubernetes.io/docs/reference/using-api/health-checks/
func (e *GinEngine) registerBasicEndpoint() {
	// /livez     the basic API endpoint and always return ok
	e.Engine.GET("/livez", func(c *gin.Context) {
		c.String(http.StatusOK, "ok")
	})

	// /readyz      the health check API endpoint
	e.Engine.GET("/readyz", func(c *gin.Context) {
		// check the database connection
		if err := e.dependencies.DB.Ping(c.Request.Context()); err != nil {
			log.Warn().Err(err).Msg("database connection error")
			c.String(http.StatusInternalServerError, "database connection error")
			return
		}

		// check the redis connection
		if err := e.dependencies.Redis.Ping(c.Request.Context()); err != nil {
			log.Warn().Err(err).Msg("redis connection error")
			c.String(http.StatusInternalServerError, "redis connection error")
			return
		}
		c.String(http.StatusOK, "ok")
	})
}
