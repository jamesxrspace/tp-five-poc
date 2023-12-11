package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrRefreshToken)(nil)

type ErrRefreshToken struct {
	*error_core.ErrorStruct
}

func NewErrRefreshToken(err error, data interface{}) *ErrRefreshToken {
	return &ErrRefreshToken{
		ErrorStruct: error_core.NewErrorStruct(GenRefreshTokenFailed, err, data),
	}
}
