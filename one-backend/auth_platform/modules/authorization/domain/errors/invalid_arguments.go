package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrInvalidArguments)(nil)

type ErrInvalidArguments struct {
	*error_core.ErrorStruct
}

func NewErrInvalidArguments(err error, data interface{}) *ErrInvalidArguments {
	return &ErrInvalidArguments{
		ErrorStruct: error_core.NewErrorStruct(InvalidArguments, err, data),
	}
}
