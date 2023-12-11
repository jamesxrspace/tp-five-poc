package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrParseToken)(nil)

type ErrParseToken struct {
	*error_core.ErrorStruct
}

func NewErrParseToken(err error, data interface{}) *ErrParseToken {
	return &ErrParseToken{
		ErrorStruct: error_core.NewErrorStruct(ParseTokenFailed, errors.New("parse token failed"), data),
	}
}
