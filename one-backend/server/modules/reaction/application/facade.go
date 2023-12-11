package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/reaction/application/command"
	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/application/query"
)

var _ application.IFacade = (*Facade)(nil)

type Facade struct {
	*application.AbsFacade

	dep define.Dependency
}

func NewFacade(dep define.Dependency) *Facade {
	f := &Facade{
		AbsFacade: application.NewAbsFacade(),
		dep:       dep,
	}

	f.RegisterUseCase(&command.LikeCommand{}, command.NewLikeUseCase(dep))
	f.RegisterUseCase(&query.GetLikeQuery{}, query.NewGetLikeUseCase(dep))

	return f
}
