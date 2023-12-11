package middleware

import (
	"auth_platform/core/responser"
	"auth_platform/core/service/jwt"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"errors"

	"github.com/gin-gonic/gin"
)

type AuthenticationMiddleware struct {
	jwtService *jwt.JwtService
	Responser  *responser.HttpResponser
}

type TokenRequest struct {
	GrantType string `form:"grant_type" json:"grant_type" validate:"required"`
}

func NewAuthenticationMiddleware(jwtService *jwt.JwtService) *AuthenticationMiddleware {
	return &AuthenticationMiddleware{
		jwtService: jwtService,
	}
}

func (a *AuthenticationMiddleware) Authenticate() gin.HandlerFunc {
	return func(ctx *gin.Context) {
		responser := a.getResponser(ctx)

		token := ctx.GetHeader("Authorization")
		accessToken := token[len("Bearer "):]
		if accessToken == "" {
			responser.Response(nil, auth_errors.NewErrInvalidToken(errors.New("wrong authorization header"), token))
			ctx.Abort()
			return
		}

		claims, err := a.jwtService.ParseJwt(accessToken)
		if err != nil || claims == nil {
			responser.Response(nil, auth_errors.NewErrInvalidToken(errors.New("token validation failed"), accessToken))
			ctx.Abort()
			return
		}

		ctx.Set("access_token", accessToken)
		ctx.Set("issuer", claims.Issuer())
		ctx.Set("user_id", claims.Subject())
		ctx.Next()
	}
}

func (a AuthenticationMiddleware) GetOrRefreshToken(getToken func(*gin.Context), refreshToken func(*gin.Context)) gin.HandlerFunc {
	return func(ctx *gin.Context) {
		responser := a.getResponser(ctx)

		var r TokenRequest

		context := ctx.Copy()
		err := context.ShouldBind(&r)
		if err != nil {
			responser.Response(nil, err)
			return
		}

		switch r.GrantType {
		case "refresh_token":
			refreshToken(ctx)
		case "password":
			getToken(ctx)
		default:
			responser.Response(nil, auth_errors.NewErrInvalidArguments(errors.New("invalid grant type"), r))
			ctx.Abort()
			return
		}
	}
}

func (a *AuthenticationMiddleware) ManagerAuthenticate() gin.HandlerFunc {
	return func(ctx *gin.Context) {
		responser := a.getResponser(ctx)

		authUserPoolID := ctx.GetHeader("x-authing-userpool-id")
		if authUserPoolID == "" || authUserPoolID != a.jwtService.GetUserPoolID() {
			responser.Response(nil, auth_errors.NewErrInvalidToken(errors.New("wrong x-authing-userpool-id header"), authUserPoolID))
			ctx.Abort()
			return
		}

		if ctx.Request.RequestURI == "/api/v3/get-management-token" && ctx.Request.Method == "POST" {
			ctx.Next()
			return
		}

		token := ctx.GetHeader("Authorization")
		managerToken := token[len("Bearer "):]
		if managerToken == "" {
			responser.Response(nil, auth_errors.NewErrInvalidToken(errors.New("wrong authorization header"), token))
			ctx.Abort()
			return
		}

		parsedToken, err := a.jwtService.VerifyManagerToken(managerToken)
		if err != nil || parsedToken == nil {
			responser.Response(nil, auth_errors.NewErrInvalidToken(errors.New("token validation failed"), managerToken))
			ctx.Abort()
			return
		}

		userPoolID, ok := parsedToken.PrivateClaims()["scoped_userpool_id"]
		if !ok || userPoolID.(string) != authUserPoolID {
			responser.Response(nil, auth_errors.NewErrInvalidToken(errors.New("token validation failed"), managerToken))
			ctx.Abort()
			return
		}

		ctx.Next()
	}
}

func (a *AuthenticationMiddleware) getResponser(ctx *gin.Context) *responser.HttpResponser {
	if a.Responser == nil {
		a.Responser = responser.NewHttpResponser(ctx)
	}

	return a.Responser
}
