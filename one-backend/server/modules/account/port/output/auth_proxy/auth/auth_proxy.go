package auth

import (
	"context"

	"xrspace.io/server/core/dependency/auth_service"
	"xrspace.io/server/modules/account/domain/proxy"
)

var _ proxy.IAuthProxy = (*AuthProxy)(nil)

type AuthProxy struct {
	authService *auth_service.AuthService
}

func NewAuthProxy(authService *auth_service.AuthService) *AuthProxy {
	return &AuthProxy{
		authService: authService,
	}
}

func (a *AuthProxy) GetProfile(ctx context.Context, params *proxy.GetProfileParams) (*proxy.GetProfileResponse, error) {
	profile, err := a.authService.GetProfile(ctx, string(params.AccessToken))

	if err != nil {
		return nil, err
	}

	return &proxy.GetProfileResponse{
		Email:           profile.Email,
		ResourceOwnerID: profile.ResourceOwnerID,
		Username:        profile.Username,
		Nickname:        profile.Nickname,
		IsEmailVerified: profile.EmailVerified,
	}, nil
}

func (a *AuthProxy) GetManagerToken(ctx context.Context, userPoolID string, params *proxy.GetManagerTokenParams) (*proxy.GetManagerTokenResponse, error) {
	result, err := a.authService.GetManagementToken(ctx, userPoolID, &auth_service.GetManagerTokenParams{
		AccessKeyID:     string(params.PoolID),
		AccessKeySecret: string(params.Secret),
	})

	if err != nil {
		return nil, err
	}

	return &proxy.GetManagerTokenResponse{
		AccessToken: string(result.AccessToken),
		ExpiresIn:   result.ExpiresIn,
	}, nil
}

func (a *AuthProxy) CreateAccount(ctx context.Context, userPoolID, managerToken string, params *proxy.CreateUserParams) (*proxy.CreateUserResponse, error) {
	result, err := a.authService.CreateAccount(ctx, userPoolID, managerToken, &auth_service.CreateAccountParams{
		Nickname:      string(params.Nickname),
		Email:         params.Email,
		Password:      params.Password,
		EmailVerified: params.EmailVerified,
		Company:       params.Company,
	})

	if err != nil {
		return nil, err
	}

	return &proxy.CreateUserResponse{
		UserID:   result.UserID,
		Username: result.Username,
	}, nil
}
