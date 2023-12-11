package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrCreateUser)(nil)

type ErrCreateUser struct {
	*error_core.ErrorStruct
}

func NewErrCreateUser(err error, data interface{}) *ErrCreateUser {
	return &ErrCreateUser{
		ErrorStruct: error_core.NewErrorStruct(CreateUserFailed, err, data),
	}
}
