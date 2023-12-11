package define

import (
	"xrspace.io/server/core/dependency/database"
	"xrspace.io/server/core/dependency/settings"
	"xrspace.io/server/modules/account/domain/proxy"
	"xrspace.io/server/modules/account/domain/repository"
)

type Dependency struct {
	AuthProxy   proxy.IAuthProxy
	Config      *settings.Config
	RedisClient database.IRedis
	AccountRepo repository.IAccountRepository
	QueryRepo   IAccountRepository
}
