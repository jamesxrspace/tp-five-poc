package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
)

var _ application.IUseCase = (*ListReelUseCase)(nil)

type ListReelQuery struct {
	XrID   string `form:"xrid"`
	ReelID string `form:"reel_id"`
	Status string `form:"status"`
	pagination.PaginationQuery
}

type ListReelResponse struct {
	Items []*entity.Reel `json:"items"`
}

type ListReelUseCase struct {
	dep define.Dependency
}

func NewListReelUseCase(dep define.Dependency) *ListReelUseCase {
	return &ListReelUseCase{
		dep: dep,
	}
}

func (u *ListReelUseCase) Execute(ctx context.Context, query any) (any, error) {
	q := query.(*ListReelQuery)

	result, err := u.dep.QueryRepo.ListReel(ctx, &define.ListReelFilter{
		XrID:            q.XrID,
		ReelID:          q.ReelID,
		Status:          q.Status,
		PaginationQuery: q.PaginationQuery,
	})

	if err != nil {
		return nil, err
	}
	return &ListReelResponse{
		Items: result.Items,
	}, nil
}
