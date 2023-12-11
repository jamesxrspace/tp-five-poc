package s3

import (
	"xrspace.io/server/core/dependency/settings"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/aigc/domain"
)

var _ domain.IStorage = (*AigcStorage)(nil)

type AigcStorage struct {
	client *s3_client.S3StorageClient
}

func NewAigcStorage(client *s3_client.S3StorageClient) *AigcStorage {
	return &AigcStorage{
		client: client,
	}
}

func (s *AigcStorage) GetUrl(key string) (string, error) {
	bucketName, objKey, err := s3_client.ParseS3Url(key)
	if err != nil {
		return "", err
	}

	bucket, err := s.client.GetBucketSetting(bucketName)
	if err != nil {
		return "", err
	}

	url, err := s.client.GetUrl(bucket, objKey)
	if err != nil {
		return "", err
	}

	return url, nil
}

func (s *AigcStorage) GetBuckets() map[string]*settings.Bucket {
	return s.client.GetBuckets()
}
