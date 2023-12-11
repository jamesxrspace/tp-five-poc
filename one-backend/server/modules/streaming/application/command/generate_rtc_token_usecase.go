package command

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/streaming/domain/enum"
	"xrspace.io/server/modules/streaming/domain/rtc_token_provider"
)

const (
	DefaultExpireTimeInSeconds = 600
)

type GenerateRTCTokenResponse struct {
	Token string `json:"token"`
}

type GenerateRTCTokenUseCase struct {
	rtcTokenProvider rtc_token_provider.IRTCTokenProvider
}

type GenerateRTCTokenCommand struct {
	Role      string `json:"role" validate:"required,oneof=subscriber publisher"`
	ChannelId string `json:"channel_id" validate:"required"`
	ExpiresIn uint32 `json:"expires_in"`
	XrId      string
}

var _ application.IUseCase = (*GenerateRTCTokenUseCase)(nil)

func NewGenerateRTCTokenUseCase(rtcTokenProvider rtc_token_provider.IRTCTokenProvider) *GenerateRTCTokenUseCase {
	return &GenerateRTCTokenUseCase{
		rtcTokenProvider: rtcTokenProvider,
	}
}

func (c *GenerateRTCTokenUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*GenerateRTCTokenCommand)

	var expireTimeInSeconds uint32 = DefaultExpireTimeInSeconds
	if cmd.ExpiresIn > 0 {
		expireTimeInSeconds = cmd.ExpiresIn
	}

	token, err := c.rtcTokenProvider.GetToken(&rtc_token_provider.GetTokenParams{
		ChannelName:        cmd.ChannelId,
		Account:            cmd.XrId,
		Role:               enum.EnumGetTokenRole(cmd.Role),
		ExpireTimeInSecond: expireTimeInSeconds,
	})

	if err != nil {
		return nil, err
	}

	return &GenerateRTCTokenResponse{
		Token: token,
	}, nil
}
