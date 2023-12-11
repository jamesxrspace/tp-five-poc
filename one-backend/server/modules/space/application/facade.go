package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/space/application/command"
	"xrspace.io/server/modules/space/application/query"
	"xrspace.io/server/modules/space/domain/repository"
)

type IGeneralRepository interface {
	repository.ISpaceGroupRepository
	query.IQuerySpaceGroupRepository
}

type IGeneralSpaceRepository interface {
	repository.ISpaceRepository
	query.IQuerySpaceRepository
}

type Facade struct {
	*application.AbsFacade

	repo       IGeneralRepository
	spaceRepo  IGeneralSpaceRepository
	unitOfWork unit_of_work.IUnitOfWork
}

func NewFacade(repo IGeneralRepository, spaceRepo IGeneralSpaceRepository, unitOfWork unit_of_work.IUnitOfWork) *Facade {
	f := &Facade{
		AbsFacade:  application.NewAbsFacade(),
		repo:       repo,
		spaceRepo:  spaceRepo,
		unitOfWork: unitOfWork,
	}

	f.RegisterUseCase(&command.CreateSpaceGroupCommand{}, command.NewCreateSpaceGroupUseCase(f.repo, f.spaceRepo, f.unitOfWork))
	f.RegisterUseCase(&query.ListSpaceGroupQuery{}, query.NewListSpaceGroupUseCase(f.repo))
	f.RegisterUseCase(&command.UpdateSpaceGroupCommand{}, command.NewUpdateSpaceGroupUseCase(f.repo, f.spaceRepo, f.unitOfWork))
	f.RegisterUseCase(&command.DeleteSpaceGroupCommand{}, command.NewDeleteSpaceGroupUseCase(f.repo, f.spaceRepo, f.unitOfWork))
	f.RegisterUseCase(&command.CreateSpaceCommand{}, command.NewCreateSpaceUseCase(f.spaceRepo, f.repo, f.unitOfWork))
	f.RegisterUseCase(&command.UpdateSpaceCommand{}, command.NewUpdateSpaceUseCase(f.spaceRepo, f.repo, f.unitOfWork))
	f.RegisterUseCase(&command.DeleteSpaceCommand{}, command.NewDeleteSpaceUseCase(f.spaceRepo))
	f.RegisterUseCase(&query.ListSpaceQuery{}, query.NewListSpaceUseCase(f.spaceRepo))
	return f
}

var _ application.IFacade = (*Facade)(nil)
