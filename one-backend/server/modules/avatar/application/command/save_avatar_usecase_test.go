package command

import (
	"context"
	"io"
	"mime/multipart"
	"os"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/enum"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/service"
	"xrspace.io/server/modules/avatar/domain/storage"
	"xrspace.io/server/modules/avatar/domain/value_object"
	"xrspace.io/server/modules/avatar/port/output/repository/inmem"
	inmemStorage "xrspace.io/server/modules/avatar/port/output/storage/inmem"
)

func TestSaveAvatarUseCase_Execute(t *testing.T) {
	type args struct {
		cmd            *SaveAvatarCommand
		defaultAvatars map[value_object.AvatarID]*entity.Avatar
	}
	type want struct {
		saveAvatarResponse *SaveAvatarResponse
		saveAssetTimes     int
		getAvatarTimes     int
		saveAvatarTimes    int
	}
	tests := []struct {
		name    string
		u       *SaveAvatarUseCase
		args    args
		want    want
		wantErr bool
		errMsg  string
	}{
		{
			name: "success_create_avatar",
			args: args{
				cmd: &SaveAvatarCommand{
					AppID:           "test_app_id",
					XrID:            "test_xrid",
					Type:            enum.AvatarTypeXrV2,
					AvatarAsset:     createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
					AvatarHead:      createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarUpperBody: createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarFullBody:  createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
				},
			},
			want: want{
				saveAvatarResponse: &SaveAvatarResponse{
					Avatar: &entity.Avatar{
						AppID: "test_app_id",
						XrID:  "test_xrid",
						Type:  enum.AvatarTypeXrV2,
					},
				},
				saveAssetTimes:  4,
				getAvatarTimes:  0,
				saveAvatarTimes: 1,
			},
		},
		{
			name: "success_update_avatar",
			args: args{
				cmd: &SaveAvatarCommand{
					AvatarID:        "test_avatar_id",
					AppID:           "test_app_id",
					XrID:            "test_xrid",
					Type:            enum.AvatarTypeXrV2,
					AvatarAsset:     createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
					AvatarHead:      createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarUpperBody: createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarFullBody:  createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
				},
				defaultAvatars: map[value_object.AvatarID]*entity.Avatar{
					"test_avatar_id": entity.NewAvatar(&entity.AvatarParams{
						AvatarID:  "test_avatar_id",
						AppID:     "test_app_id",
						XrID:      "test_xrid",
						Type:      enum.AvatarTypeXrV2,
						AvatarUrl: "test_avatar_url",
						Thumbnail: &entity.AvatarThumbnail{
							Head:      "test_head_thumbnail",
							UpperBody: "test_upper_body_thumbnail",
							FullBody:  "test_full_body_thumbnail",
						},
					}),
				},
			},
			want: want{
				saveAvatarResponse: &SaveAvatarResponse{
					Avatar: &entity.Avatar{
						AppID: "test_app_id",
						XrID:  "test_xrid",
						Type:  enum.AvatarTypeXrV2,
					},
				},
				saveAssetTimes:  4,
				getAvatarTimes:  1,
				saveAvatarTimes: 1,
			},
		},
		{
			name: "failed_if_avatar_not_found",
			args: args{
				cmd: &SaveAvatarCommand{
					AvatarID:        "test_avatar_id",
					AppID:           "test_app_id",
					XrID:            "test_xrid",
					Type:            enum.AvatarTypeXrV2,
					AvatarAsset:     createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
					AvatarHead:      createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarUpperBody: createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarFullBody:  createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
				},
			},
			wantErr: true,
			errMsg:  "avatar not found",
		},
		{
			name: "failed_if_all_thumbnails_are_not_provided",
			args: args{
				cmd: &SaveAvatarCommand{
					AppID: "test_app_id",
					XrID:  "test_xrid",
					Type:  enum.AvatarTypeXrV2,
				},
			},
			wantErr: true,
			errMsg:  "Error:Field validation",
		},
		{
			name: "failed_if_one_of_thumbnails_is_not_provided",
			args: args{
				cmd: &SaveAvatarCommand{
					AppID:           "test_app_id",
					XrID:            "test_xrid",
					Type:            enum.AvatarTypeXrV2,
					AvatarHead:      createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarUpperBody: createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
				},
			},
			wantErr: true,
			errMsg:  "Error:Field validation",
		},
		{
			name: "failed_if_thumbnail_file_type_is_not_allowed",
			args: args{
				cmd: &SaveAvatarCommand{
					AppID:           "test_app_id",
					XrID:            "test_xrid",
					Type:            enum.AvatarTypeXrV2,
					AvatarAsset:     createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
					AvatarHead:      createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
					AvatarUpperBody: createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarFullBody:  createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
				},
			},
			wantErr: true,
			errMsg:  "file type is not allowed",
		},
		{
			name: "failed_if_avatar_type_is_not_allowed",
			args: args{
				cmd: &SaveAvatarCommand{
					AppID:           "test_app_id",
					XrID:            "test_xrid",
					Type:            "test_avatar_type",
					AvatarAsset:     createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
					AvatarHead:      createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarUpperBody: createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
					AvatarFullBody:  createFakeAvatarAsset("test_avatar_asset.png", "12345", "image/png"),
				},
			},
			wantErr: true,
			errMsg:  "avatar type is not allowed",
		},
		{
			name: "success_update_avatar_without_avatar_thumbnails",
			args: args{
				cmd: &SaveAvatarCommand{
					AvatarID:    "test_avatar_id",
					AppID:       "test_app_id",
					XrID:        "test_xrid",
					Type:        enum.AvatarTypeXrV2,
					AvatarAsset: createFakeAvatarAsset("test_avatar_asset.zip", "12345", "application/zip"),
				},
				defaultAvatars: map[value_object.AvatarID]*entity.Avatar{
					"test_avatar_id": entity.NewAvatar(&entity.AvatarParams{
						AvatarID:  "test_avatar_id",
						AppID:     "test_app_id",
						XrID:      "test_xrid",
						Type:      enum.AvatarTypeXrV2,
						AvatarUrl: "test_avatar_url",
						Thumbnail: &entity.AvatarThumbnail{
							Head:      "test_head_thumbnail",
							UpperBody: "test_upper_body_thumbnail",
							FullBody:  "test_full_body_thumbnail",
						},
					}),
				},
			},
			want: want{
				saveAvatarResponse: &SaveAvatarResponse{
					Avatar: &entity.Avatar{
						AvatarID: "test_avatar_id",
						AppID:    "test_app_id",
						XrID:     "test_xrid",
						Type:     enum.AvatarTypeXrV2,
					},
				},
				saveAssetTimes:  1,
				getAvatarTimes:  1,
				saveAvatarTimes: 1,
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := newAvatarRepositoryForTest(tt.args.defaultAvatars)
			storageClient, _ := newStorageForTest()
			service := service.NewAvatarService()
			u := NewSaveAvatarUseCase(repo, storageClient, service)

			// act
			got, err := u.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("SaveAvatarUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			// assert
			assert.Equal(t, tt.want.saveAvatarResponse.Avatar.AppID, got.Avatar.AppID)
			assert.Equal(t, tt.want.saveAvatarResponse.Avatar.XrID, got.Avatar.XrID)
			assert.Equal(t, tt.want.saveAvatarResponse.Avatar.Type, got.Avatar.Type)

			assert.Equal(t, tt.want.saveAssetTimes, storageClient.GetSaveAssetTimes())
			assert.Equal(t, tt.want.getAvatarTimes, repo.GetAvatarTimes())
			assert.Equal(t, tt.want.saveAvatarTimes, repo.SaveAvatarTimes())

			if tt.args.cmd.AvatarAsset != nil {
				os.Remove(tt.args.cmd.AvatarAsset.Filename)
			}
			if tt.args.cmd.AvatarHead != nil {
				os.Remove(tt.args.cmd.AvatarHead.Filename)
			}
			if tt.args.cmd.AvatarUpperBody != nil {
				os.Remove(tt.args.cmd.AvatarUpperBody.Filename)
			}
			if tt.args.cmd.AvatarFullBody != nil {
				os.Remove(tt.args.cmd.AvatarFullBody.Filename)
			}
		})
	}
}

func createFakeAvatarAsset(filename, content, fileType string) *value_object.AvatarAsset {
	_ = os.WriteFile(filename, []byte(content), 0644)
	file, _ := os.Open(filename)
	fileHeader := &multipart.FileHeader{
		Filename: filename,
	}
	fileHeader.Header = make(map[string][]string)
	fileHeader.Header.Add("Content-Type", fileType)
	return &value_object.AvatarAsset{
		File:       file,
		FileHeader: fileHeader,
	}
}

// IAssetStorageClientForTest is a test interface for asset storage client
type IAssetStorageClientForTest interface {
	storage.IAssetStorage
	GetSaveAssetTimes() int
}

var _ IAssetStorageClientForTest = (*storageForTest)(nil)

func newStorageForTest() (IAssetStorageClientForTest, error) {
	storage, err := inmemStorage.NewAssetStorageClient()
	if err != nil {
		return nil, err
	}
	return &storageForTest{
		AssetStorageClient: storage,
		saveAssetTimes:     0,
	}, nil
}

type storageForTest struct {
	*inmemStorage.AssetStorageClient
	saveAssetTimes int
}

func (s *storageForTest) SaveAsset(ctx context.Context, toPath string, file io.Reader) (value_object.AssetUrl, error) {
	s.saveAssetTimes++
	return "https://test.file.url", nil
}

func (s *storageForTest) GetSaveAssetTimes() int {
	return s.saveAssetTimes
}

func (s *storageForTest) GetEndpoint() string {
	return ""
}

// IAvatarRepositoryForTest is a test interface for avatar repository
type IAvatarRepositoryForTest interface {
	repository.IAvatarRepository
	GetAvatarTimes() int
	SaveAvatarTimes() int
	GetAvatarForTest(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error)
}

var _ IAvatarRepositoryForTest = (*avatarRepositoryForTest)(nil)

func newAvatarRepositoryForTest(avatars map[value_object.AvatarID]*entity.Avatar) IAvatarRepositoryForTest {
	return &avatarRepositoryForTest{
		AvatarRepository: inmem.NewAvatarRepository(avatars, nil),
		getAvatarTimes:   0,
		saveAvatarTimes:  0,
	}
}

type avatarRepositoryForTest struct {
	*inmem.AvatarRepository
	getAvatarTimes  int
	saveAvatarTimes int
}

func (r *avatarRepositoryForTest) GetAvatar(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error) {
	r.getAvatarTimes++
	return r.AvatarRepository.GetAvatar(ctx, avatarID)
}

func (r *avatarRepositoryForTest) SaveAvatar(ctx context.Context, avatar *entity.Avatar) error {
	r.saveAvatarTimes++
	return r.AvatarRepository.SaveAvatar(ctx, avatar)
}

func (r *avatarRepositoryForTest) GetAvatarTimes() int {
	return r.getAvatarTimes
}

func (r *avatarRepositoryForTest) SaveAvatarTimes() int {
	return r.saveAvatarTimes
}

func (r *avatarRepositoryForTest) GetAvatarForTest(ctx context.Context, avatarID value_object.AvatarID) (*entity.Avatar, error) {
	return r.AvatarRepository.GetAvatar(ctx, avatarID)
}
