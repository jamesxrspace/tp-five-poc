package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrGetAccountInfoFailed)(nil)

type ErrGetAccountInfoFailed struct {
	*error_core.ErrorStruct
}

func NewErrGetAccountInfoFailed(err error, data interface{}) *ErrGetAccountInfoFailed {
	return &ErrGetAccountInfoFailed{
		ErrorStruct: error_core.NewErrorStruct(GetAccountInfoFailed, err, data),
	}
}
