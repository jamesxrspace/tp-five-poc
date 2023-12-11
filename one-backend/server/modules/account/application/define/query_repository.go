package define

import (
	"context"

	"xrspace.io/server/modules/account/domain/entity"
)

type IAccountRepository interface {
	GenXrID(ctx context.Context) string
	GetAccountByResourceOwnerID(ctx context.Context, resourceOwnerID string) (*entity.Account, error)
}
