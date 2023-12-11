package application

import (
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/modules/streaming/application/command"
	"xrspace.io/server/modules/streaming/domain/rtc_token_provider"
)

type Facade struct {
	*application.AbsFacade

	rtcTokenProvider rtc_token_provider.IRTCTokenProvider
}

func NewFacade(rtcTokenProvider rtc_token_provider.IRTCTokenProvider) *Facade {
	f := &Facade{
		AbsFacade:        application.NewAbsFacade(),
		rtcTokenProvider: rtcTokenProvider,
	}

	f.RegisterUseCase(&command.GenerateRTCTokenCommand{}, command.NewGenerateRTCTokenUseCase(f.rtcTokenProvider))
	return f
}

var _ application.IFacade = (*Facade)(nil)
