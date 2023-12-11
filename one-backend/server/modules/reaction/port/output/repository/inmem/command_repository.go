package inmem

import (
	"xrspace.io/server/core/arch/port/repository/inmem"
	"xrspace.io/server/modules/reaction/domain/entity"
	"xrspace.io/server/modules/reaction/domain/repository"
)

func NewLikeRepository(storage map[interface{}]*entity.Like) repository.ILikeRepository {
	return inmem.NewInMemGenericRepository[*entity.Like](storage)
}
