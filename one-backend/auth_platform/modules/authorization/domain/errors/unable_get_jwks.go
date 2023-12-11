package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrUnableGetJwks)(nil)

type ErrUnableGetJwks struct {
	*error_core.ErrorStruct
}

func NewErrUnableGetJwks(err error, data interface{}) *ErrUnableGetJwks {
	return &ErrUnableGetJwks{
		ErrorStruct: error_core.NewErrorStruct(UnableGetJwks, errors.New("unable get jwks"), data),
	}
}
