package value_object

import (
	"mime/multipart"
	"net/http"
	"path/filepath"
)

type AvatarAsset struct {
	multipart.File
	*multipart.FileHeader
}

func NewAvatarAsset(file multipart.File, fileHeader *multipart.FileHeader) *AvatarAsset {
	return &AvatarAsset{
		File:       file,
		FileHeader: fileHeader,
	}
}

func (a *AvatarAsset) IsEmpty() bool {
	return a.File == nil
}

func (a *AvatarAsset) GetExt() FileExt {
	if a.isFileHeaderEmpty() {
		return ""
	}

	return FileExt(filepath.Ext(a.Filename))
}

func (a *AvatarAsset) GetFileType() (string, error) {
	if !a.isFileHeaderEmpty() && a.Header.Get("Content-Type") != "" {
		return a.Header.Get("Content-Type"), nil
	}

	buf := make([]byte, 512)
	_, err := a.Read(buf)
	if err != nil {
		return "", err
	}

	contentType := http.DetectContentType(buf)
	return contentType, nil
}

func (a *AvatarAsset) isFileHeaderEmpty() bool {
	return a.FileHeader == nil
}
