package gin

import (
	"github.com/gin-gonic/gin"
	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/modules/daily_build/application"
)

type DailyBuildController struct {
	facade *application.Facade
}

func NewDailyBuildController(repo application.IDailyBuildStorage) *DailyBuildController {
	return &DailyBuildController{
		facade: application.NewFacade(repo),
	}
}

func (c *DailyBuildController) ListDailyBuild(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &application.ListDailyBuildQuery{})
}

func (c *DailyBuildController) DeleteDailyBuild(ctx *gin.Context) {
	portGin.Handle(ctx, c.facade, &application.DeleteDailyBuildQuery{})
}
