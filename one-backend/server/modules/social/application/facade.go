package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/social/application/command"
	"xrspace.io/server/modules/social/application/define"
	"xrspace.io/server/modules/social/application/query"
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

	f.RegisterUseCase(&command.FollowCommand{}, command.NewFollowUseCase(dep))
	f.RegisterUseCase(&query.IsFollowingQuery{}, query.NewIsFollowingUseCase(dep))
	f.RegisterUseCase(&query.GetFollowersCountQuery{}, query.NewGetFollowersCountUseCase(dep))
	f.RegisterUseCase(&query.GetFollowingCountQuery{}, query.NewGetFollowingCountUseCase(dep))
	f.RegisterUseCase(&query.ListFollowersQuery{}, query.NewListFollowersUseCase(dep))
	f.RegisterUseCase(&query.ListFollowingQuery{}, query.NewListFollowingUseCase(dep))

	return f
}
