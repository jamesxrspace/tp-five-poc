package command

import (
	"context"
	"encoding/base64"

	"github.com/go-playground/validator/v10"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/repository"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/domain/value_object"
)

type CreateUploadRequestCommand struct {
	XrID       value_object.XrID       `json:"xrid" validate:"required" token:"xrid"`
	RequestID  value_object.RequestID  `json:"request_id"` // used only in unit test
	Type       value_object.UploadType `json:"type"`
	Categories []value_object.Category `json:"categories"`
	Files      []*storage.FileMeta     `json:"files" validate:"gt=0,unique=FileID,dive,required"`
	Tags       []value_object.Tag      `json:"tags"`
}

type CreateUploadRequestResponse struct {
	PresignedUrls map[value_object.FileID]value_object.Url `json:"presigned_urls"`
	RequestID     value_object.RequestID                   `json:"request_id"`
}

func (c *CreateUploadRequestCommand) Validate() error {
	validate := validator.New()
	err := validate.Struct(c)
	if err != nil {
		for _, e := range err.(validator.ValidationErrors) {
			return e
		}
	}

	return nil
}

var _ application.IUseCase = (*CreateUploadRequestUseCase)(nil)

type CreateUploadRequestUseCase struct {
	repository repository.IUploadRequestRepository
	storage    storage.IStorage
}

func NewCreateUploadRequestUseCase(repository repository.IUploadRequestRepository, storage storage.IStorage) *CreateUploadRequestUseCase {
	return &CreateUploadRequestUseCase{
		repository: repository,
		storage:    storage,
	}
}

func (u *CreateUploadRequestUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	c := cmdz.(*CreateUploadRequestCommand)

	uploadReq := entity.NewUploadRequest(&entity.UploadRequestParams{
		RequestID:  c.RequestID,
		XrID:       c.XrID,
		Tags:       c.Tags,
		Type:       c.Type,
		Categories: c.Categories,
	})

	result, err := u.storage.GetPresignedUrls(ctx, c.Files, uploadReq.RequestID)

	if err != nil || result == nil {
		return nil, core_error.NewInternalError("failed to get presigned urls")
	}

	uploadReq.RequestFiles = result

	if err := u.repository.SaveUploadRequest(ctx, uploadReq); err != nil {
		return nil, err
	}

	presignedUrls := make(map[value_object.FileID]value_object.Url, len(result))
	for _, v := range result {
		fileID, err := base64.RawURLEncoding.DecodeString(string(v.FileID))
		if err != nil {
			return nil, core_error.NewInternalError(err)
		}
		presignedUrls[value_object.FileID(fileID)] = v.Url
	}

	return &CreateUploadRequestResponse{
		RequestID:     uploadReq.RequestID,
		PresignedUrls: presignedUrls,
	}, nil
}
