package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/room/application/command"
	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/application/query"
)

type Facade struct {
	*application.AbsFacade
	dep *define.Dependency
}

func NewFacade(dep *define.Dependency) *Facade {
	f := &Facade{
		AbsFacade: application.NewAbsFacade(),
		dep:       dep,
	}

	f.RegisterUseCase(&command.SyncJoinRoomStatusCommand{}, command.NewSyncJoinRoomStatusUseCase(dep))
	f.RegisterUseCase(&command.SyncLeaveRoomStatusCommand{}, command.NewSyncLeaveRoomStatusUseCase(dep))
	f.RegisterUseCase(&query.FusionCustomAuthBody{}, query.NewFusionCustomAuthUseCase(dep))
	f.RegisterUseCase(&query.GetAllRoomDataQuery{}, query.NewGetAllRoomDataUseCase(dep))
	f.RegisterUseCase(&query.GetRoomByIDQuery{}, query.NewGetRoomByIDUseCase(dep))

	f.RegisterUseCase(&query.ListRoomBySpaceIDQuery{}, query.NewListRoomBySpaceIDUsecase(dep))
	return f
}

var _ application.IFacade = (*Facade)(nil)
