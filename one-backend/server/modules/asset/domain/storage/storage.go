package storage

import (
	"context"

	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/value_object"
)

type FileMeta struct {
	FileID        value_object.FileID      `json:"file_id" validate:"required"`
	ContentType   value_object.ContentType `json:"content_type" validate:"required"`
	Checksum      value_object.Checksum    `json:"checksum" validate:"required"`
	ContentLength int64                    `json:"content_length" validate:"required"`
}

type IStorage interface {
	GetPresignedUrls(ctx context.Context, files []*FileMeta, requestID value_object.RequestID) ([]*entity.IntermediateObjectMeta, error)
	CopyToPermanentBucket(ctx context.Context, uploadRequest *entity.UploadRequest) ([]*entity.PermanentObjectMeta, error)
}
