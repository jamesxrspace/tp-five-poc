package application

import (
	"xrspace.io/server/core/arch/application"
)

type IDailyBuildStorage interface {
	IQueryDailyBuildStorage
	IDeleteDailyBuildStorage
}

type Facade struct {
	*application.AbsFacade

	repo IDailyBuildStorage
}

func NewFacade(repo IDailyBuildStorage) *Facade {
	f := &Facade{
		AbsFacade: application.NewAbsFacade(),
		repo:      repo,
	}

	f.RegisterUseCase(&ListDailyBuildQuery{}, NewListDailyBuildUseCase(f.repo))
	f.RegisterUseCase(&DeleteDailyBuildQuery{}, NewDeleteDailyBuildUseCase(f.repo))
	return f
}

var _ application.IFacade = (*Facade)(nil)
