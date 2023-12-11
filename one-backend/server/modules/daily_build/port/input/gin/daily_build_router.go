package gin

import (
	"xrspace.io/server/core/dependency/gin_engine/router"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/daily_build/port/output/storage/s3"
)

var _ router.IRouter = (*DailyBuildRouter)(nil)

type DailyBuildRouter struct {
	dailyBuildController *DailyBuildController
	dependencies         *router.Dependencies
}

func NewDailyBuildRouter(dependencies *router.Dependencies) *DailyBuildRouter {
	repo := s3.NewDailyBuildStorage(
		s3_client.NewS3StorageClient(dependencies.AwsSession, dependencies.Config.Aws.S3),
		dependencies.Config.Aws.S3.Buckets["apk_build"].Name,
	)

	return &DailyBuildRouter{
		dailyBuildController: NewDailyBuildController(
			repo,
		),
		dependencies: dependencies,
	}
}

func (s *DailyBuildRouter) RegisterRoutes(router router.RouterGroup) {
	v1 := router.CmsGroup.Group("/v1")
	g := v1.Group("/daily_build")
	g.GET("/list", s.dailyBuildController.ListDailyBuild)
	g.DELETE("/delete", s.dailyBuildController.DeleteDailyBuild)
}
