package command

import (
	"context"
	"encoding/base64"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/value_object"
	"xrspace.io/server/modules/asset/port/output/repository/inmem"
	inmemStorage "xrspace.io/server/modules/asset/port/output/storage/inmem"
)

const (
	testS3Url      = "permanentUrl"
	testS3Path     = "permanentPath"
	testRequestID2 = value_object.RequestID("testRequestID2")
)

func TestNotifyUploadedUseCase(t *testing.T) {
	type args struct {
		c *ConfirmUploadedCommand
	}
	tests := []struct {
		args    args
		want    interface{}
		name    string
		errMsg  string
		wantErr bool
	}{
		{
			name: "success_notify_uploaded",
			args: args{
				c: &ConfirmUploadedCommand{
					RequestID: testRequestID,
				},
			},
			want: map[value_object.FileID]ConfirmUploadedObjects{
				testFileID: {
					Url:  testS3Url,
					Path: testS3Path,
				},
			},
			wantErr: false,
		},
		{
			name: "failed_when_request_id_not_found",
			args: args{
				c: &ConfirmUploadedCommand{
					RequestID: testRequestID2,
				},
			},
			want:    nil,
			wantErr: true,
			errMsg:  "request not found",
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
			u := NewConfirmUploadedUsecase(repo, storage)
			got, err := u.Execute(ctx, tt.args.c)
			if (err != nil) != tt.wantErr {
				t.Errorf("NotifyUploadedUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}
			assert.Equal(t, tt.want, got)
		})
	}
}
