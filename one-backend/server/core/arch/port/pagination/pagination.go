package pagination

import "github.com/rs/zerolog/log"

type PaginationQuery struct {
	Offset int `json:"offset" form:"offset" binding:"min=0" validate:"min=0"`                             // this is page number which start with 0 in backend, front-end start with 1
	Size   int `json:"size" form:"size" binding:"required,min=1,max=50" validate:"required,min=1,max=50"` // size of a page
}

func (p *PaginationQuery) Skip() int {
	return p.Offset * p.Size
}

func (p *PaginationQuery) Limit() int {
	return p.Size
}

type PaginationResponse[T any] struct {
	Total int `json:"total"`
	Items []T `json:"items"`
}

func Paginate[T any](items []T, params PaginationQuery) PaginationResponse[T] {
	if params.Offset < 0 || params.Size <= 0 {
		log.Warn().Msgf("invalid pagination params: %+v", params)
		return PaginationResponse[T]{}
	}

	startIndex := int(params.Skip())
	if startIndex >= len(items) {
		return PaginationResponse[T]{}
	}

	endIndex := startIndex + params.Size
	if endIndex > len(items) {
		endIndex = len(items)
	}

	return PaginationResponse[T]{
		Total: len(items),
		Items: items[startIndex:endIndex],
	}
}

func NewPaginationResponse[T any](items []T, total int) *PaginationResponse[T] {
	return &PaginationResponse[T]{
		Total: total,
		Items: items,
	}
}
