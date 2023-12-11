package inmem

import (
	"xrspace.io/server/core/arch/example/domain/entity"
	"xrspace.io/server/core/arch/example/domain/repository"
	"xrspace.io/server/core/arch/port/repository/inmem"
)

func NewRoomRepository() repository.IExampleRoomRepository {
	return inmem.NewInMemGenericRepository[*entity.ExampleRoom](map[interface{}]*entity.ExampleRoom{})
}
