package inmem

import (
	"xrspace.io/server/core/dependency/settings"
	"xrspace.io/server/modules/aigc/domain"
)

var _ domain.IStorage = (*AigcStorage)(nil)

type AigcStorage struct{}

func NewAigcStorage() *AigcStorage {
	return &AigcStorage{}
}

func (s *AigcStorage) GetUrl(key string) (string, error) {
	return "https://test.file.url", nil
}

func (s *AigcStorage) GetBuckets() map[string]*settings.Bucket {
	return nil
}
