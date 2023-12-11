package auth_errors

import (
	"auth_platform/core/error_core"
)

var _ error_core.IError = (*ErrEmailDuplicated)(nil)

type ErrEmailDuplicated struct {
	*error_core.ErrorStruct
}

func NewErrEmailDuplicated(err error, data interface{}) *ErrEmailDuplicated {
	return &ErrEmailDuplicated{
		ErrorStruct: error_core.NewErrorStruct(EmailDuplicated, err, data),
	}
}
