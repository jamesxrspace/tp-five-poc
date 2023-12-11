package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrJsonMarshal)(nil)

type ErrJsonMarshal struct {
	*error_core.ErrorStruct
}

func NewErrJsonMarshal(err error, data interface{}) *ErrJsonMarshal {
	return &ErrJsonMarshal{
		ErrorStruct: error_core.NewErrorStruct(JsonMarshalFailed, errors.New("json marshal failed"), data),
	}
}
