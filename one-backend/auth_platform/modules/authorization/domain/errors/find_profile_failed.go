package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrFindProfile)(nil)

type ErrFindProfile struct {
	*error_core.ErrorStruct
}

func NewErrFindProfile(err error, data interface{}) *ErrFindProfile {
	return &ErrFindProfile{
		ErrorStruct: error_core.NewErrorStruct(FindProfileFailed, err, data),
	}
}
