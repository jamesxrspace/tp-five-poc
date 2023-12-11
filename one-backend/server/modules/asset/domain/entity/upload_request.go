package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/modules/asset/domain/value_object"
)

type UploadRequestParams UploadRequest

type UploadRequest struct {
	CreatedAt       time.Time                 `json:"created_at" bson:"created_at"`
	UpdatedAt       time.Time                 `json:"updated_at" bson:"updated_at"`
	RequestFiles    []*IntermediateObjectMeta `json:"request_files" bson:"request_files"`
	UploadedObjects []*PermanentObjectMeta    `json:"uploaded_objects" bson:"uploaded_objects"`
	RequestID       value_object.RequestID    `json:"request_id" bson:"request_id"`
	XrID            value_object.XrID         `json:"xr_id" bson:"xr_id"`
	Type            value_object.UploadType   `json:"type" bson:"type"`
	Categories      []value_object.Category   `json:"category" bson:"category"`
	Tags            []value_object.Tag        `json:"tags" bson:"tags"`
}

type IntermediateObjectMeta struct {
	FileID   value_object.FileID   `json:"file_id" bson:"file_id"`
	Url      value_object.Url      `json:"url" bson:"-"`
	Checksum value_object.Checksum `json:"checksum" bson:"checksum"`
}

type PermanentObjectMeta struct {
	FileID value_object.FileID `json:"file_id" bson:"file_id"`
	Url    value_object.Url    `json:"url" bson:"url"`
	Path   value_object.Path   `json:"path" bson:"path"`
}

func NewUploadRequest(params *UploadRequestParams) *UploadRequest {
	if params.RequestID == "" {
		params.RequestID = value_object.RequestID(uuid.New().String())
	}
	if params.CreatedAt.IsZero() {
		params.CreatedAt = time.Now()
	}
	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = time.Now()
	}

	return &UploadRequest{
		RequestID:  params.RequestID,
		XrID:       params.XrID,
		Tags:       params.Tags,
		Type:       params.Type,
		Categories: params.Categories,
		CreatedAt:  params.CreatedAt,
		UpdatedAt:  params.UpdatedAt,
	}
}
