package define

import (
	"context"

	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/room/domain/entity"
)

type IQueryRepository interface {
	ListRoomBySpaceID(ctx context.Context, spaceID string, page *pagination.PaginationQuery) (roomList []*entity.Room, total int, err error)
}
