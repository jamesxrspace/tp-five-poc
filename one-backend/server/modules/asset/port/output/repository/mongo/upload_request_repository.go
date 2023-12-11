package mongo

import (
	"context"
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/repository"
	"xrspace.io/server/modules/asset/domain/value_object"
)

const uploadReqCollection = "upload_request"

var _ repository.IUploadRequestRepository = (*UploadRepository)(nil)

type UploadRepository struct {
	db                  *docdb.DocDB
	uploadReqCollection *mongo.Collection
}

func NewUploadRepository(db *docdb.DocDB) *UploadRepository {
	return &UploadRepository{
		db:                  db,
		uploadReqCollection: db.Collection(uploadReqCollection),
	}
}

func (r *UploadRepository) InitIndex(ctx context.Context) error {
	if err := r.db.PopulateIndex(ctx, uploadReqCollection, "request_id", 1, true); err != nil {
		return err
	}

	return r.db.PopulateTTLIndex(ctx, uploadReqCollection, "created_at", 1, true, time.Minute*30)
}

func (r *UploadRepository) SaveUploadRequest(ctx context.Context, upload *entity.UploadRequest) error {
	filter := bson.M{"request_id": upload.RequestID}

	update := bson.M{
		"$set": upload,
	}

	opt := options.Update().SetUpsert(true)

	if _, err := r.uploadReqCollection.UpdateOne(ctx, filter, update, opt); err != nil {
		return core_error.StackError(err)
	}

	return nil
}

func (r *UploadRepository) GetUploadRequest(ctx context.Context, requestID value_object.RequestID) (*entity.UploadRequest, error) {
	var upload entity.UploadRequest

	filter := bson.M{"request_id": requestID}

	if err := r.uploadReqCollection.FindOne(ctx, filter).Decode(&upload); err != nil {
		return nil, core_error.StackError(err)
	}

	return &upload, nil
}

func (r *UploadRepository) DeleteUploadRequest(ctx context.Context, requestID value_object.RequestID) error {
	filter := bson.M{"request_id": requestID}

	if _, err := r.uploadReqCollection.DeleteOne(ctx, filter); err != nil {
		return core_error.StackError(err)
	}

	return nil
}
