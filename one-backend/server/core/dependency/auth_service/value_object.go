package auth_service

type ResourceOwnerID string
type Email string
type Username string
type Nickname string
type Issuer string
type AccessToken string
type AuthorizedParty string

type ValidateAccessTokenResult struct {
	Issuer          Issuer
	ResourceOwnerID ResourceOwnerID
	AuthorizedParty AuthorizedParty
}

type CreateAccountParams struct {
	Username      string `json:"username"`
	Nickname      string `json:"nickname"`
	Email         string `json:"email"`
	Password      string `json:"password"`
	Company       string `json:"company"`
	EmailVerified bool   `json:"emailVerified"`
}

type CreateAccountResult struct {
	UserID   string `json:"userId"`
	Username string `json:"username"`
}

type GetProfileResult struct {
	Email           string `json:"email"`
	ResourceOwnerID string `json:"sub"`
	Username        string `json:"username"`
	Nickname        string `json:"nickname"`
	EmailVerified   bool   `json:"email_verified"`
}

type GetManagerTokenParams struct {
	AccessKeyID     string `json:"accessKeyId" validate:"required"`
	AccessKeySecret string `json:"accessKeySecret" validate:"required"`
}

type GetManagerTokenResult struct {
	AccessToken AccessToken `json:"access_token"`
	ExpiresIn   int         `json:"expires_in"`
}
