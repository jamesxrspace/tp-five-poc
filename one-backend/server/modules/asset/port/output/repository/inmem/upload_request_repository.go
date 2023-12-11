package inmem

import (
	"context"

	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/repository"
	"xrspace.io/server/modules/asset/domain/value_object"
)

var _ repository.IUploadRequestRepository = (*UploadRepository)(nil)

type UploadRepository struct {
	requests map[value_object.RequestID]*entity.UploadRequest
}

func NewUploadRepository(requests map[value_object.RequestID]*entity.UploadRequest) *UploadRepository {
	if requests == nil {
		requests = make(map[value_object.RequestID]*entity.UploadRequest)
	}

	return &UploadRepository{
		requests: requests,
	}
}

func (r *UploadRepository) SaveUploadRequest(ctx context.Context, upload *entity.UploadRequest) error {
	r.requests[upload.RequestID] = upload

	return nil
}

func (r *UploadRepository) GetUploadRequest(ctx context.Context, requestID value_object.RequestID) (*entity.UploadRequest, error) {
	return r.requests[requestID], nil
}

func (r *UploadRepository) DeleteUploadRequest(ctx context.Context, requestID value_object.RequestID) error {
	delete(r.requests, requestID)

	return nil
}
