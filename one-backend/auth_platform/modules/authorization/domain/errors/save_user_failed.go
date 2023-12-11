package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrSaveUser)(nil)

type ErrSaveUser struct {
	*error_core.ErrorStruct
}

func NewErrSaveUser(err error, data interface{}) *ErrSaveUser {
	return &ErrSaveUser{
		ErrorStruct: error_core.NewErrorStruct(SaveUserFailed, errors.New("unable get jwks"), data),
	}
}
