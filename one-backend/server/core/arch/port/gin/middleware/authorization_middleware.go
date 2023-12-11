package middleware

import (
	"errors"
	"fmt"
	"net/http"
	"strings"

	"xrspace.io/server/core/dependency/auth_service"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/settings"
	"xrspace.io/server/modules/account/port/input/adapter"
	"xrspace.io/server/modules/account/port/output/repository/mongo"

	"github.com/gin-gonic/gin"
)

type azp string
type appID string

type AuthorizationMiddleware struct {
	authService    *auth_service.AuthService
	accountAdapter *adapter.AccountAdapter
	appIDs         map[azp]appID
}

func NewAuthorizationMiddleware(
	authService *auth_service.AuthService,
	db *docdb.DocDB,
	appConfig settings.AppConfig,
) *AuthorizationMiddleware {
	accountRepository := mongo.NewQueryRepository(db)
	appIDs := make(map[azp]appID)
	for _, item := range appConfig.AppIDs {
		appIDs[azp(item["azp"])] = appID(item["app_id"])
	}
	return &AuthorizationMiddleware{
		authService:    authService,
		accountAdapter: adapter.NewAccountAdapter(accountRepository),
		appIDs:         appIDs,
	}
}

func (m *AuthorizationMiddleware) ValidateAccessToken() gin.HandlerFunc {
	return func(c *gin.Context) {
		accessToken, err := m.getAccessTokenFromHeader(c)
		if err != nil {
			c.JSON(http.StatusUnauthorized, gin.H{
				"message": fmt.Sprintf("[ValidateAccessToken] invalid authorization header: %s", err.Error()),
			})
			c.Abort()
			return
		}

		result, err := m.authService.ValidateAccessToken(c, accessToken)
		if err != nil {
			c.JSON(http.StatusUnauthorized, gin.H{
				"message": fmt.Sprintf("[ValidateAccessToken] invalid token or expired: %s", err.Error()),
			})
			c.Abort()
			return
		}

		c.Set("access_token", accessToken)
		c.Set("resource_owner_id", string(result.ResourceOwnerID))
		c.Set("issuer", string(result.Issuer))
		c.Set("azp", string(result.AuthorizedParty))
		c.Next()
	}
}

func (m *AuthorizationMiddleware) SetXrID() gin.HandlerFunc {
	return func(c *gin.Context) {
		resourceOwnerID := c.GetString("resource_owner_id")
		if resourceOwnerID == "" {
			c.JSON(http.StatusBadRequest, gin.H{
				"message": fmt.Sprintf("[SetXrID] resource_owner_id does not exist: %s", resourceOwnerID),
			})
			c.Abort()
			return
		}

		xrid, err := m.accountAdapter.GetXrID(c.Request.Context(), resourceOwnerID)
		if err != nil {
			c.JSON(http.StatusBadRequest, gin.H{
				"message": fmt.Sprintf("[SetXrID] get xrid error: %s", err.Error()),
			})
			c.Abort()
			return
		}

		c.Set("xrid", xrid)
		c.Next()
	}
}

func (m *AuthorizationMiddleware) SetAppID() gin.HandlerFunc {
	return func(c *gin.Context) {
		authorizedParty := c.GetString("azp")
		if authorizedParty == "" {
			c.JSON(http.StatusBadRequest, gin.H{
				"message": fmt.Sprintf("[SetAppID] azp does not exist: %s", authorizedParty),
			})
			c.Abort()
			return
		}
		appID, ok := m.appIDs[azp(authorizedParty)]
		if !ok {
			c.JSON(http.StatusBadRequest, gin.H{
				"message": fmt.Sprintf("[SetAppID] app_id does not exist: %s", authorizedParty),
			})
			c.Abort()
			return
		}

		c.Set("app_id", string(appID))
		c.Next()
	}
}

func (m *AuthorizationMiddleware) getAccessTokenFromHeader(c *gin.Context) (string, error) {
	authToken := c.Request.Header.Get("Authorization")
	authComps := strings.Split(authToken, " ")
	if len(authComps) != 2 || strings.ToLower(authComps[0]) != "bearer" {
		return "", errors.New("[getAccessTokenFromHeader] invalid authorization header")
	}

	return authComps[1], nil
}
