package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrInvalidEmailOrPassword)(nil)

type ErrInvalidEmailOrPassword struct {
	*error_core.ErrorStruct
}

func NewErrInvalidEmailOrPassword(err error, data interface{}) *ErrInvalidEmailOrPassword {
	return &ErrInvalidEmailOrPassword{
		ErrorStruct: error_core.NewErrorStruct(InvalidEmailOrPassword, err, data),
	}
}
