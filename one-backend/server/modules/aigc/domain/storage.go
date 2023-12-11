package domain

import "xrspace.io/server/core/dependency/settings"

type IStorage interface {
	GetUrl(key string) (string, error)
	GetBuckets() map[string]*settings.Bucket
}
