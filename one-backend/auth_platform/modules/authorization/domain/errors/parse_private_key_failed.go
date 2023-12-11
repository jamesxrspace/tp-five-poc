package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrParsePrivateKey)(nil)

type ErrParsePrivateKey struct {
	*error_core.ErrorStruct
}

func NewErrParsePrivateKey(err error, data interface{}) *ErrParsePrivateKey {
	return &ErrParsePrivateKey{
		ErrorStruct: error_core.NewErrorStruct(ParsePrivateKeyFailed, errors.New("read private key failed"), data),
	}
}
