package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrReadPrivateKey)(nil)

type ErrReadPrivateKey struct {
	*error_core.ErrorStruct
}

func NewErrReadPrivateKey(err error, data interface{}) *ErrReadPrivateKey {
	return &ErrReadPrivateKey{
		ErrorStruct: error_core.NewErrorStruct(ReadPrivateKeyFailed, errors.New("read private key failed"), data),
	}
}
