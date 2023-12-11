package service

import (
	"fmt"

	"xrspace.io/server/modules/avatar/domain/value_object"
)

type IAvatarService interface {
	GetAvatarUploadPath(xrid value_object.XrID, avatarID value_object.AvatarID, ext value_object.FileExt) string
	GetAvatarThumbnailUploadPath(xrid value_object.XrID, avatarID value_object.AvatarID, part string, ext value_object.FileExt) string
}

var _ IAvatarService = (*AvatarService)(nil)

type AvatarService struct {
}

func NewAvatarService() *AvatarService {
	return &AvatarService{}
}

func (s *AvatarService) GetAvatarUploadPath(xrid value_object.XrID, avatarID value_object.AvatarID, ext value_object.FileExt) string {
	return fmt.Sprintf("avatar/%s/%s/avatar%s", xrid, avatarID, ext)
}

func (s *AvatarService) GetAvatarThumbnailUploadPath(xrid value_object.XrID, avatarID value_object.AvatarID, part string, ext value_object.FileExt) string {
	return fmt.Sprintf("avatar/%s/%s/%s%s", xrid, avatarID, part, ext)
}
