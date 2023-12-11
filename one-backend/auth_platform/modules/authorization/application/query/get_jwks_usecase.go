package query

import (
	"context"

	"auth_platform/core/service/jwt"

	"github.com/lestrrat-go/jwx/v2/jwk"
)

type GetJwksUsecase struct {
	jwtService *jwt.JwtService
}

func NewGetJwksUsecase(s *jwt.JwtService) *GetJwksUsecase {
	return &GetJwksUsecase{
		jwtService: s,
	}
}

func (uc *GetJwksUsecase) Execute(ctx context.Context) map[string][]jwk.Key {
	return uc.jwtService.GetJwks()
}
