package auth_gateway

import (
	"context"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/account/domain/proxy"
)

var _ proxy.IAuthProxy = (*AuthProxy)(nil)

type AuthProxy struct {
	profiles      map[string]*proxy.GetProfileResponse
	users         map[string]*proxy.CreateUserResponse
	managerTokens map[string]*proxy.GetManagerTokenResponse
}

func NewAuthProxy(
	profiles map[string]*proxy.GetProfileResponse,
	users map[string]*proxy.CreateUserResponse,
	managerTokens map[string]*proxy.GetManagerTokenResponse,
) *AuthProxy {

	if profiles == nil {
		profiles = make(map[string]*proxy.GetProfileResponse)
	}

	if users == nil {
		users = make(map[string]*proxy.CreateUserResponse)
	}

	if managerTokens == nil {
		managerTokens = make(map[string]*proxy.GetManagerTokenResponse)
	}

	return &AuthProxy{
		profiles:      profiles,
		users:         users,
		managerTokens: managerTokens,
	}
}

func (a *AuthProxy) GetProfile(ctx context.Context, params *proxy.GetProfileParams) (*proxy.GetProfileResponse, error) {
	profile, ok := a.profiles[params.AccessToken]
	if !ok {
		return nil, core_error.StackError("profile not found")
	}

	return profile, nil
}

func (a *AuthProxy) GetManagerToken(ctx context.Context, userPoolID string, params *proxy.GetManagerTokenParams) (*proxy.GetManagerTokenResponse, error) {
	managerToken, ok := a.managerTokens[userPoolID]
	if !ok {
		return nil, core_error.StackError("manager token not found")
	}

	return managerToken, nil
}

func (a *AuthProxy) CreateAccount(ctx context.Context, userPoolID, token string, params *proxy.CreateUserParams) (*proxy.CreateUserResponse, error) {
	user, ok := a.users[params.Nickname]
	if !ok {
		return nil, core_error.StackError("user not found")
	}

	return user, nil
}
