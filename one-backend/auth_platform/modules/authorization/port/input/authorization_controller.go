package input

import (
	"auth_platform/core/responser"
	service "auth_platform/core/service/jwt"
	"auth_platform/modules/authorization/application/command"
	"auth_platform/modules/authorization/application/query"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"auth_platform/modules/authorization/domain/repository"

	"github.com/gin-gonic/gin"
)

type HttpResponse struct {
	Data any    `json:"data,omitempty"`
	Msg  string `json:"msg,omitempty"`
	Code int    `json:"code,omitempty"`
}

type AuthorizationController struct {
	AuthenticationService  *service.JwtService
	GetTokenUsecase        *query.GetTokenUsecase
	GetJwksUsecase         *query.GetJwksUsecase
	GetProfileUsecase      *query.GetProfileUsecase
	RefreshTokenUsecase    *query.RefreshTokenUsecase
	GetManagerTokenUsecase *query.GetManagerTokenUsecase
	UpdateProfileUsecase   *command.UpdateProfileUsecase
	CreateUserUsecase      *command.CreateUserUsecase
	Responser              *responser.HttpResponser
}

func NewAuthorizationController(s *service.JwtService, repo repository.IUserRepository) *AuthorizationController {
	return &AuthorizationController{
		AuthenticationService:  s,
		GetTokenUsecase:        query.NewGetTokenUsecase(s, repo),
		GetJwksUsecase:         query.NewGetJwksUsecase(s),
		GetProfileUsecase:      query.NewGetProfileUsecase(s, repo),
		RefreshTokenUsecase:    query.NewRefreshTokenUsecase(s, repo),
		GetManagerTokenUsecase: query.NewGetManagerTokenUsecase(s),
		UpdateProfileUsecase:   command.NewUpdateProfileUsecase(s, repo),
		CreateUserUsecase:      command.NewCreateUserUsecase(s, repo),
	}
}

func (a AuthorizationController) GetToken(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	var q query.GetTokenQuery

	if err := ctx.ShouldBind(&q); err != nil {
		responser.Response(nil, err)
		return
	}

	resp, err := a.GetTokenUsecase.Execute(ctx, &q)
	if err != nil {
		responser.Response(nil, err)
		return
	}

	responser.Response(resp, nil)
}

func (a AuthorizationController) GetJwks(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	jwks := a.GetJwksUsecase.Execute(ctx)

	responser.Response(jwks, nil)
}

func (a AuthorizationController) GetProfile(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	var q query.GetProfileQuery

	if err := ctx.ShouldBind(&q); err != nil {
		responser.Response(nil, err)
		return
	}
	q.AccessToken = ctx.GetString("access_token")
	q.UserID = ctx.GetString("user_id")

	resp, err := a.GetProfileUsecase.Execute(ctx, &q)
	if err != nil {
		responser.Response(nil, err)
		return
	}

	responser.Response(resp, nil)
}

func (a AuthorizationController) RefreshToken(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	var q query.RefreshTokenQuery

	if err := ctx.ShouldBind(&q); err != nil {
		responser.Response(nil, err)
		return
	}

	resp, err := a.RefreshTokenUsecase.Execute(ctx, &q)
	if err != nil {
		responser.Response(nil, err)
		return
	}

	responser.Response(resp, nil)
}

func (a AuthorizationController) UpdateProfile(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	var c command.UpdateProfileCommand

	if err := ctx.ShouldBind(&c); err != nil {
		responser.Response(nil, err)
		return
	}

	c.AccessToken = ctx.GetString("access_token")
	c.UserID = ctx.GetString("user_id")

	resp, err := a.UpdateProfileUsecase.Execute(ctx, &c)
	if err != nil {
		responser.Response(nil, err)
		return
	}

	responser.Response(resp, nil)
}

func (a AuthorizationController) CreateUser(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	var c command.CreateUserCommand

	if err := ctx.ShouldBind(&c); err != nil {
		responser.Response(nil, err)
		return
	}

	resp, err := a.CreateUserUsecase.Execute(ctx, &c)
	if err != nil {
		responser.Response(nil, auth_errors.NewErrCreateUser(err, nil))
		return
	}

	responser.Response(resp, nil)
}

func (a AuthorizationController) SignIn(ctx *gin.Context) {
	responser := a.getResponser(ctx)
	responser.ResponseWithHtml("index", "Sign In")
}

func (a AuthorizationController) GetManagerToken(ctx *gin.Context) {
	responser := a.getResponser(ctx)

	var q query.GetManagerTokenQuery

	if err := ctx.ShouldBind(&q); err != nil {
		responser.Response(nil, err)
		return
	}

	resp, err := a.GetManagerTokenUsecase.Execute(ctx, &q)
	if err != nil {
		responser.Response(nil, err)
		return
	}

	responser.Response(resp, nil)
}

func (a AuthorizationController) getResponser(ctx *gin.Context) *responser.HttpResponser {
	if a.Responser == nil {
		a.Responser = responser.NewHttpResponser(ctx)
	}
	return a.Responser
}
