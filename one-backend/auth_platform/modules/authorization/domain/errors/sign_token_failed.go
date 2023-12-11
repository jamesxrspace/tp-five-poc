package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrSignToken)(nil)

type ErrSignToken struct {
	*error_core.ErrorStruct
}

func NewErrSignToken(err error, data interface{}) *ErrSignToken {
	return &ErrSignToken{
		ErrorStruct: error_core.NewErrorStruct(SignTokenFailed, errors.New("sign token failed"), data),
	}
}
