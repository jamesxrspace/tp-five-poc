package repository

import (
	"context"

	"xrspace.io/server/modules/room/domain/entity"
)

type IRoomRepository interface {
	Get(ctx context.Context, id string) (*entity.Room, error)
	Save(ctx context.Context, room *entity.Room) error
	AllRoomData(ctx context.Context) ([]*entity.Room, error)
	GetByID(ctx context.Context, id string) (*entity.Room, error)
}
