package application

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/daily_build/domain"
)

type IQueryDailyBuildStorage interface {
	List(ctx context.Context, paginationParams pagination.PaginationQuery, buildTypes []string) (*ListDailyBuildResponse, error)
}

type ListDailyBuildQuery struct {
	pagination.PaginationQuery
	BuildTypes []string `json:"build_types" form:"build_types"`
}

type ListDailyBuildResponse struct {
	pagination.PaginationResponse[*domain.DailyBuild]
}

func NewListDailyBuildUseCase(storage IQueryDailyBuildStorage) *ListDailyBuildUsecase {
	return &ListDailyBuildUsecase{
		storage: storage,
	}
}

type ListDailyBuildUsecase struct {
	storage IQueryDailyBuildStorage
}

var _ application.IUseCase = (*ListDailyBuildUsecase)(nil)

func (s *ListDailyBuildUsecase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*ListDailyBuildQuery)

	dailyBuildsResp, err := s.storage.List(ctx, cmd.PaginationQuery, cmd.BuildTypes)

	if err != nil {
		return nil, core_error.NewCoreError(domain.ListDailyBuildError, err)
	}

	return dailyBuildsResp, nil
}
