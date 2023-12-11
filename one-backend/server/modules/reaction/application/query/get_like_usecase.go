package query

import (
	"context"

	"xrspace.io/server/modules/reaction/application/define"
	"xrspace.io/server/modules/reaction/domain/enum"
)

type GetLikeQuery struct {
	XrID       string          `json:"xrid" token:"xrid" validate:"required"`
	TargetType enum.TargetType `uri:"target_type" validate:"required"`
	TargetID   string          `uri:"target_id" validate:"required"`
}

type GetLikeResponse struct {
	IsLike bool `json:"is_like"`
	Count  int  `json:"count"`
}

type GetLikeUseCase struct {
	dep define.Dependency
}

func NewGetLikeUseCase(dep define.Dependency) *GetLikeUseCase {
	return &GetLikeUseCase{dep: dep}
}

func (uc *GetLikeUseCase) Execute(ctx context.Context, query any) (any, error) {
	q := query.(*GetLikeQuery)

	result, err := uc.dep.QueryRepo.GetLikeReaction(ctx, define.GetLikeFilter{
		XrID:       q.XrID,
		TargetType: q.TargetType,
		TargetID:   q.TargetID,
	})
	if err != nil {
		return nil, err
	}

	if result == nil {
		return &GetLikeResponse{
			IsLike: false,
			Count:  0,
		}, nil
	}

	return &GetLikeResponse{
		IsLike: result.IsLike,
		Count:  result.Count,
	}, nil
}
