package proxy

import (
	"context"

	"xrspace.io/server/core/dependency/auth_service"
	"xrspace.io/server/modules/room/domain"
)

/*
PhotonTokenValidator is depended on auth module
*/
type PhotonTokenValidator struct {
	authService *auth_service.AuthService
}

func NewPhotonTokenValidator(authService *auth_service.AuthService) *PhotonTokenValidator {
	return &PhotonTokenValidator{
		authService: authService,
	}
}

var _ domain.IPhotonAccessTokenValidator = (*PhotonTokenValidator)(nil)

func (p *PhotonTokenValidator) PhotonValidate(ctx context.Context, accessToken string) error {
	_, err := p.authService.ValidateAccessToken(ctx, accessToken)
	return err
}
