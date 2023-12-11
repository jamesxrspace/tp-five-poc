package repository

import (
	"xrspace.io/server/core/arch/domain/repository"
	"xrspace.io/server/core/arch/example/domain/entity"
)

type IExampleRoomRepository repository.IDomainRepository[*entity.ExampleRoom]
