package command

import (
	"context"
	"encoding/base64"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/domain/value_object"
	"xrspace.io/server/modules/asset/port/output/repository/inmem"
	inmemStorage "xrspace.io/server/modules/asset/port/output/storage/inmem"
)

const (
	testRequestID     = value_object.RequestID("testRequestID")
	testXrID          = value_object.XrID("testXrID")
	testFileID        = value_object.FileID("testFileID")
	testContenType    = value_object.ContentType("testContenType")
	testContentLength = int64(1024)
	testChecksum      = "testChecksum"
	testUrl           = value_object.Url("presignedUrl")
)

func TestCreateUploadRequestUsecase(t *testing.T) {
	type args struct {
		c *CreateUploadRequestCommand
	}
	tests := []struct {
		args    args
		want    *CreateUploadRequestResponse
		name    string
		errMsg  string
		wantErr bool
	}{
		{
			name: "success_create_upload_request",
			args: args{
				c: &CreateUploadRequestCommand{
					RequestID: testRequestID,
					XrID:      testXrID,
					Files: []*storage.FileMeta{
						{
							FileID:        testFileID,
							ContentType:   testContenType,
							ContentLength: testContentLength,
							Checksum:      testChecksum,
						},
					},
				},
			},
			want: &CreateUploadRequestResponse{
				RequestID: testRequestID,
				PresignedUrls: map[value_object.FileID]value_object.Url{
					testFileID: testUrl,
				},
			},
			wantErr: false,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			ctx := context.Background()
			repo := inmem.NewUploadRepository(map[value_object.RequestID]*entity.UploadRequest{
				testRequestID: {
					RequestID: testRequestID,
					RequestFiles: []*entity.IntermediateObjectMeta{
						{
							FileID:   value_object.FileID(base64.RawURLEncoding.EncodeToString([]byte(testFileID))),
							Url:      testUrl,
							Checksum: value_object.Checksum(testChecksum),
						},
					},
				},
			})
			storage := inmemStorage.NewS3Storage()
			u := NewCreateUploadRequestUseCase(repo, storage)
			got, err := u.Execute(ctx, tt.args.c)
			if (err != nil) != tt.wantErr {
				t.Errorf("CreateUploadUrlUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}
			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}
			assert.Equal(t, tt.want, got)
		})
	}
}
