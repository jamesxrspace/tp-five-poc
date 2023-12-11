package storage

import (
	"context"
	"io"

	"xrspace.io/server/modules/avatar/domain/value_object"
)

type IAssetStorage interface {
	SaveAsset(ctx context.Context, toPath string, file io.Reader) (value_object.AssetUrl, error)
}
