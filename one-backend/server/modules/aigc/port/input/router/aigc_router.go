package gin

import (
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/core/dependency/sagemaker"
	sns_client "xrspace.io/server/core/dependency/sns"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/aigc/port/input/controller"
	sg_inferencer "xrspace.io/server/modules/aigc/port/output/inferencer/sagemaker"
	redis_queue "xrspace.io/server/modules/aigc/port/output/queue/redis"
	s3_storage "xrspace.io/server/modules/aigc/port/output/storage/s3"
)

type AigcRouter struct {
	aigcController *controller.AigcController
	dependencies   *router.Dependencies
}

var _ router.IRouter = (*AigcRouter)(nil)

func NewAvatarRouter(dependencies *router.Dependencies) *AigcRouter {
	return &AigcRouter{
		aigcController: controller.NewAigcController(
			redis_queue.NewAigcQueue(dependencies.Redis),
			s3_storage.NewAigcStorage(
				s3_client.NewS3StorageClient(dependencies.AwsSession, dependencies.Config.Aws.S3),
			),
			sg_inferencer.NewAigcInferencer(
				sagemaker.NewSageMakerRuntimeClient(dependencies.AwsSession),
				dependencies.Config.AIServer.AigcMotion.Endpoint,
			),
			sns_client.NewSNSClient(dependencies.AwsSession),
			dependencies.Config.Env,
		),
		dependencies: dependencies,
	}
}

func (r *AigcRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")
	aigc := v1.Group("/aigc")
	aigc.POST("/motion/generate", r.aigcController.GenerateMotion)
	aigc.POST("/motion/generate/mock", r.aigcController.GenerateMockMotion)

	public := g.PublicApiGroup.Group("/v1")
	publicAigc := public.Group("/aigc")
	publicAigc.POST("/sns_endpoint/motion", r.aigcController.AwsSnsHandler)
}
