package controller

import (
	"errors"
	"net/http"

	"github.com/gin-gonic/gin"
	"xrspace.io/server/core/response/gin_responser"
	"xrspace.io/server/modules/avatar/application/command"
	"xrspace.io/server/modules/avatar/application/query"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/service"
	"xrspace.io/server/modules/avatar/domain/storage"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type AvatarController struct {
	SaveAvatarUseCase       *command.SaveAvatarUseCase
	SetPlayerAvatarUseCase  *command.SetPlayerAvatarUseCase
	GetCurrentAvatarUseCase *query.GetCurrentAvatarUseCase
	ListGroupAvatarsUseCase *query.ListGroupAvatarsUseCase
	responser               *gin_responser.GinResponser
}

func NewAvatarController(
	repository repository.IAvatarRepository,
	storage storage.IAssetStorage,
	service service.IAvatarService,
) *AvatarController {
	return &AvatarController{
		SaveAvatarUseCase: command.NewSaveAvatarUseCase(
			repository,
			storage,
			service,
		),
		SetPlayerAvatarUseCase: command.NewSetPlayerAvatarUseCase(
			repository,
		),
		GetCurrentAvatarUseCase: query.NewGetCurrentAvatarUseCase(
			repository,
		),
		ListGroupAvatarsUseCase: query.NewListGroupAvatarsUseCase(
			repository,
		),
		responser: gin_responser.NewGinResponser(),
	}
}

func (c *AvatarController) SaveAvatar(ctx *gin.Context) {
	var cmd command.SaveAvatarCommand
	if err := ctx.ShouldBind(&cmd); err != nil {
		ctx.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	cmd.XrID = value_object.XrID(ctx.GetString("xrid"))
	cmd.AppID = value_object.AppID(ctx.GetString("app_id"))

	avatarAsset, err := parseFile(ctx, "avatar_asset")
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}
	cmd.AvatarAsset = avatarAsset

	avatarHead, err := parseFile(ctx, "avatar_head")
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}
	cmd.AvatarHead = avatarHead

	avatarUpperBody, err := parseFile(ctx, "avatar_upper_body")
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}
	cmd.AvatarUpperBody = avatarUpperBody

	avatarFullBody, err := parseFile(ctx, "avatar_full_body")
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}
	cmd.AvatarFullBody = avatarFullBody

	res, err := c.SaveAvatarUseCase.Execute(ctx.Request.Context(), &cmd)
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}

	c.responser.Response(ctx, res, nil)
}

func (c *AvatarController) SetPlayerAvatar(ctx *gin.Context) {
	var cmd command.SetPlayerAvatarCommand
	if err := ctx.ShouldBind(&cmd); err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}
	cmd.XrID = value_object.XrID(ctx.GetString("xrid"))
	cmd.AppID = value_object.AppID(ctx.GetString("app_id"))
	cmd.AvatarID = value_object.AvatarID(ctx.Param("avatar_id"))

	res, err := c.SetPlayerAvatarUseCase.Execute(ctx.Request.Context(), &cmd)
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}

	c.responser.Response(ctx, res, nil)
}

func (c *AvatarController) CurrentAvatar(ctx *gin.Context) {
	var q query.GetCurrentAvatarQuery
	if err := ctx.ShouldBind(&q); err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}

	q.XrID = value_object.XrID(ctx.GetString("xrid"))
	q.AppID = value_object.AppID(ctx.GetString("app_id"))

	res, err := c.GetCurrentAvatarUseCase.Execute(ctx.Request.Context(), &q)
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}

	c.responser.Response(ctx, res, nil)
}

func (c *AvatarController) ListCurrentAvatar(ctx *gin.Context) {
	var q query.ListGroupAvatarsQuery
	if err := ctx.ShouldBind(&q); err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}

	q.AppID = value_object.AppID(ctx.GetString("app_id"))

	res, err := c.ListGroupAvatarsUseCase.Execute(ctx.Request.Context(), &q)
	if err != nil {
		c.responser.Response(ctx, nil, err)
		return
	}

	c.responser.Response(ctx, res, nil)
}

func parseFile(ctx *gin.Context, key string) (*value_object.AvatarAsset, error) {
	file, header, err := ctx.Request.FormFile(key)
	if err != nil {
		if errors.Is(err, http.ErrMissingFile) {
			return nil, nil
		}
		return nil, err
	}

	return &value_object.AvatarAsset{
		File:       file,
		FileHeader: header,
	}, nil
}
