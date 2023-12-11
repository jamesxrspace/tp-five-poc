package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrGenToken)(nil)

type ErrGenToken struct {
	*error_core.ErrorStruct
}

func NewErrGenToken(err error, data interface{}) *ErrGenToken {
	return &ErrGenToken{
		ErrorStruct: error_core.NewErrorStruct(GenTokenFailed, err, data),
	}
}
