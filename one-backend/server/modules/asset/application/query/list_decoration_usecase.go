package query

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/port/pagination"
)

type DecorationItem struct {
	Id            string   `json:"id"`
	TitleI18n     string   `json:"title_i18n"`
	ThumbnailKey  string   `json:"thumbnail_key"`
	Tags          []string `json:"tags"`
	BundleID      string   `json:"bundle_id"`
	DecorationKey string   `json:"decoration_key"`
	CategoryID    []string `json:"category_id"`
}

type ListDecorationResponse struct {
	pagination.PaginationResponse[DecorationItem]
}
type ListDecorationQuery struct {
	pagination.PaginationQuery
	Category []string `form:"cate" validate:"required,min=1,max=50,unique"`
}

type ListDecorationUseCase struct {
	repository IQueryRepository
}

var (
	_ application.IUseCase = (*ListDecorationUseCase)(nil)
)

func NewListDecorationUseCase(
	repository IQueryRepository,
) *ListDecorationUseCase {
	return &ListDecorationUseCase{
		repository: repository,
	}
}

func (u *ListDecorationUseCase) Execute(ctx context.Context, query any) (any, error) {
	q := query.(*ListDecorationQuery)
	decorations, total, err := u.repository.QueryDecorations(ctx, q.Category, q.Offset, q.Size)

	return pagination.PaginationResponse[*DecorationItem]{
		Items: decorations,
		Total: (int)(total),
	}, err
}
