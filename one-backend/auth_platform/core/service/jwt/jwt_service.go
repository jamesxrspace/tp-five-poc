package jwt

import (
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"crypto/rand"
	"encoding/base64"
	"errors"
	"os"
	"time"

	"github.com/lestrrat-go/jwx/v2/jwa"
	"github.com/lestrrat-go/jwx/v2/jwk"
	"github.com/lestrrat-go/jwx/v2/jwt"
)

const (
	mockKid      = "XR"
	mockIssuer   = "https://xrspace.io/oidc"
	mockJwtID    = "JTI"
	mockAudience = "http://localhost:8090"
)

type JwtService struct {
	privateKey jwk.Key
	publicKey  jwk.Key
	signBytes  []byte
	appID      string
	userPoolID string
	secret     string
}

func NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret string) *JwtService {
	signBytes, err := os.ReadFile(privateKeyPath)
	if err != nil {
		panic(err)
	}

	privateKey, publicKey, err := generatePrivatePublicKey(signBytes)
	if err != nil {
		panic(err)
	}

	return &JwtService{
		privateKey: privateKey,
		publicKey:  publicKey,
		signBytes:  signBytes,
		appID:      mockAppID,
		userPoolID: mockPooID,
		secret:     mockSecret,
	}
}

func (j JwtService) GenAccessToken(userID string, expireSecs int) (string, error) {
	token, err := jwt.NewBuilder().
		Subject(userID).
		Audience([]string{mockAudience}).
		IssuedAt(time.Now()).
		Expiration(time.Now().Add(time.Duration(expireSecs)*time.Second)).
		JwtID(mockJwtID).
		Issuer(mockIssuer).
		Claim("scope", "openid email profile phone").
		Claim("external_id", "EXTERNAL_ID").
		Claim("azp", j.appID).
		Build()

	if err != nil {
		return "", auth_errors.NewErrSignToken(err, userID)
	}

	signed, err := jwt.Sign(token, jwt.WithKey(jwa.RS256, j.privateKey))
	if err != nil {
		return "", auth_errors.NewErrSignToken(err, userID)
	}

	return string(signed), nil
}

func (j JwtService) GenRefreshToken(userID string) (string, error) {
	tokenBytes := make([]byte, 32)
	_, err := rand.Read(tokenBytes)
	if err != nil {
		return "", auth_errors.NewErrGenRand(err, userID)
	}

	return base64.RawStdEncoding.EncodeToString(tokenBytes), nil
}

func (j JwtService) GetJwks() map[string][]jwk.Key {
	return map[string][]jwk.Key{
		"keys": {j.publicKey},
	}
}

func (j JwtService) ParseJwt(tokenString string) (jwt.Token, error) {
	verifiedToken, err := jwt.Parse([]byte(tokenString), jwt.WithKey(jwa.RS256, j.publicKey))

	if err != nil {
		return nil, auth_errors.NewErrParseToken(err, tokenString)
	}

	return verifiedToken, nil
}

func (j JwtService) GetUserPoolID() string {
	return j.userPoolID
}

func (j JwtService) GenManagerToken(accessKeyID, accessKeySecret string, expireSecs int) (string, error) {
	if accessKeyID != j.userPoolID || accessKeySecret != j.secret {
		return "", auth_errors.NewErrInvalidEmailOrPassword(errors.New("invalid access key id or secret"), accessKeyID)
	}

	token, err := jwt.NewBuilder().
		Claim("token_type", "management_token").
		Claim("scoped_userpool_id", accessKeyID).
		Claim("access_key_type", "userpool").
		Claim("access_key_id", accessKeyID).
		IssuedAt(time.Now()).
		Expiration(time.Now().Add(time.Duration(expireSecs) * time.Second)).
		Build()

	if err != nil {
		return "", err
	}

	signed, err := jwt.Sign(token, jwt.WithKey(jwa.HS256, j.signBytes))
	if err != nil {
		return "", auth_errors.NewErrSignToken(err, accessKeyID)
	}

	return string(signed), nil
}

func (j JwtService) VerifyManagerToken(tokenString string) (jwt.Token, error) {
	verifiedToken, err := jwt.Parse([]byte(tokenString), jwt.WithKey(jwa.HS256, j.signBytes))

	if err != nil {
		return nil, auth_errors.NewErrParseToken(err, tokenString)
	}

	return verifiedToken, nil
}

func generatePrivatePublicKey(signBytes []byte) (jwk.Key, jwk.Key, error) {
	signKey, err := jwk.ParseKey(signBytes, jwk.WithPEM(true))
	if err != nil {
		return nil, nil, auth_errors.NewErrParsePrivateKey(err, string(signBytes))
	}

	publicKey, err := jwk.PublicKeyOf(signKey)
	if err != nil {
		return nil, nil, auth_errors.NewErrParsePrivateKey(err, string(signBytes))
	}

	publicKey.Set(jwk.AlgorithmKey, "RS256") //nolint:unchecked,errcheck
	publicKey.Set(jwk.KeyUsageKey, "sig")    //nolint:unchecked,errcheck
	publicKey.Set(jwk.KeyIDKey, mockKid)     //nolint:unchecked,errcheck
	signKey.Set(jwk.KeyIDKey, mockKid)       //nolint:unchecked,errcheck

	return signKey, publicKey, nil
}
