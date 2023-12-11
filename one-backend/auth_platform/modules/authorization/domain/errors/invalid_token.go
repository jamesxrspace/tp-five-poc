package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrInvalidToken)(nil)

type ErrInvalidToken struct {
	*error_core.ErrorStruct
}

func NewErrInvalidToken(err error, data interface{}) *ErrInvalidToken {
	return &ErrInvalidToken{
		ErrorStruct: error_core.NewErrorStruct(InvalidToken, errors.New("invalid token"), data),
	}
}
