package command

import (
	"context"
	"encoding/base64"

	"github.com/rs/zerolog"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/asset/domain/repository"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/domain/value_object"
)

type ConfirmUploadedCommand struct {
	RequestID value_object.RequestID `json:"-" validate:"required" uri:"request_id"`
}

type ConfirmUploadedObjects struct {
	Url  value_object.Url  `json:"url"`
	Path value_object.Path `json:"path"`
}

type ConfirmUploadedUsecase struct {
	repository repository.IUploadRequestRepository
	storage    storage.IStorage
}

var _ application.IUseCase = (*ConfirmUploadedUsecase)(nil)

func NewConfirmUploadedUsecase(uploadRepository repository.IUploadRequestRepository, storage storage.IStorage) *ConfirmUploadedUsecase {
	return &ConfirmUploadedUsecase{
		repository: uploadRepository,
		storage:    storage,
	}
}

func (u *ConfirmUploadedUsecase) Execute(ctx context.Context, cmdz any) (any, error) {
	c := cmdz.(*ConfirmUploadedCommand)

	req, err := u.repository.GetUploadRequest(ctx, c.RequestID)
	if err != nil {
		return nil, err
	}

	if req == nil {
		return nil, core_error.NewEntityNotFoundError("upload request", c.RequestID)
	}

	defer func() {
		if err := u.repository.DeleteUploadRequest(ctx, req.RequestID); err != nil {
			log := zerolog.Ctx(ctx)
			log.Error().Err(err).Msg("delete upload request")
		}
	}()

	result, err := u.storage.CopyToPermanentBucket(ctx, req)

	if err != nil || len(result) == 0 {
		return nil, err
	}

	req.UploadedObjects = result
	// TODO: before deleting upload request, create asset from upload request

	objects := make(map[value_object.FileID]ConfirmUploadedObjects, len(result))
	for _, v := range result {
		fileID, err := base64.RawURLEncoding.DecodeString(string(v.FileID))
		if err != nil {
			return nil, err
		}
		objects[value_object.FileID(fileID)] = ConfirmUploadedObjects{
			Url:  v.Url,
			Path: v.Path,
		}
	}

	return objects, nil
}
