package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrDecodeClaims)(nil)

type ErrDecodeClaims struct {
	*error_core.ErrorStruct
}

func NewErrDecodeClaims(err error, data interface{}) *ErrDecodeClaims {
	return &ErrDecodeClaims{
		ErrorStruct: error_core.NewErrorStruct(DecodeClaimsFailed, errors.New("decode claims failed"), data),
	}
}
