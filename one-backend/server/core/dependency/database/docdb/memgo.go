package docdb

import (
	"context"
	"runtime"

	"github.com/tryvium-travels/memongo"
	"github.com/tryvium-travels/memongo/memongolog"

	"xrspace.io/server/core/dependency/settings"
)

const (
	MEMGO_MONGO_VERSION   = "6.0.9"
	MEMGO_M1_DOWNLOAD_URL = "https://fastdl.mongodb.org/osx/mongodb-macos-x86_64-6.0.9.tgz"
	MEMGO_DEFAULT_DB      = "test"
)

func NewMemgoDocDB(ctx context.Context) (*DocDB, *memongo.Server, error) {
	opts := &memongo.Options{
		MongoVersion: MEMGO_MONGO_VERSION,
		LogLevel:     memongolog.LogLevelWarn,
	}

	// Workaround for Apple Silicon (M1). https://github.com/tryvium-travels/memongo#known-bugs-with-apple-silicon-m1
	if runtime.GOARCH == "arm64" && runtime.GOOS == "darwin" {
		opts.DownloadURL = MEMGO_M1_DOWNLOAD_URL
	}

	mongoServer, err := memongo.StartWithOptions(opts)
	if err != nil {
		panic(err)
	}

	config := settings.DocDBConfig{
		ConnectionUri:       mongoServer.URI(),
		ConnectionTimeoutMS: 1000,
		SocketTimeoutMS:     1000,
		MaxPoolSize:         100,
		DefaultDB:           MEMGO_DEFAULT_DB,
	}

	db, err := NewDocDB(ctx, &config)
	return db, mongoServer, err
}
