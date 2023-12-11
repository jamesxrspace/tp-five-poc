package entity

import (
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"
)

func TestJoinRoom(t *testing.T) {
	room := Room{
		ID:      "1",
		SpaceID: "1",
		Users:   make(map[XrID]User),
	}
	user := NewUser("1")

	// act
	err := room.JoinRoom(user)

	// assert
	expected := 1
	assert.Equal(t, expected, len(room.Users))
	assert.Nil(t, err)
}
