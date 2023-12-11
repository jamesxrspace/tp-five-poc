package inmem

import (
	"context"
	"encoding/json"

	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/domain/repository"
)

type RoomRepo struct {
	repo map[string][]byte
}

func (i *RoomRepo) GetByID(_ context.Context, id string) (*entity.Room, error) {
	if _, ok := i.repo[id]; ok {
		room := &entity.Room{}
		err := json.Unmarshal(i.repo[id], room)

		if err != nil {
			return nil, err
		}
		return room, nil
	}
	return nil, nil
}

var _ repository.IRoomRepository = (*RoomRepo)(nil)

func NewInMemRoomRepo() *RoomRepo {
	return &RoomRepo{
		repo: make(map[string][]byte),
	}
}

func (i *RoomRepo) Get(_ context.Context, id string) (*entity.Room, error) {
	if _, ok := i.repo[id]; !ok {
		return nil, nil
	}

	room := &entity.Room{}
	err := json.Unmarshal(i.repo[id], room)
	if err != nil {
		return nil, err
	}

	return room, nil
}

func (i *RoomRepo) Save(_ context.Context, room *entity.Room) error {
	j, _ := json.Marshal(room)
	i.repo[room.ID] = j

	return nil
}

func (i *RoomRepo) AllRoomData(_ context.Context) ([]*entity.Room, error) {
	var rooms []*entity.Room
	for _, v := range i.repo {
		r := &entity.Room{}
		_ = json.Unmarshal(v, r)
		rooms = append(rooms, r)
	}
	return rooms, nil
}
