package inmem

import (
	"context"
	"io"

	"xrspace.io/server/modules/avatar/domain/storage"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

var _ storage.IAssetStorage = (*AssetStorageClient)(nil)

type AssetStorageClient struct {
}

func NewAssetStorageClient() (*AssetStorageClient, error) {
	return &AssetStorageClient{}, nil
}

func (s *AssetStorageClient) SaveAsset(ctx context.Context, toPath string, file io.Reader) (value_object.AssetUrl, error) {
	return "https://test.file.url", nil
}

func (s *AssetStorageClient) GetDomain() string {
	return "http://asset_domain.com"
}

func (s *AssetStorageClient) Cloudfront() string {
	return "http://cloudfront.net"
}
