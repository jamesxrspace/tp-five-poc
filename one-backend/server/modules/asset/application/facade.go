package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/asset/application/command"
	"xrspace.io/server/modules/asset/application/query"
	"xrspace.io/server/modules/asset/domain/repository"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/port/output/repository/inmem"
)

type IGeneralRepository interface {
	repository.IUploadRequestRepository
}

type Facade struct {
	*application.AbsFacade

	repo    IGeneralRepository
	storage storage.IStorage
}

func NewFacade(repo IGeneralRepository, storage storage.IStorage) *Facade {
	f := &Facade{
		AbsFacade: application.NewAbsFacade(),
		repo:      repo,
		storage:   storage,
	}

	f.RegisterUseCase(&command.CreateUploadRequestCommand{}, command.NewCreateUploadRequestUseCase(repo, storage))
	f.RegisterUseCase(&command.ConfirmUploadedCommand{}, command.NewConfirmUploadedUsecase(repo, storage))

	fakeRepo := &inmem.QueryRepository{}
	f.RegisterUseCase(&query.ListDecorationQuery{}, query.NewListDecorationUseCase(fakeRepo))
	f.RegisterUseCase(&query.ListDecorationCategoryQuery{}, query.NewListDecorationCategoryUseCase(fakeRepo))

	return f
}

var _ application.IFacade = (*Facade)(nil)
