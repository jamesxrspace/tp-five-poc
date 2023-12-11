package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/port/pagination"
)

type CategoryItem struct {
	ID        string `json:"id"`
	TitleI18n string `json:"title_i18n"`
}

type ListDecorationCategoryResponse struct {
	pagination.PaginationResponse[DecorationItem]
}
type ListDecorationCategoryQuery struct {
	pagination.PaginationQuery
}

type ListDecorationCategoryUseCase struct {
	repository IQueryRepository
}

var (
	_ application.IUseCase = (*ListDecorationCategoryUseCase)(nil)
)

func NewListDecorationCategoryUseCase(
	repository IQueryRepository,
) *ListDecorationCategoryUseCase {
	return &ListDecorationCategoryUseCase{
		repository: repository,
	}
}

func (u *ListDecorationCategoryUseCase) Execute(ctx context.Context, query any) (any, error) {
	q := query.(*ListDecorationCategoryQuery)
	categories, total, err := u.repository.QueryCategory(ctx, q.Offset, q.Size)

	return pagination.PaginationResponse[*CategoryItem]{
		Items: categories,
		Total: (int)(total),
	}, err
}
