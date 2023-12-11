package s3_client

import (
	"context"
	"fmt"
	"io"
	"net/url"
	"strings"
	"time"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/awserr"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/s3"
	"github.com/aws/aws-sdk-go/service/s3/s3manager"
	"github.com/rs/zerolog"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/settings"
)

const (
	defaultConnectionTimeout = time.Second * 180
	presignedUrlExpireTime   = time.Minute * 15
	defaultBucket            = "default"
	intermediateBucket       = "intermediate"
	prefix                   = "s3://"
)

type ObjectMeta struct {
	FileName      string
	ContentType   string
	Checksum      string
	ContentLength int64
}

type S3StorageClient struct {
	client     *s3.S3
	uploader   *s3manager.Uploader
	downloader *s3manager.Downloader
	buckets    map[string]*settings.Bucket
	cloudfront string
	timeout    time.Duration
}

func NewS3StorageClient(awsSession *session.Session, config *settings.S3Config) *S3StorageClient {
	client := s3.New(awsSession)
	ensureBucketExist(client, config.Buckets)

	downloader := s3manager.NewDownloader(awsSession, func(d *s3manager.Downloader) {
		d.PartSize = 64 * 1024 * 1024
		d.Concurrency = 4
		d.BufferProvider = s3manager.NewPooledBufferedWriterReadFromProvider(1024 * 1024 * 8)
	})

	return &S3StorageClient{
		timeout:    defaultConnectionTimeout,
		client:     client,
		uploader:   s3manager.NewUploader(awsSession),
		downloader: downloader,
		buckets:    config.Buckets,
		cloudfront: config.CloudfrontDomain,
	}
}

func ensureBucketExist(client *s3.S3, buckets map[string]*settings.Bucket) {
	for _, bucket := range buckets {
		_, err := client.HeadBucket(&s3.HeadBucketInput{
			Bucket: aws.String(bucket.Name),
		})
		if err != nil {
			panic(err)
		}
	}
}

func (s *S3StorageClient) GetBuckets() map[string]*settings.Bucket {
	return s.buckets
}

func (s *S3StorageClient) GetBucketSetting(bucketName string) (*settings.Bucket, error) {
	for _, bucket := range s.GetBuckets() {
		if bucket.Name == bucketName {
			return bucket, nil
		}
	}

	return nil, core_error.NewInternalError("bucket not found")
}

func (s *S3StorageClient) GetUrl(bucket *settings.Bucket, objKey string) (string, error) {
	url, err := url.JoinPath(bucket.CloudfrontDomain, objKey)
	if err != nil {
		return "", core_error.NewInternalError(err)
	}

	return url, nil
}

func (s *S3StorageClient) CreatePutPresignedUrl(ctx context.Context, file *ObjectMeta, key string, bucket string) string {
	req, _ := s.client.PutObjectRequest(&s3.PutObjectInput{
		Bucket:         aws.String(bucket),
		Key:            aws.String(key),
		ContentType:    aws.String(file.ContentType),
		ContentLength:  aws.Int64(file.ContentLength),
		ChecksumSHA256: aws.String(file.Checksum),
	})
	log := zerolog.Ctx(ctx)
	if req == nil {
		log.Error().Msgf("failed creating %s presigned put request", file.FileName)
		return ""
	}

	req.SetContext(ctx)
	url, err := req.Presign(presignedUrlExpireTime)
	if err != nil {
		log.Error().Err(err).Msgf("failed presigning put request")
		return ""
	}

	return url
}

func (s *S3StorageClient) CreateGetPresignedUrl(ctx context.Context, key string, bucket string) string {
	req, _ := s.client.GetObjectRequest(&s3.GetObjectInput{
		Bucket: aws.String(bucket),
		Key:    aws.String(key),
	})
	log := zerolog.Ctx(ctx)
	if req == nil {
		log.Error().Msgf("failed creating %s presigned get request", key)
		return ""
	}

	req.SetContext(ctx)
	url, err := req.Presign(presignedUrlExpireTime)
	if err != nil {
		log.Error().Err(err).Msgf("failed presigning get request")
		return ""
	}

	return url
}

