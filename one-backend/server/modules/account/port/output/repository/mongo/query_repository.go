package mongo

import (
	"context"
	"errors"

	"go.mongodb.org/mongo-driver/bson"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/account/application/define"
	"xrspace.io/server/modules/account/domain/entity"

	"github.com/google/uuid"
	"go.mongodb.org/mongo-driver/mongo"
)

var _ define.IAccountRepository = (*QueryRepository)(nil)

const (
	AccountCollectionName string = "account"
)

type QueryRepository struct {
	db                *docdb.DocDB
	accountCollection *mongo.Collection
}

func NewQueryRepository(db *docdb.DocDB) *QueryRepository {
	return &QueryRepository{
		db:                db,
		accountCollection: db.Collection(AccountCollectionName),
	}
}

func (r *QueryRepository) GenXrID(ctx context.Context) (xrID string) {
	return uuid.NewString()
}

func (r *QueryRepository) GetAccountByResourceOwnerID(ctx context.Context, resourceOwnerID string) (*entity.Account, error) {
	accountInfo := &entity.Account{}
	if err := r.accountCollection.FindOne(
		ctx, bson.M{"resource_owner_ids": resourceOwnerID},
	).Decode(accountInfo); err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			return nil, nil
		}
		return nil, core_error.StackError(err)
	}

	return accountInfo, nil
}

func (r *QueryRepository) InitIndex(ctx context.Context) error {
	for _, item := range []string{"id", "xrid", "username", "resource_owner_ids"} {
		if err := r.db.PopulateIndex(ctx, AccountCollectionName, item, 1, true); err != nil {
			return err
		}
	}
	return nil
}
