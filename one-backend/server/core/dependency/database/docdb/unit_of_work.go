package docdb

import (
	"context"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
	"go.mongodb.org/mongo-driver/mongo/writeconcern"

	"xrspace.io/server/core/dependency/unit_of_work"
)

type UnitOfWork struct {
	docDB DocDB
}

func NewUnitOfWork(docDB DocDB) unit_of_work.IUnitOfWork {
	return &UnitOfWork{
		docDB: docDB,
	}
}

func (w *UnitOfWork) WithTransaction(ctx context.Context, fn func(context.Context) (any, error)) (any, error) {
	session, err := w.docDB.Client().StartSession()
	if err != nil {
		return nil, err
	}
	defer session.EndSession(ctx)
	txnOpts := options.Transaction().SetWriteConcern(writeconcern.Majority()).SetReadPreference(readpref.Primary())
	txnFunc := func(sc mongo.SessionContext) (any, error) {
		return fn(sc)
	}

	return session.WithTransaction(ctx, txnFunc, txnOpts)
}
