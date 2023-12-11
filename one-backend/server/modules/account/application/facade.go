package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/account/application/command"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/application/query"
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

	f.RegisterUseCase(&command.LoginCommand{}, command.NewLoginUseCase(dep))
	f.RegisterUseCase(&command.CreateGuestCommand{}, command.NewCreateGuestUseCase(dep))
	f.RegisterUseCase(&query.GetProfileQuery{}, query.NewGetProfileUseCase(dep))

	return f
}
