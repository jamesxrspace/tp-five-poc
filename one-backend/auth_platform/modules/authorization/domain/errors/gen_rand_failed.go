package auth_errors

import (
	"auth_platform/core/error_core"
	"errors"
)

var _ error_core.IError = (*ErrGenRand)(nil)

type ErrGenRand struct {
	*error_core.ErrorStruct
}

func NewErrGenRand(err error, data interface{}) *ErrGenRand {
	return &ErrGenRand{
		ErrorStruct: error_core.NewErrorStruct(GenRandFailed, errors.New("gen random bytes failed"), data),
	}
}
