package command

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	rtc_token_provider "xrspace.io/server/core/dependency/streaming/rtc_token_provider/inmem"
)

func TestGenerateRTCTokenUseCase_Execute(t *testing.T) {
	tests := []struct {
		name   string
		cmd    *GenerateRTCTokenCommand
		expect string
	}{
		{
			name: "Regular case with role publisher",
			cmd: &GenerateRTCTokenCommand{
				Role:      "publisher",
				ChannelId: "dummy_channel_id",
				XrId:      "dummy_xr_id",
			},
			expect: "&{ChannelName:dummy_channel_id Account:dummy_xr_id Role:publisher ExpireTimeInSecond:600}",
		},
		{
			name: "Regular case with role subscriber",
			cmd: &GenerateRTCTokenCommand{
				Role:      "subscriber",
				ChannelId: "dummy_channel_id",
				XrId:      "dummy_xr_id",
			},
			expect: "&{ChannelName:dummy_channel_id Account:dummy_xr_id Role:subscriber ExpireTimeInSecond:600}",
		},
		{
			name: "Regular case with ExpiresIn",
			cmd: &GenerateRTCTokenCommand{
				Role:      "publisher",
				ChannelId: "dummy_channel_id",
				XrId:      "dummy_xr_id",
				ExpiresIn: 300,
			},
			expect: "&{ChannelName:dummy_channel_id Account:dummy_xr_id Role:publisher ExpireTimeInSecond:300}",
		},
		{
			name: "Regular case with zero ExpiresIn",
			cmd: &GenerateRTCTokenCommand{
				Role:      "publisher",
				ChannelId: "dummy_channel_id",
				XrId:      "dummy_xr_id",
				ExpiresIn: 0,
			},
			expect: "&{ChannelName:dummy_channel_id Account:dummy_xr_id Role:publisher ExpireTimeInSecond:600}",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			provider := rtc_token_provider.NewRTCTokenProvider()
			useCase := NewGenerateRTCTokenUseCase(provider)

			// act
			resp, err := useCase.Execute(ctx, tt.cmd)

			// assert
			assert.Nil(t, err)
			assert.Equal(t, resp.(*GenerateRTCTokenResponse).Token, tt.expect)
		})
	}
}
