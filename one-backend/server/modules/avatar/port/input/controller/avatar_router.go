package controller

import (
	"xrspace.io/server/core/dependency/gin_engine/router"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/avatar/domain/service"
	"xrspace.io/server/modules/avatar/port/output/repository/mongo"
	s3_storage "xrspace.io/server/modules/avatar/port/output/storage/s3storage"
)

var _ router.IRouter = (*AvatarRouter)(nil)

type AvatarRouter struct {
	avatarController *AvatarController
	dependencies     *router.Dependencies
}

func NewAvatarRouter(dependencies *router.Dependencies) *AvatarRouter {
	avatarRepository := mongo.NewAvatarRepository(dependencies.DB, &dependencies.Config.Avatar)
	storageClient := s3_client.NewS3StorageClient(dependencies.AwsSession, dependencies.Config.Aws.S3)
	avatarStorage := s3_storage.NewS3Storage(*storageClient)
	avatarService := service.NewAvatarService()

	return &AvatarRouter{
		avatarController: NewAvatarController(
			avatarRepository,
			avatarStorage,
			avatarService,
		),
		dependencies: dependencies,
	}
}

func (r *AvatarRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")
	avatar := v1.Group("/avatar")
	avatar.POST("/save", r.avatarController.SaveAvatar)
	avatar.POST("/activate/:avatar_id", r.avatarController.SetPlayerAvatar)
	avatar.GET("/current", r.avatarController.CurrentAvatar)
	avatar.GET("/current/list", r.avatarController.ListCurrentAvatar)
}
