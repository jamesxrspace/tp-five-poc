package s3_storage

import (
	"context"
	"io"
	"time"

	"xrspace.io/server/core/dependency/settings"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/avatar/domain/storage"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

const (
	defaultConnectionTimeout = time.Second * 180
	avatarBucketConfigKey    = "avatar"
)

var _ storage.IAssetStorage = (*S3Storage)(nil)

type S3Storage struct {
	buckets map[string]*settings.Bucket
	client  s3_client.S3StorageClient
}

func NewS3Storage(client s3_client.S3StorageClient) storage.IAssetStorage {
	buckets := client.GetBuckets()

	return &S3Storage{
		buckets,
		client,
	}
}

func (s *S3Storage) SaveAsset(ctx context.Context, toPath string, file io.Reader) (value_object.AssetUrl, error) {
	url, err := s.client.UploadFile(ctx, avatarBucketConfigKey, toPath, file)
	if err != nil {
		return "", err
	}

	return value_object.AssetUrl(url), nil
}
