package application

import (
	"context"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/modules/asset/application/command"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/domain/value_object"
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

func TestFailed_when_fileID_duplicated(t *testing.T) {
	c := &command.CreateUploadRequestCommand{
		RequestID: testRequestID,
		XrID:      testXrID,
		Files: []*storage.FileMeta{
			{
				FileID:        testFileID,
				ContentType:   testContenType,
				ContentLength: testContentLength,
				Checksum:      testChecksum,
			},
			{
				FileID:        testFileID,
				ContentType:   testContenType,
				ContentLength: testContentLength,
				Checksum:      testChecksum,
			},
		},
	}
	f := NewFacade(nil, nil)
	_, err := f.Execute(context.Background(), c)
	assert.Equal(t, "Key: 'CreateUploadRequestCommand.Files' Error:Field validation for 'Files' failed on the 'unique' tag", err.Error())
}

func Test_Failed_When_alack_of_File_Details(t *testing.T) {
	c := &command.CreateUploadRequestCommand{
		XrID: testXrID,
		Files: []*storage.FileMeta{
			{
				FileID:        testFileID,
				ContentType:   testContenType,
				ContentLength: testContentLength,
			},
		},
	}

	f := NewFacade(nil, nil)
	_, err := f.Execute(context.Background(), c)
	assert.Equal(t, "Key: 'CreateUploadRequestCommand.Files[0].Checksum' Error:Field validation for 'Checksum' failed on the 'required' tag", err.Error())
}
