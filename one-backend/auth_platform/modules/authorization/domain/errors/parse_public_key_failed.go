package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrParsePublicKey)(nil)

type ErrParsePublicKey struct {
	*error_core.ErrorStruct
}

func NewErrParsePublicKey(err error, data interface{}) *ErrParsePublicKey {
	return &ErrParsePublicKey{
		ErrorStruct: error_core.NewErrorStruct(ParsePublicKeyFailed, errors.New("read public key failed"), data),
	}
}
