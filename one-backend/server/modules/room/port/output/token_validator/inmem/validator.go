package proxy

import (
	"context"
	"errors"

	"xrspace.io/server/modules/room/domain"
)

type TokenValidator struct {
	Success bool
}

var _ domain.IPhotonAccessTokenValidator = (*TokenValidator)(nil)

func NewTokenValidator() *TokenValidator {
	return &TokenValidator{}
}

func (t *TokenValidator) Should(success bool) {
	t.Success = success
}

func (t *TokenValidator) PhotonValidate(ctx context.Context, accessToken string) error {
	if t.Success {
		return nil
	} else {
		return errors.New("token invalid")
	}
}
