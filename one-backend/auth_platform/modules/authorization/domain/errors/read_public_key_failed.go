package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrReadPublicKey)(nil)

type ErrReadPublicKey struct {
	*error_core.ErrorStruct
}

func NewErrReadPublicKey(err error, data interface{}) *ErrReadPublicKey {
	return &ErrReadPublicKey{
		ErrorStruct: error_core.NewErrorStruct(ReadPublicKeyFailed, errors.New("read public key failed"), data),
	}
}
