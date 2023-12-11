package s3_test

import (
	"context"
	"testing"
	"time"

	"github.com/aws/aws-sdk-go/aws"
	aws_s3 "github.com/aws/aws-sdk-go/service/s3"
	"github.com/stretchr/testify/mock"
	"github.com/stretchr/testify/suite"

	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/daily_build/domain"
	"xrspace.io/server/modules/daily_build/port/output/storage/s3"
)

func TestDailyBuildStorageSuite(t *testing.T) {
	suite.Run(t, new(DailyBuildStorage))
}

type DailyBuildStorage struct {
	suite.Suite
	mockClient   *mockS3Client
	prefix       string
	preSignedUrl string
	apkPath      []string
	vrPath       []string
	expectItems  []*domain.DailyBuild
}

type mockS3Client struct {
	mock.Mock
}

func (m *mockS3Client) ListObjectsV2Pages(ctx context.Context, bucket string, prefix string) ([]string, error) {
	args := m.Called(ctx, bucket, prefix)
	resp := args.Get(0).([]string)
	return resp, args.Error(1)
}

func (m *mockS3Client) CreateGetPresignedUrl(ctx context.Context, key string, bucket string) string {
	args := m.Called(ctx, key)
	resp := args.Get(0).(string)
	return resp
}

func (m *mockS3Client) GetObjectInfo(ctx context.Context, bucket string, key string) (*aws_s3.HeadObjectOutput, error) {
	args := m.Called(ctx, bucket, key)
	resp := args.Get(0).(*aws_s3.HeadObjectOutput)
	return resp, args.Error(1)
}

func (m *mockS3Client) RenameObject(bucket, key string, newKey string) error {
	args := m.Called(bucket, key, newKey)
	return args.Error(0)
}

func (m *mockS3Client) CheckObjectExists(bucket string, key string) (bool, error) {
	args := m.Called(bucket, key)
	resp := args.Get(0).(bool)
	return resp, args.Error(1)
}

func newDailyBuild(key string, buildType domain.BuildType, date time.Time, preSignedUrl, filepath string) *domain.DailyBuild {
	return &domain.DailyBuild{
		Key:       key,
		BuildType: buildType,
		Date:      date,
		Url:       preSignedUrl,
		Filepath:  filepath,
	}
}

func (s *DailyBuildStorage) SetupTest() {
	s3.TimeNow = func() time.Time { return time.Date(2023, 10, 31, 0, 0, 0, 0, time.UTC) }
	s.prefix = s3.Folder + "/2023/10"
	s.preSignedUrl = "success url"
	s.mockClient = new(mockS3Client)
	s.apkPath = []string{
		s.prefix + "/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/apk/dev/app-dev-release-2023_10_01-8d0dab8.apk",
		s.prefix + "/05/8d0dab80c9d394ed3f2f35437549a709db6620ec/apk/dev/app-dev-release-2023_10_05-8d0dab8.apk",
		s.prefix + "/10/8d0dab80c9d394ed3f2f35437549a709db6620ec/apk/dev/app-dev-release-2023_10_10-8d0dab8.apk",
	}
	s.vrPath = []string{
		s.prefix + "/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_10_01-8d0dab8.apk",
		s.prefix + "/05/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_10_05-8d0dab8.apk",
		s.prefix + "/10/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_10_10-8d0dab8.apk",
	}
	s.expectItems = []*domain.DailyBuild{
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.APK, time.Date(2023, 10, 1, 0, 0, 0, 0, time.UTC), s.preSignedUrl, s.apkPath[0]),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.APK, time.Date(2023, 10, 5, 0, 0, 0, 0, time.UTC), s.preSignedUrl, s.apkPath[1]),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.APK, time.Date(2023, 10, 10, 0, 0, 0, 0, time.UTC), s.preSignedUrl, s.apkPath[2]),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.VRAPK, time.Date(2023, 10, 1, 0, 0, 0, 0, time.UTC), s.preSignedUrl, s.vrPath[0]),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.VRAPK, time.Date(2023, 10, 5, 0, 0, 0, 0, time.UTC), s.preSignedUrl, s.vrPath[1]),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.VRAPK, time.Date(2023, 10, 10, 0, 0, 0, 0, time.UTC), s.preSignedUrl, s.vrPath[2]),
	}
	// set default mockClient action
	s.mockClient.Mock.On("CreateGetPresignedUrl", mock.Anything, mock.Anything, mock.Anything).Return(s.preSignedUrl)

	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, s.apkPath[0]).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 1, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, s.apkPath[1]).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 5, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, s.apkPath[2]).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 10, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, s.vrPath[0]).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 1, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, s.vrPath[1]).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 5, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, s.vrPath[2]).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 10, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
}