func (s *S3StorageClient) CopyObject(sourceObj, targetBucket, targetObjKey string) error {
	_, err := s.client.CopyObject(&s3.CopyObjectInput{
		Bucket:     aws.String(targetBucket),
		CopySource: aws.String(sourceObj),
		Key:        aws.String(targetObjKey),
	})

	if err != nil {
		return core_error.StackError(fmt.Sprint("copy object failed: ", err))
	}

	return nil
}

func (s *S3StorageClient) DownloadObject(bucket, key string) ([]byte, error) {
	buf := &aws.WriteAtBuffer{}

	_, err := s.downloader.Download(buf, &s3.GetObjectInput{
		Bucket: aws.String(bucket),
		Key:    aws.String(key),
	})
	if err != nil {
		return nil, err
	}

	return buf.Bytes(), nil
}

func (s *S3StorageClient) DeleteObject(bucket, key string) error {
	_, err := s.client.DeleteObject(&s3.DeleteObjectInput{
		Bucket: aws.String(bucket),
		Key:    aws.String(key),
	})

	if err != nil {
		return core_error.StackError(fmt.Sprint("delete object failed: ", err))
	}

	return nil
}

func (s *S3StorageClient) ListObjectsV2Pages(ctx context.Context, bucket string, prefix string) ([]string, error) {
	targetItems := make([]string, 0)

	err := s.client.ListObjectsV2PagesWithContext(ctx,
		&s3.ListObjectsV2Input{
			Bucket: aws.String(bucket),
			Prefix: &prefix,
		}, func(page *s3.ListObjectsV2Output, lastPage bool) bool {
			for _, item := range page.Contents {
				targetItems = append(targetItems, *item.Key)
			}
			return true
		})
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}
	return targetItems, nil
}

func (s *S3StorageClient) UploadFile(ctx context.Context, bucketConfigKey, key string, file io.Reader) (string, error) {
	_, err := s.uploader.UploadWithContext(ctx, &s3manager.UploadInput{
		Body:   file,
		Bucket: aws.String(s.buckets[bucketConfigKey].Name),
		Key:    aws.String(key),
	})
	if err != nil {
		return "", core_error.NewInternalError(err)
	}

	url, err := url.JoinPath(s.buckets[bucketConfigKey].CloudfrontDomain, key)
	if err != nil {
		return "", core_error.NewInternalError(err)
	}
	return url, nil
}

func (s *S3StorageClient) GetObjectInfo(ctx context.Context, bucket, key string) (*s3.HeadObjectOutput, error) {
	result, err := s.client.HeadObjectWithContext(ctx, &s3.HeadObjectInput{
		Bucket: aws.String(bucket),
		Key:    aws.String(key),
	})

	if err != nil {
		return nil, core_error.StackError(fmt.Sprint("get object Info failed", err))
	}

	return result, nil
}

func ParseS3Url(rawUrl string) (bucketName, objKey string, err error) {
	urlObj, err := url.Parse(rawUrl)
	if err != nil {
		return "", "", core_error.NewInternalError(err)
	}

	return urlObj.Host, urlObj.Path, nil
}

func (s *S3StorageClient) RenameObject(bucket, key string, newKey string) error {
	sourceObj := strings.Join([]string{bucket, key}, "/")

	err := s.CopyObject(sourceObj, bucket, newKey)
	if err != nil {
		return err
	}

	err = s.DeleteObject(bucket, key)
	if err != nil {
		return err
	}

	return nil
}

func (s *S3StorageClient) CheckObjectExists(bucket, key string) (bool, error) {
	_, err := s.client.HeadObject(&s3.HeadObjectInput{
		Bucket: aws.String(bucket),
		Key:    aws.String(key),
	})

	if err != nil {
		// Verity if the object is not found
		if aerr, ok := err.(awserr.Error); ok && aerr.Code() != "NotFound" {
			return false, core_error.StackError(fmt.Sprint("check object exists failed: ", err))
		} else {
			return false, nil
		}
	}

	return true, nil
}
