package gin

import (
	"github.com/gin-gonic/gin"
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/space/application"
	"xrspace.io/server/modules/space/application/command"
	"xrspace.io/server/modules/space/application/query"
)

type SpaceController struct {
	facade *application.Facade
}

func NewSpaceController(spaceGroupRepo application.IGeneralRepository, spaceRepo application.IGeneralSpaceRepository, unitOfWork unit_of_work.IUnitOfWork) *SpaceController {
	return &SpaceController{
		facade: application.NewFacade(spaceGroupRepo, spaceRepo, unitOfWork),
	}
}

func (c *SpaceController) CreateSpaceGroup(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.CreateSpaceGroupCommand{})
}

func (c *SpaceController) ListSpaceGroup(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &query.ListSpaceGroupQuery{})
}

func (c *SpaceController) UpdateSpaceGroup(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.UpdateSpaceGroupCommand{})
}

func (c *SpaceController) DeleteSpaceGroup(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.DeleteSpaceGroupCommand{})
}

func (c *SpaceController) CreateSpace(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.CreateSpaceCommand{})
}

func (c *SpaceController) UpdateSpace(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.UpdateSpaceCommand{})
}

func (c *SpaceController) DeleteSpace(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &command.DeleteSpaceCommand{})
}

func (c *SpaceController) ListSpace(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &query.ListSpaceQuery{})
}
