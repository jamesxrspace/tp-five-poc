package controller

import (
	"github.com/gin-gonic/gin"

	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/modules/asset/application"
	"xrspace.io/server/modules/asset/application/command"
	"xrspace.io/server/modules/asset/application/query"
	"xrspace.io/server/modules/asset/domain/repository"
	"xrspace.io/server/modules/asset/domain/storage"
)

type AssetController struct {
	facade *application.Facade
}

func NewAssetController(
	repository repository.IUploadRequestRepository,
	storage storage.IStorage,
) *AssetController {
	return &AssetController{
		facade: application.NewFacade(repository, storage),
	}
}

func (a *AssetController) CreateUploadRequest(ctx *gin.Context) {
	portGin.Handle(ctx, a.facade, &command.CreateUploadRequestCommand{})
}

func (a *AssetController) ConfirmUploaded(ctx *gin.Context) {
	portGin.Handle(ctx, a.facade, &command.ConfirmUploadedCommand{})
}

func (a *AssetController) ListDecoration(ctx *gin.Context) {
	portGin.Handle(ctx, a.facade, &query.ListDecorationQuery{})
}

func (a *AssetController) ListDecorationCategory(ctx *gin.Context) {
	portGin.Handle(ctx, a.facade, &query.ListDecorationCategoryQuery{})
}
