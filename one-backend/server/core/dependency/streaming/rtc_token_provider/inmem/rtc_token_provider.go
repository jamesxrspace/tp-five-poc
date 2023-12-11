package rtc_token_provider

import (
	"fmt"

	"xrspace.io/server/modules/streaming/domain/rtc_token_provider"
)

var _ rtc_token_provider.IRTCTokenProvider = (*RTCTokenProvider)(nil)

type RTCTokenProvider struct{}

func NewRTCTokenProvider() *RTCTokenProvider {
	return &RTCTokenProvider{}
}

func (t *RTCTokenProvider) GetToken(params *rtc_token_provider.GetTokenParams) (string, error) {
	return fmt.Sprintf("%+v", params), nil
}
