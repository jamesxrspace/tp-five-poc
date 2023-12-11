package proxy

import (
	"context"
)

type GetProfileParams struct {
	AccessToken string
}

type GetManagerTokenParams struct {
	PoolID string
	Secret string
}

type CreateUserParams struct {
	Nickname      string
	Email         string
	Password      string
	Company       string
	EmailVerified bool
}

type GetProfileResponse struct {
	Email           string
	ResourceOwnerID string
	Username        string
	Nickname        string
	IsEmailVerified bool
}

type CreateUserResponse struct {
	UserID   string `json:"userId"`
	Username string `json:"username"`
}

type GetManagerTokenResponse struct {
	AccessToken string
	ExpiresIn   int
}

type IAuthProxy interface {
	GetProfile(ctx context.Context, params *GetProfileParams) (*GetProfileResponse, error)
	GetManagerToken(ctx context.Context, userPoolID string, params *GetManagerTokenParams) (*GetManagerTokenResponse, error)
	CreateAccount(ctx context.Context, userPoolID, managerToken string, params *CreateUserParams) (*CreateUserResponse, error)
}
