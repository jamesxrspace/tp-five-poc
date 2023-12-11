package controller

import (
	"xrspace.io/server/core/dependency/gin_engine/router"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/asset/port/output/repository/mongo"
	"xrspace.io/server/modules/asset/port/output/storage/s3storage"
)

var _ router.IRouter = (*AssetRouter)(nil)

type AssetRouter struct {
	assetController *AssetController
	dependencies    *router.Dependencies
}

func NewAssetRouter(dependencies *router.Dependencies) *AssetRouter {
	repository := mongo.NewUploadRepository(dependencies.DB)
	storageClient := s3_client.NewS3StorageClient(dependencies.AwsSession, dependencies.Config.Aws.S3)
	storage := s3storage.NewS3Storage(*storageClient)

	return &AssetRouter{
		assetController: NewAssetController(
			repository,
			storage,
		),
		dependencies: dependencies,
	}
}

func (r *AssetRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")
	asset := v1.Group("/asset")
	asset.POST("/upload", r.assetController.CreateUploadRequest)
	asset.POST("/uploaded/:request_id", r.assetController.ConfirmUploaded)

	// decoration
	decoration := asset.Group("/decoration")
	decoration.GET("/list", r.assetController.ListDecoration)
	decoration.GET("/category", r.assetController.ListDecorationCategory)
}
