package query

import (
	"context"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/modules/room/application/define"
	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/port/output/repo/inmem"
)

func TestGetAllRoomData(t *testing.T) {

	type args struct {
		rooms []*entity.Room
	}
	tests := []struct {
		name    string
		args    args
		want    *GetAllRoomDataResponse
		wantErr bool
		ErrMsg  string
	}{
		{
			name: "success",
			args: args{
				rooms: DefaultRoom(),
			},
			want: &GetAllRoomDataResponse{
				Rooms: []*entity.Room{
					entity.NewRoom("1", "1"),
				},
			},
			wantErr: false,
		},
		{
			name: "room not found",
			args: args{
				rooms: []*entity.Room{},
			},
			want:    &GetAllRoomDataResponse{},
			wantErr: false,
			ErrMsg:  "room not found",
		},
	}

	for _, tt := range tests {
		// arrange
		ctx := context.Background()
		repository := inmem.NewInMemRoomRepo()
		for _, room := range tt.args.rooms {
			_ = repository.Save(ctx, room)
		}

		dep := &define.Dependency{
			Repo: repository,
		}
		u := NewGetAllRoomDataUseCase(dep)

		// act
		got, err := u.Execute(ctx, &GetAllRoomDataQuery{})

		if (err != nil) != tt.wantErr {
			t.Errorf("GetSpaceListUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
			return
		}

		if tt.wantErr {
			assert.Contains(t, err.Error(), tt.ErrMsg)
			return
		}

		// assert

		assert.Equal(t, len(tt.want.Rooms), len(got.(*GetAllRoomDataResponse).Rooms))
	}
}

func DefaultRoom() []*entity.Room {
	var rooms []*entity.Room

	room1 := entity.NewRoom("1", "1")

	_ = room1.JoinRoom(
		entity.NewUser("1"),
	)

	rooms = append(rooms, room1)

	return rooms
}
