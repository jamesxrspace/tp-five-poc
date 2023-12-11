package gin

import (
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/gin_engine/router"
	"xrspace.io/server/modules/space/port/output/repository/mongo"
)

var _ router.IRouter = (*SpaceRouter)(nil)

type SpaceRouter struct {
	spaceController *SpaceController
	dependencies    *router.Dependencies
}

func NewSpaceRouter(dependencies *router.Dependencies) *SpaceRouter {
	repo := mongo.NewSpaceGroupRepository(dependencies.DB)
	spaceRepo := mongo.NewSpaceRepository(dependencies.DB)
	unitOfWork := docdb.NewUnitOfWork(*dependencies.DB)

	return &SpaceRouter{
		spaceController: NewSpaceController(
			repo,
			spaceRepo,
			unitOfWork,
		),
		dependencies: dependencies,
	}
}

func (s *SpaceRouter) RegisterRoutes(router router.RouterGroup) {
	// endpoit for app: /api/v1/space
	v1 := router.ApiGroup.Group("/v1")
	g := v1.Group("/space")

	// space
	g.GET("/list", s.spaceController.ListSpace)

	// space/group
	g.GET("/group/list", s.spaceController.ListSpaceGroup)

	// endpoit for cms: /api/cms/v1/space
	v1 = router.CmsGroup.Group("/v1")
	g = v1.Group("/space")

	// space
	g.POST("", s.spaceController.CreateSpace)
	g.GET("/list", s.spaceController.ListSpace)
	g.PUT("/:space_id", s.spaceController.UpdateSpace)
	g.DELETE("/:space_id", s.spaceController.DeleteSpace)

	// space/group
	g.POST("/group", s.spaceController.CreateSpaceGroup)
	g.GET("/group/list", s.spaceController.ListSpaceGroup)
	g.PUT("/group/:space_group_id", s.spaceController.UpdateSpaceGroup)
	g.DELETE("/group/:space_group_id", s.spaceController.DeleteSpaceGroup)
}
