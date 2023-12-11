package rtc_token_provider

import "xrspace.io/server/modules/streaming/domain/enum"

type GetTokenParams struct {
	ChannelName        string
	Account            string
	Role               enum.EnumGetTokenRole
	ExpireTimeInSecond uint32
}

type IRTCTokenProvider interface {
	GetToken(params *GetTokenParams) (string, error)
}
