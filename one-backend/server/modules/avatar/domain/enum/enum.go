package enum

import "xrspace.io/server/modules/avatar/domain/value_object"

const (
	AvatarTypeXrV2 value_object.AvatarType = "xr_v2"

	ThumbnailPartHead      = "head"
	ThumbnailPartUpperBody = "upper_body"
	ThumbnailPartFullBody  = "full_body"

	AssetFileTypeJpeg = "image/jpeg"
	AssetFileTypePng  = "image/png"
)
