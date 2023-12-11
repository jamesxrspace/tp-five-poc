package gin

import (
	"github.com/gin-gonic/gin"

	portGin "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/modules/room/application"
	"xrspace.io/server/modules/room/application/command"
	"xrspace.io/server/modules/room/application/query"
)

type GinInputPort struct {
	facade *application.Facade
}

func NewGinInputPort(facade *application.Facade) *GinInputPort {
	return &GinInputPort{
		facade: facade,
	}
}

func (g *GinInputPort) JoinRoom(ctx *gin.Context) {
	portGin.Handle(ctx, g.facade, &command.SyncJoinRoomStatusCommand{})
}

func (g *GinInputPort) LeaveRoom(ctx *gin.Context) {
	portGin.Handle(ctx, g.facade, &command.SyncLeaveRoomStatusCommand{})
}

func (g *GinInputPort) FusionCustomAuth(ctx *gin.Context) {
	portGin.HandleCustomResponse(ctx, g.facade, &query.FusionCustomAuthBody{}, photonResponse)
}

func (g *GinInputPort) GetRoomByID(ctx *gin.Context) {
	portGin.Handle(ctx, g.facade, &query.GetRoomByIDQuery{})
}

func (g *GinInputPort) AllRoomData(ctx *gin.Context) {
	portGin.Handle(ctx, g.facade, &query.GetAllRoomDataQuery{})

}
