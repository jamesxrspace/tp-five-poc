package application

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"xrspace.io/server/modules/streaming/application/command"
)

func TestFailedCommandValidation(t *testing.T) {
	tests := []struct {
		name   string
		cmd    *command.GenerateRTCTokenCommand
		errMsg string
	}{
		{
			name: "Failed case with invalid role",
			cmd: &command.GenerateRTCTokenCommand{
				Role:      "invalid",
				ChannelId: "dummy_channel_id",
			},
			errMsg: "Key: 'GenerateRTCTokenCommand.Role' Error:Field validation for 'Role' failed on the 'oneof' tag",
		},
		{
			name: "Failed case without channel id",
			cmd: &command.GenerateRTCTokenCommand{
				Role: "publisher",
			},
			errMsg: "Key: 'GenerateRTCTokenCommand.ChannelId' Error:Field validation for 'ChannelId' failed on the 'required' tag",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			facade := NewFacade(nil)

			// act
			_, err := facade.Execute(context.Background(), tt.cmd)

			// assert
			assert.Equal(t, tt.errMsg, err.Error())
		})
	}
}
