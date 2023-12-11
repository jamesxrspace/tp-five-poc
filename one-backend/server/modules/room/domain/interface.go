package domain

import "context"

type IPhotonAccessTokenValidator interface {
	// PhotonValidate is implemented by
	// one-backend/server/modules/auth/port/input/access_token_validator.go
	PhotonValidate(ctx context.Context, accessToken string) error
}
