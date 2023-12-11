package application

import (
	"context"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/core/arch/example/port/repository/inmem"
)

func TestFacadeExecuteSuccess(t *testing.T) {
	repo := inmem.NewRoomRepository()
	facade := NewAFacade(repo)
	cmd := &JoinCommand{
		XrId:   "123",
		RoomId: "456",
	}

	result, err := facade.Execute(context.Background(), cmd)

	j, _ := result.(*joinResponse)

	assert.Nil(t, err)
	assert.Equal(t, j.XrRoom, "456123")
	assert.IsType(t, result, &joinResponse{})
}

func TestFacadeExecute_CmdInvalid_ShouldFail(t *testing.T) {
	repo := inmem.NewRoomRepository()
	facade := NewAFacade(repo)
	cmd := &JoinCommand{
		RoomId: "456",
	}

	result, err := facade.Execute(context.Background(), cmd)

	j, _ := result.(*joinResponse)

	assert.Error(t, err)
	assert.Nil(t, j)
}

func TestFacadeExecute_CmdValidateInterface_ShouldSuccess(t *testing.T) {
	repo := inmem.NewRoomRepository()
	facade := NewAFacade(repo)

	cmdJoin := &JoinCommand{
		XrId:   "123",
		RoomId: "456",
	}
	_, _ = facade.Execute(context.Background(), cmdJoin)

	// act
	cmd := &LeaveCommand{
		XrId:   "123",
		RoomId: "456",
	}

	result, err := facade.Execute(context.Background(), cmd)

	j, _ := result.(*leaveResponse)

	assert.Nil(t, err)
	assert.Equal(t, j.XrRoom, "456123")
	assert.IsType(t, result, &leaveResponse{})
}

func TestFacadeExecute_CmdValidateInterface_ShouldFail(t *testing.T) {
	repo := inmem.NewRoomRepository()
	facade := NewAFacade(repo)
	cmd := &LeaveCommand{
		XrId:   "Not_123",
		RoomId: "456",
	}

	result, err := facade.Execute(context.Background(), cmd)

	j, _ := result.(*joinResponse)

	assert.Error(t, err)
	assert.Nil(t, j)
}
