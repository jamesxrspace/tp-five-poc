package repo

import (
	"context"
	"fmt"
	"os"
	"testing"

	"github.com/stretchr/testify/suite"

	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/room/domain/entity"
	"xrspace.io/server/modules/room/domain/repository"
	"xrspace.io/server/modules/room/port/output/repo/inmem"
	"xrspace.io/server/modules/room/port/output/repo/mongo"
)

func TestTestSuite(t *testing.T) {

	testInMem(t)

	if os.Getenv("IN_MAKEFILE") == "1" {
		fmt.Println("test mongo")
		testMongo(t)
	}
}

func testInMem(t *testing.T) {
	s := new(TestSuite)
	s.repo = inmem.NewInMemRoomRepo()
	suite.Run(t, s)
}

func testMongo(t *testing.T) {
	s := new(TestSuite)
	db, err := docdb.NewLocalDocDB(context.Background())
	if err != nil {
		panic(err)
	}
	s.repo = mongo.NewRoomRepo(db)
	suite.Run(t, s)
}

type TestSuite struct {
	suite.Suite
	repo repository.IRoomRepository
}

func (s *TestSuite) SetupTest() {
}

func (s *TestSuite) TestRoomRepo_Save() {
	roomID := "test_room_id"
	room := entity.NewRoom("S1", roomID)
	_ = room.JoinRoom(entity.NewUser("test_user_id"))

	// act
	err := s.repo.Save(context.Background(), room)
	saveRoom, errGet := s.repo.Get(context.Background(), roomID)

	// assert
	s.Equal(roomID, saveRoom.ID)
	s.Equal("S1", saveRoom.SpaceID)
	s.NoError(err)
	s.NoError(errGet)
}

func (s *TestSuite) TestRoomRepo_GivenNoRoom_WhenGet_ShouldReturnNil() {
	roomID := "not_exists_room_id"
	// act
	saveRoom, errGet := s.repo.Get(context.Background(), roomID)

	// assert
	s.Nil(saveRoom)
	s.Nil(errGet)
}

func (s *TestSuite) TestRoomRepo_GivenNoRoom_WhenGetAllRoom_ShouldReturnError() {
	// act
	room, errGet := s.repo.AllRoomData(context.Background())

	// assert
	s.Equal(0, len(room))
	s.Nil(errGet)
}
