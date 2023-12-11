package auth_service

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"net/url"
	"time"

	"github.com/lestrrat-go/jwx/v2/jwk"
	"github.com/lestrrat-go/jwx/v2/jwt"
	"github.com/rs/zerolog/log"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/settings"
)

type Endpoints struct {
	Userinfo   string
	Jwks       string
	Management string
	CreateUser string
}

type AuthService struct {
	Jwks        jwk.Set
	DomainUrl   string
	Endpoints   Endpoints
	HttpTimeout time.Duration
}

func NewAuthService(config *settings.OIDCConfig) (*AuthService, error) {
	service := &AuthService{
		DomainUrl: config.DomainUrl,
		Endpoints: Endpoints{
			Userinfo:   config.Endpoints.Userinfo,
			Jwks:       config.Endpoints.Jwks,
			Management: config.Endpoints.Management,
			CreateUser: config.Endpoints.CreateUser,
		},
		HttpTimeout: time.Duration(config.HttpTimeout) * time.Second,
	}

	jwks, err := service.downloadJwks()
	if err != nil {
		return nil, err
	}

	service.Jwks = jwks

	return service, nil
}

func (p *AuthService) ValidateAccessToken(ctx context.Context, accessToken string) (*ValidateAccessTokenResult, error) {
	token, err := jwt.Parse([]byte(accessToken), jwt.WithKeySet(p.Jwks))
	if err != nil {
		return nil, core_error.StackError(err)
	}

	authorizedParty, ok := token.PrivateClaims()["azp"]
	if !ok {
		return nil, core_error.StackError("[ValidateAccessToken] invalid azp")
	}

	return &ValidateAccessTokenResult{
		Issuer:          Issuer(token.Issuer()),
		ResourceOwnerID: ResourceOwnerID(token.Subject()),
		AuthorizedParty: AuthorizedParty(authorizedParty.(string)),
	}, nil
}

func (p *AuthService) GetProfile(ctx context.Context, accessToken string) (*GetProfileResult, error) {
	path, err := url.JoinPath(p.DomainUrl, p.Endpoints.Userinfo)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	headers := map[string]string{
		"Authorization": "Bearer " + accessToken,
	}

	resp, err := p.getRequest(path, "", headers)
	if err != nil {
		return nil, err
	}

	var result GetProfileResult
	if err := json.Unmarshal(resp, &result); err != nil {
		return nil, core_error.StackError(err)
	}

	return &result, nil
}

func (p *AuthService) GetManagementToken(ctx context.Context, userPoolID string, params *GetManagerTokenParams) (*GetManagerTokenResult, error) {
	path, err := url.JoinPath(p.DomainUrl, p.Endpoints.Management)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	payload, err := json.Marshal(params)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	headers := map[string]string{
		"Content-Type":          "application/json",
		"x-authing-userpool-id": userPoolID,
	}

	resp, err := p.postRequest(path, payload, headers)
	if err != nil {
		return nil, err
	}

	var result GetManagerTokenResult
	if err := json.Unmarshal(resp, &result); err != nil {
		return nil, core_error.StackError(err)
	}

	return &result, nil
}

func (p *AuthService) CreateAccount(ctx context.Context, userPoolID, managerToken string, params *CreateAccountParams) (*CreateAccountResult, error) {
	path, err := url.JoinPath(p.DomainUrl, p.Endpoints.CreateUser)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	payload, err := json.Marshal(params)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	headers := map[string]string{
		"Authorization":         "Bearer " + managerToken,
		"Content-Type":          "application/json",
		"x-authing-userpool-id": userPoolID,
	}

	resp, err := p.postRequest(path, payload, headers)
	if err != nil {
		return nil, err
	}

	var result CreateAccountResult
	if err := json.Unmarshal(resp, &result); err != nil {
		return nil, core_error.StackError(err)
	}

	return &result, nil
}

func (p *AuthService) downloadJwks() (jwk.Set, error) {
	log.Info().Str("url", p.DomainUrl+p.Endpoints.Jwks).Msg("[downloadJwks] downloading jwks")

	httpClient := &http.Client{
		Timeout: p.HttpTimeout,
	}

	endpoint, err := url.JoinPath(p.DomainUrl, p.Endpoints.Jwks)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	resp, err := httpClient.Get(endpoint)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	defer resp.Body.Close()
	if resp.StatusCode != http.StatusOK {
		return nil, core_error.StackError(fmt.Errorf("[downloadJwks] status code not ok, status code: %d", resp.StatusCode))
	}

	jwksBytes, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	return jwk.Parse(jwksBytes)
}

func (p *AuthService) postRequest(path string, payload []byte, headers map[string]string) ([]byte, error) {
	return p.request(http.MethodPost, path, bytes.NewBuffer(payload), headers)
}

func (p *AuthService) getRequest(path, queryString string, headers map[string]string) ([]byte, error) {
	if queryString != "" {
		path = path + "?" + queryString
	}

	return p.request(http.MethodGet, path, nil, headers)
}

func (p *AuthService) request(method, path string, body io.Reader, headers map[string]string) ([]byte, error) {
	var req *http.Request
	var err error

	switch method {
	case http.MethodPost:
		req, err = http.NewRequest(http.MethodPost, path, body)
	case http.MethodGet:
		req, err = http.NewRequest(http.MethodGet, path, nil)
	}
	if err != nil {
		return nil, core_error.StackError(err)
	}

	if len(headers) > 0 {
		for k, v := range headers {
			req.Header.Set(k, v)
		}
	}

	httpClient := &http.Client{
		Timeout: p.HttpTimeout,
	}

	resp, err := httpClient.Do(req)
	if err != nil {
		return nil, core_error.StackError(err)
	}

	result, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, core_error.StackError(fmt.Errorf(
			"[AuthService] status code not ok, status code: %d, body: %s",
			resp.StatusCode,
			string(result),
		))
	}

	return result, nil
}
