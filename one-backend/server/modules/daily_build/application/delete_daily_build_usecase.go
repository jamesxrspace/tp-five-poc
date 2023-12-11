package application

import (
	"context"

	"xrspace.io/server/core/arch/application"
)

type IDeleteDailyBuildStorage interface {
	Delete(ctx context.Context, filepath string) (*DeleteDailyBuildResponse, error)
}

type DeleteDailyBuildQuery struct {
	Filepath string `form:"file_path" validate:"required"`
}

type DeleteDailyBuildResponse struct {
	Success bool
	Message string
}

func NewDeleteDailyBuildUseCase(storage IDeleteDailyBuildStorage) *DeleteDailyBuildUsecase {
	return &DeleteDailyBuildUsecase{
		storage: storage,
	}
}

type DeleteDailyBuildUsecase struct {
	storage IDeleteDailyBuildStorage
}

var _ application.IUseCase = (*DeleteDailyBuildUsecase)(nil)

func (s *DeleteDailyBuildUsecase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*DeleteDailyBuildQuery)

	dailyBuildsResp, err := s.storage.Delete(ctx, cmd.Filepath)

	if err != nil {
		return nil, err
	}

	return dailyBuildsResp, nil
}
