package s3

import (
	"context"
	"fmt"
	"regexp"
	"sort"
	"time"

	"github.com/aws/aws-sdk-go/service/s3"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/log"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/core/dependency/utility"
	"xrspace.io/server/modules/daily_build/application"
	"xrspace.io/server/modules/daily_build/domain"
)

var _ application.IQueryDailyBuildStorage = (*DailyBuildStorage)(nil)
var _ application.IDeleteDailyBuildStorage = (*DailyBuildStorage)(nil)

const (
	Folder = "artifact"
)

var TimeNow = time.Now

type IS3Cient interface {
	ListObjectsV2Pages(ctx context.Context, bucket string, prefix string) ([]string, error)
	RenameObject(bucket string, key string, newKey string) error
	CheckObjectExists(bucket string, key string) (bool, error)
	CreateGetPresignedUrl(ctx context.Context, key string, bucket string) string
	GetObjectInfo(ctx context.Context, bucket, key string) (*s3.HeadObjectOutput, error)
}

type DailyBuildStorage struct {
	client           IS3Cient
	apk_build_bucket string
}

func NewDailyBuildStorage(client IS3Cient, apkBuildBucket string) *DailyBuildStorage {
	return &DailyBuildStorage{
		client:           client,
		apk_build_bucket: apkBuildBucket,
	}
}

func containsBuildType(buildTypes []string, buildType string) bool {
	for _, t := range buildTypes {
		if t == buildType {
			return true
		}
	}
	return false
}

func (s *DailyBuildStorage) Delete(ctx context.Context, sourceKey string) (*application.DeleteDailyBuildResponse, error) {
	isFileExist, err := s.client.CheckObjectExists(s.apk_build_bucket, sourceKey)
	if err != nil {
		return nil, err
	}

	if !isFileExist {
		return &application.DeleteDailyBuildResponse{
			Success: true,
			Message: "File had been deleted before",
		}, nil
	}

	newKey := sourceKey + ".bad"
	err = s.client.RenameObject(s.apk_build_bucket, sourceKey, newKey)
	if err != nil {
		return nil, err
	}

	return &application.DeleteDailyBuildResponse{
		Success: true,
		Message: "File deleted successfully after renaming",
	}, nil
}

func (s *DailyBuildStorage) List(ctx context.Context, paginationParams pagination.PaginationQuery, buildTypes []string) (*application.ListDailyBuildResponse, error) {
	monthRange := 3
	now := TimeNow()
	fetchMonths := []time.Time{now}
	for i := 0; i < monthRange-1; i++ {
		Date := fetchMonths[len(fetchMonths)-1]
		fetchMonths = append(fetchMonths, utility.GetPreviousMonthLastDate(Date))
	}

	result := make([]string, 0)
	for _, monthPrefix := range fetchMonths {
		prefix := fmt.Sprintf("%s/%s", Folder, monthPrefix.Format("2006/01"))
		resp, err := s.client.ListObjectsV2Pages(ctx, s.apk_build_bucket, prefix)
		if err != nil {
			return nil, fmt.Errorf("list objects: %v", err)
		}
		result = append(result, resp...)
	}

	items := make(domain.DailyBuilds, 0)
	for _, obj := range result {
		file := s.matchFile(obj)
		if file == nil {
			log.Info().Msgf("failed to match file %s", obj)
			continue
		}

		if len(buildTypes) > 0 && !containsBuildType(buildTypes, string(file.BuildType)) {
			continue
		}
		url := s.client.CreateGetPresignedUrl(ctx, file.Key, s.apk_build_bucket)
		if url == "" {
			log := zerolog.Ctx(ctx)
			log.Warn().Str("key", file.Key).Msg("failed to create a presigned URL for file")
			continue
		}

		fileInfo, err := s.client.GetObjectInfo(ctx, s.apk_build_bucket, file.Key)
		if err != nil {
			log := zerolog.Ctx(ctx)
			log.Warn().Str("key", file.Key).Msg("failed to get object info")
			continue
		}

		items = append(items, &domain.DailyBuild{
			Key:       file.hash,
			BuildType: file.BuildType,
			Date:      *fileInfo.LastModified,
			Url:       url,
			Filepath:  file.filepath,
		})
	}

	sort.Sort(items) // sort by date desc

	resp := pagination.Paginate[*domain.DailyBuild](items, paginationParams)

	return &application.ListDailyBuildResponse{
		PaginationResponse: resp,
	}, nil
}

type fileInfo struct {
	Key       string
	BuildType domain.BuildType
	Date      time.Time
	hash      string
	filepath  string
}

func (s *DailyBuildStorage) matchFile(path string) *fileInfo {
	apkPattern := `^artifact/(?P<year>\d{4})/(?P<month>\d{2})/(?P<day>\d{2})/(?P<hash>[a-f0-9]{8,})/apk/dev/app-dev-release-(?P<year2>\d{4})_(?P<month2>\d{2})_(?P<day2>\d{2})-(?P<short_hash>[a-f0-9]{7})\.apk$`
	vrPattern := `^artifact/(?P<year>\d{4})/(?P<month>\d{2})/(?P<day>\d{2})/(?P<hash>[a-f0-9]{8,})/vr-apk/dev/app-dev-release-(?P<year2>\d{4})_(?P<month2>\d{2})_(?P<day2>\d{2})-(?P<short_hash>[a-f0-9]{7})\.apk$`

	newFileInfo := func(match []string, buildType domain.BuildType) *fileInfo {
		year := match[1]
		month := match[2]
		day := match[3]
		hash := match[4]
		filepath := match[0]

		parsedDate, err := time.Parse(time.DateOnly, year+"-"+month+"-"+day)
		if err != nil {
			log.Info().Err(err).Msgf("failed to parse date %s for file path %s", parsedDate, path)
			return nil
		}

		return &fileInfo{
			Key:       path,
			BuildType: buildType,
			Date:      parsedDate,
			hash:      hash,
			filepath:  filepath,
		}

	}

	match := regexp.MustCompile(vrPattern).FindStringSubmatch(path)
	if len(match) > 0 {
		return newFileInfo(match, domain.VRAPK)
	}

	match = regexp.MustCompile(apkPattern).FindStringSubmatch(path)
	if len(match) > 0 {
		return newFileInfo(match, domain.APK)
	}
	return nil
}