func (s *DailyBuildStorage) TestList() {
	// arrange
	other := s.prefix + "/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/otherfile"
	pageResp := []string{s.apkPath[0], s.vrPath[0], other}
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s.prefix).Return(
		pageResp, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, mock.Anything).Return(
		[]string{}, nil)

	params := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}

	buildTypes := []string{string(domain.APK), string(domain.VRAPK)}

	expectItems := []*domain.DailyBuild{
		s.expectItems[0],
		s.expectItems[3],
	}

	// act
	storage := s3.NewDailyBuildStorage(s.mockClient, "apk_build_bucket")
	result, _ := storage.List(context.TODO(), params, buildTypes)

	// assert
	s.Equal(2, result.Total)
	s.Equal(expectItems, result.Items)

}

func (s *DailyBuildStorage) TestListPreviousThreeMonth() {
	// arrange
	vrPath10 := s3.Folder + "/2023/10/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_10_01-8d0dab8.apk"
	vrPath09 := s3.Folder + "/2023/09/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_09_01-8d0dab8.apk"
	vrPath08 := s3.Folder + "/2023/08/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_08_01-8d0dab8.apk"
	vrPath07 := s3.Folder + "/2023/07/01/8d0dab80c9d394ed3f2f35437549a709db6620ec/vr-apk/dev/app-dev-release-2023_07_01-8d0dab8.apk"
	pageResp10 := []string{vrPath10}
	pageResp09 := []string{vrPath09}
	pageResp08 := []string{vrPath08}
	pageResp07 := []string{vrPath07}
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s3.Folder+"/2023/10").Return(
		pageResp10, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s3.Folder+"/2023/09").Return(
		pageResp09, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s3.Folder+"/2023/08").Return(
		pageResp08, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s3.Folder+"/2023/07").Return(
		pageResp07, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, mock.Anything).Return(
		[]string{}, nil)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, vrPath10).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 10, 1, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, vrPath09).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 9, 1, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)
	s.mockClient.Mock.On("GetObjectInfo", mock.Anything, mock.Anything, vrPath08).Return(
		&aws_s3.HeadObjectOutput{
			LastModified: aws.Time(time.Date(2023, 8, 1, 0, 0, 0, 0, time.UTC)),
		},
		nil,
	)

	buildTypes := []string{string(domain.APK), string(domain.VRAPK)}

	params := pagination.PaginationQuery{
		Offset: 0,
		Size:   3,
	}

	expectItems := []*domain.DailyBuild{
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.VRAPK, time.Date(2023, 10, 1, 0, 0, 0, 0, time.UTC), s.preSignedUrl, vrPath10),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.VRAPK, time.Date(2023, 9, 1, 0, 0, 0, 0, time.UTC), s.preSignedUrl, vrPath09),
		newDailyBuild("8d0dab80c9d394ed3f2f35437549a709db6620ec", domain.VRAPK, time.Date(2023, 8, 1, 0, 0, 0, 0, time.UTC), s.preSignedUrl, vrPath08),
	}

	// act
	storage := s3.NewDailyBuildStorage(s.mockClient, "apk_build_bucket")
	result, _ := storage.List(context.TODO(), params, buildTypes)

	// assert
	s.Equal(3, result.Total)
	s.Equal(expectItems, result.Items)

}

