package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrInvalidSignMethod)(nil)

type ErrInvalidSignMethod struct {
	*error_core.ErrorStruct
}

func NewErrInvalidSignMethod(err error, data interface{}) *ErrInvalidSignMethod {
	return &ErrInvalidSignMethod{
		ErrorStruct: error_core.NewErrorStruct(InvalidSignMethod, errors.New("invalid signing method"), data),
	}
}
