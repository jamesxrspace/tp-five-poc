package application

import (
	"xrspace.io/server/core/arch/application"
	sns_client "xrspace.io/server/core/dependency/sns"
	"xrspace.io/server/modules/aigc/application/callback"
	"xrspace.io/server/modules/aigc/application/command"
	"xrspace.io/server/modules/aigc/application/mock"
	"xrspace.io/server/modules/aigc/domain"
)

type Facade struct {
	*application.AbsFacade

	queue      domain.IQueue
	storage    domain.IStorage
	inferencer domain.IInferencer
	snsClient  *sns_client.SNSClient
}

func NewFacade(queue domain.IQueue, storage domain.IStorage, inferencer domain.IInferencer, snsClient *sns_client.SNSClient) *Facade {
	f := &Facade{
		AbsFacade:  application.NewAbsFacade(),
		queue:      queue,
		storage:    storage,
		inferencer: inferencer,
		snsClient:  snsClient,
	}

	f.RegisterUseCase(&command.GenerateMotionCommand{}, command.NewGenerateMotionUsecase(f.queue, f.storage, f.inferencer))
	f.RegisterUseCase(&mock.GenerateMotionCommand{}, mock.NewGenerateMotionUsecase(f.storage))
	f.RegisterUseCase(&callback.ConfirmaCommand{}, callback.NewInferencerCallbackConfirmation(f.snsClient))
	f.RegisterUseCase(&callback.Payload{}, callback.NewInferencerCallback(f.queue, f.snsClient))
	return f
}

var _ application.IFacade = (*Facade)(nil)
