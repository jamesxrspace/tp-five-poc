package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrFindProfile)(nil)

type ErrUpdateProfile struct {
	*error_core.ErrorStruct
}

func NewErrUpdateProfile(err error, data interface{}) *ErrUpdateProfile {
	return &ErrUpdateProfile{
		ErrorStruct: error_core.NewErrorStruct(UpdateProfileFailed, err, data),
	}
}
