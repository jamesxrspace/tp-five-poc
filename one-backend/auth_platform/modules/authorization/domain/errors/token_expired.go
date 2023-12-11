package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrTokenExpired)(nil)

type ErrTokenExpired struct {
	*error_core.ErrorStruct
}

func NewErrTokenExpired(err error, data interface{}) *ErrTokenExpired {
	return &ErrTokenExpired{
		ErrorStruct: error_core.NewErrorStruct(TokenExpired, errors.New("token expired"), data),
	}
}
