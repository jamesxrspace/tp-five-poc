package repository

import (
	"context"

	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/value_object"
)

type IUploadRequestRepository interface {
	SaveUploadRequest(ctx context.Context, uploadReq *entity.UploadRequest) error
	GetUploadRequest(ctx context.Context, requestID value_object.RequestID) (*entity.UploadRequest, error)
	DeleteUploadRequest(ctx context.Context, requestID value_object.RequestID) error
}
