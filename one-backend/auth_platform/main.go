package main

import (
	"auth_platform/core/middleware"
	"auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/port/input"
	"auth_platform/modules/authorization/port/output/file"
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/signal"
	"runtime/debug"
	"syscall"
	"time"

	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
	"github.com/joho/godotenv"
)

const (
	privateKeyPath = ".ssh/private_key.pem"
	port           = ":9453"
	cms            = "http://localhost:3000"
)

func main() {
	defer func() {
		err := recover()
		if err != nil {
			fmt.Printf("Executing main failed:%+v, Stack:%+v", err, string(debug.Stack()))
		}
	}()

	_ = godotenv.Load("../.env")
	mockAppID := os.Getenv("AZP")
	mockPoolID := os.Getenv("POOL_ID")
	mockSecret := os.Getenv("SECRET")

	jwtService := jwt.NewJwtService(privateKeyPath, mockAppID, mockPoolID, mockSecret)
	userRepo := file.NewUserRepositoryWithFakeData()
	authorizationController := input.NewAuthorizationController(jwtService, userRepo)
	middleware := middleware.NewAuthenticationMiddleware(jwtService)

	r := gin.Default()
	r.LoadHTMLFiles("pub/index.html")

	r.Use(cors.New(cors.Config{
		AllowOrigins:     []string{cms},
		AllowMethods:     []string{"*"},
		AllowHeaders:     []string{"*"},
		AllowCredentials: true,
		MaxAge:           time.Hour,
	}))

	r.GET("/health", func(c *gin.Context) {
		if userRepo.IsReady() {
			c.JSON(http.StatusOK, gin.H{"status": "ok"})
			return
		}
		c.JSON(http.StatusInternalServerError, gin.H{"status": "not ready"})
	})

	r.POST("/sign-up", authorizationController.CreateUser)
	r.GET("/sign-in", authorizationController.SignIn)

	oidc := r.Group("/oidc")
	oidc.POST("/token", middleware.GetOrRefreshToken(authorizationController.GetToken, authorizationController.RefreshToken))
	oidc.GET("/.well-known/jwks.json", authorizationController.GetJwks)
	oidc.Use(middleware.Authenticate())
	oidc.GET("/me", authorizationController.GetProfile)

	apiV3 := r.Group("/api/v3")
	apiV3.POST("/update-profile", middleware.Authenticate(), authorizationController.UpdateProfile)
	apiV3.Use(middleware.ManagerAuthenticate())
	apiV3.POST("/get-management-token", authorizationController.GetManagerToken)
	apiV3.POST("/create-user", authorizationController.CreateUser)

	srv := &http.Server{
		Addr:    port,
		Handler: r,
	}

	go func() {
		fmt.Println("Listening connection on port " + port)
		if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Fatalf("listen: %s\n", err)
		}
	}()

	stop := make(chan os.Signal, 1)
	signal.Notify(stop, syscall.SIGINT, syscall.SIGTERM)
	<-stop

	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	if err := srv.Shutdown(ctx); err != nil {
		log.Fatal("Server Shutdown:", err)
	}

	<-ctx.Done()
}
