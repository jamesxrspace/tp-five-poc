package s3storage

import (
	"bytes"
	"context"
	"crypto/sha256"
	"encoding/base64"
	"fmt"
	"io"
	"path/filepath"
	"sync"

	"github.com/rs/zerolog"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/settings"
	s3_client "xrspace.io/server/core/dependency/storage/s3"
	"xrspace.io/server/modules/asset/domain/entity"
	"xrspace.io/server/modules/asset/domain/storage"
	"xrspace.io/server/modules/asset/domain/value_object"
)

const (
	defaultBucket      = "default"
	intermediateBucket = "intermediate"
)

var _ storage.IStorage = (*S3Storage)(nil)

type S3Storage struct {
	buckets map[string]*settings.Bucket
	client  s3_client.S3StorageClient
}

func NewS3Storage(client s3_client.S3StorageClient) storage.IStorage {
	buckets := client.GetBuckets()

	return &S3Storage{
		buckets,
		client,
	}
}

func (s *S3Storage) GetPresignedUrls(ctx context.Context, files []*storage.FileMeta, requestID value_object.RequestID) ([]*entity.IntermediateObjectMeta, error) {
	num := len(files)
	result := make([]*entity.IntermediateObjectMeta, 0, num)
	ch := make(chan *entity.IntermediateObjectMeta, num)
	sig := make(chan struct{})

	cancel := func() {
		close(sig)
	}

	var wg sync.WaitGroup
	wg.Add(num)

	for _, file := range files {
		go func(file *storage.FileMeta) {
			defer wg.Done()

			key := filepath.Join(string(requestID), string(file.FileID))

			url := s.client.CreatePutPresignedUrl(ctx, &s3_client.ObjectMeta{
				FileName:      string(file.FileID),
				ContentType:   string(file.ContentType),
				Checksum:      string(file.Checksum),
				ContentLength: file.ContentLength,
			}, key, s.buckets[intermediateBucket].Name)

			if url == "" {
				log := zerolog.Ctx(ctx)
				log.Error().Msg("failed getting presigned url")
				cancel()
				return
			}

			ch <- &entity.IntermediateObjectMeta{
				FileID:   value_object.FileID(base64.RawURLEncoding.EncodeToString([]byte(file.FileID))),
				Url:      value_object.Url(url),
				Checksum: file.Checksum,
			}
		}(file)
	}

	go func() {
		wg.Wait()
		close(ch)
	}()

	for len(result) < num {
		select {
		case <-sig:
			return nil, core_error.StackError("failed getting presigned urls")
		case file := <-ch:
			result = append(result, file)
		}
	}

	return result, nil
}

func (s *S3Storage) CopyToPermanentBucket(ctx context.Context, uploadRequest *entity.UploadRequest) ([]*entity.PermanentObjectMeta, error) {
	num := len(uploadRequest.RequestFiles)
	result := make([]*entity.PermanentObjectMeta, 0, num)
	ch := make(chan *entity.PermanentObjectMeta, num)
	sig := make(chan string)

	cancel := func(msg string) {
		sig <- msg
	}

	var wg sync.WaitGroup
	wg.Add(num)

	for _, file := range uploadRequest.RequestFiles {
		go func(file *entity.IntermediateObjectMeta) {
			defer wg.Done()

			fileID, err := base64.RawURLEncoding.DecodeString(string(file.FileID))
			if err != nil {
				cancel(fmt.Sprintf("failed decoding fileID %s: %+v", file.FileID, err))
				return
			}

			objKey := filepath.Join(string(uploadRequest.RequestID), string(fileID))

			err = s.isChecksumMatch(objKey, string(file.Checksum))
			if err != nil {
				cancel(fmt.Sprintf("failed checksum match %s: %+v", file.FileID, err))
				return
			}

			err = s.client.CopyObject(filepath.Join(s.buckets[intermediateBucket].Name, objKey), s.buckets[defaultBucket].Name, objKey)
			if err != nil {
				cancel(fmt.Sprintf("failed moving file %s: %+v", file.FileID, err))
				return
			}

			url, err := s.client.GetUrl(s.buckets[defaultBucket], objKey)
			if err != nil {
				cancel(fmt.Sprintf("failed getting path %s: %+v", file.FileID, err))
				return
			}

			ch <- &entity.PermanentObjectMeta{
				FileID: file.FileID,
				Url:    value_object.Url(url),
				Path:   value_object.Path(filepath.Join(s.buckets[defaultBucket].Name, objKey)),
			}
		}(file)
	}

	go func() {
		wg.Wait()

		if len(result) != num {
			s.deleteFailObjects(ctx, uploadRequest)
		}
	}()

	for len(result) < num {
		select {
		case msg := <-sig:
			return nil, core_error.StackError(fmt.Sprintf("copying to permanent bucket: %s", msg))
		case file := <-ch:
			result = append(result, file)
		}
	}

	return result, nil
}

func (s *S3Storage) isChecksumMatch(objKey, fileChecksum string) error {
	fileBytes, err := s.client.DownloadObject(s.buckets[intermediateBucket].Name, objKey)
	if err != nil {
		return err
	}

	hash := sha256.New()
	_, err = io.Copy(hash, bytes.NewReader(fileBytes))
	if err != nil {
		return err
	}

	hashBytes := hash.Sum(nil)
	checksum := base64.StdEncoding.EncodeToString(hashBytes)
	if checksum != fileChecksum {
		return core_error.StackError("checksum mismatch")
	}

	return nil
}

func (s *S3Storage) deleteFailObjects(ctx context.Context, req *entity.UploadRequest) {
	for _, file := range req.RequestFiles {
		objKey := filepath.Join(string(req.RequestID), string(file.FileID))
		err := s.client.DeleteObject(s.buckets[intermediateBucket].Name, objKey)
		if err != nil {
			log := zerolog.Ctx(ctx)
			log.Error().Err(err).Msgf("failed deleting file %s", file.FileID)
		}
	}
}
