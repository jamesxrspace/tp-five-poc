package inmem

import (
	"context"
	"encoding/base64"

	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/domain/value_object"
)

var _ storage.IStorage = (*S3Storage)(nil)

type S3Storage struct {
}

func NewS3Storage() *S3Storage {
	return &S3Storage{}
}

func (s *S3Storage) GetPresignedUrls(ctx context.Context, files []*storage.FileMeta, requestID value_object.RequestID) ([]*entity.IntermediateObjectMeta, error) {
	return []*entity.IntermediateObjectMeta{
		{
			FileID: value_object.FileID(base64.RawURLEncoding.EncodeToString([]byte("testFileID"))),
			Url:    "presignedUrl",
		},
	}, nil
}

func (s *S3Storage) CopyToPermanentBucket(ctx context.Context, uploadRequest *entity.UploadRequest) ([]*entity.PermanentObjectMeta, error) {
	return []*entity.PermanentObjectMeta{
		{
			FileID: value_object.FileID(base64.RawURLEncoding.EncodeToString([]byte("testFileID"))),
			Url:    "permanentUrl",
			Path:   "permanentPath",
		},
	}, nil
}
