package application

import (
	"context"
	// this package must be used for stack trace
	"github.com/pkg/errors"

	"xrspace.io/server/core/arch/application"
)

type ErrorCommand struct {
}

type ErrorResponse struct {
	RoomId string
	XrId   string
	XrRoom string
}

type ErrorUseCase struct {
}

var _ application.IUseCase = (*ErrorUseCase)(nil)

func newErrorUseCase() *ErrorUseCase {
	return &ErrorUseCase{}
}

func (A *ErrorUseCase) Execute(_ context.Context, c any) (any, error) {
	return nil, errors.New("This is a error for stack test")
}