func (s *DailyBuildStorage) TestOderByDate() {
	// arrange
	pageResp := []string{s.apkPath[0], s.apkPath[1], s.apkPath[2]}
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s.prefix).Return(
		pageResp, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, mock.Anything).Return(
		[]string{}, nil)

	params := pagination.PaginationQuery{
		Offset: 0,
		Size:   3,
	}

	buildTypes := []string{string(domain.APK), string(domain.VRAPK)}

	expectItems := []*domain.DailyBuild{
		s.expectItems[2],
		s.expectItems[1],
		s.expectItems[0],
	}

	// act
	storage := s3.NewDailyBuildStorage(s.mockClient, "apk_build_bucket")
	result, _ := storage.List(context.TODO(), params, buildTypes)

	// assert
	s.Equal(expectItems, result.Items)

}

func (s *DailyBuildStorage) TestFilterByBuildType() {
	// arrange
	pageResp := []string{s.vrPath[0], s.vrPath[1], s.apkPath[0], s.apkPath[1]}
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s.prefix).Return(
		pageResp, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, mock.Anything).Return(
		[]string{}, nil)

	params := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}

	buildTypes := []string{string(domain.VRAPK)}

	expectItems := []*domain.DailyBuild{
		s.expectItems[4],
		s.expectItems[3],
	}

	// act
	storage := s3.NewDailyBuildStorage(s.mockClient, "apk_build_bucket")
	result, _ := storage.List(context.TODO(), params, buildTypes)

	// assert
	s.Equal(expectItems, result.Items)

}

func (s *DailyBuildStorage) TestPassEmptyBuildType() {
	// arrange
	pageResp := []string{s.vrPath[0], s.vrPath[1], s.apkPath[0], s.apkPath[1]}
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s.prefix).Return(
		pageResp, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, mock.Anything).Return(
		[]string{}, nil)

	params := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}

	buildTypes := []string{}

	expectItems := []*domain.DailyBuild{
		s.expectItems[4],
		s.expectItems[1],
	}

	// act
	storage := s3.NewDailyBuildStorage(s.mockClient, "apk_build_bucket")
	result, _ := storage.List(context.TODO(), params, buildTypes)

	// assert
	s.Equal(expectItems, result.Items)

}

func (s *DailyBuildStorage) TestDelete() {
	// arrange
	pageResp := []string{s.apkPath[0], s.apkPath[1], s.apkPath[2]}
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, s.prefix).Return(
		pageResp, nil)
	s.mockClient.Mock.On("ListObjectsV2Pages", mock.Anything, mock.Anything, mock.Anything).Return(
		[]string{}, nil)

	params := pagination.PaginationQuery{
		Offset: 0,
		Size:   2,
	}

	buildTypes := []string{string(domain.APK), string(domain.VRAPK)}

	expectItems := []*domain.DailyBuild{
		s.expectItems[2],
		s.expectItems[1],
	}

	sourceKey := s.apkPath[0]
	newKey := sourceKey + ".bad"

	s.mockClient.Mock.On("RenameObject", "apk_build_bucket", sourceKey, newKey).Return(nil)
	s.mockClient.Mock.On("CheckObjectExists", "apk_build_bucket", sourceKey).Return(true, nil)

	// act
	storage := s3.NewDailyBuildStorage(s.mockClient, "apk_build_bucket")
	delete_result, err := storage.Delete(context.TODO(), sourceKey)

	// assert
	s.Nil(err)
	s.NotNil(delete_result)
	s.True(delete_result.Success)
	s.Equal("File deleted successfully after renaming", delete_result.Message)

	// act
	list_result, _ := storage.List(context.TODO(), params, buildTypes)

	// assert
	s.Equal(expectItems, list_result.Items)

}
