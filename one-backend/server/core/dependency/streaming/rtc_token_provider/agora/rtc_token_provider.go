package rtc_token_provider

import (
	"errors"

	rtctokenbuilder "github.com/AgoraIO-Community/go-tokenbuilder/rtctokenbuilder"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/settings"
	"xrspace.io/server/modules/streaming/domain/enum"
	"xrspace.io/server/modules/streaming/domain/rtc_token_provider"
	"xrspace.io/server/modules/streaming/domain/streaming_errors"
)

var _ rtc_token_provider.IRTCTokenProvider = (*RTCTokenProvider)(nil)

type RTCTokenProvider struct {
	config *settings.AgoraConfig
}

func NewRTCTokenProvider(config *settings.AgoraConfig) *RTCTokenProvider {
	return &RTCTokenProvider{
		config: config,
	}
}

func (t *RTCTokenProvider) GetToken(params *rtc_token_provider.GetTokenParams) (string, error) {
	var role rtctokenbuilder.Role
	switch params.Role {
	case enum.Publisher:
		role = rtctokenbuilder.RolePublisher
	case enum.Subscriber:
		role = rtctokenbuilder.RoleSubscriber
	default:
		return "", core_error.NewCoreError(
			streaming_errors.ValidateError,
			errors.New("invalid role"),
		)
	}

	return rtctokenbuilder.BuildTokenWithAccount(
		t.config.AppId,
		t.config.AppCertificate,
		params.ChannelName,
		params.Account,
		role,
		params.ExpireTimeInSecond)
}
