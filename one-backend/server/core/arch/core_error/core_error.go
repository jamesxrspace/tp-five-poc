package core_error

import (
	"fmt"

	"github.com/pkg/errors"
	"github.com/rs/zerolog/log"
)

// Deprecated: use CodeError
type CoreError struct {
	MError ModuleError
	Err    error
}

// Deprecated: use NewCodeError
func NewCoreError(mError ModuleError, err error) *CoreError {
	return &CoreError{
		MError: mError,
		Err:    err,
	}
}

// Deprecated: use StackError
func NewInternalError(msg interface{}) error {
	switch v := msg.(type) {
	case string:
		return errors.New(v)
	case error:
		return errors.WithStack(v)
	default:
		log.Warn().Type("type", v).Msg("NewInternalError get unknown type")
		return errors.New(fmt.Sprintf("%v", v))
	}
}

func NewEntityNotFoundError(entityName string, id interface{}) error {
	return NewCodeError(
		EntityNotFoundErrCode,
		StackError(fmt.Errorf("entity %s with id %v not found", entityName, id)),
	)
}
func (e CoreError) Error() string {
	return e.Err.Error()
}

// Deprecated: use CodeError
type ModuleError struct {
	ModuleName string
	ModuleCode int
	ErrorCode  int
}

// Deprecated: remove soon
func NewValidateErrorOrNil(err error) error {
	if err == nil {
		return nil
	}

	return NewCoreError(ValidationError, err)
}

func (m ModuleError) GetModuleErrorCode() int {
	return m.ModuleCode*10000 + m.ErrorCode
}

func (m *ModuleError) Equals(m2 ModuleError) bool {
	return m.ErrorCode == m2.ErrorCode &&
		m.ModuleCode == m2.ModuleCode &&
		m.ModuleName == m2.ModuleName
}

// ------------------ New Version of Error Type ------------------

// CodeError is an error type that includes an error code
// in order to facilitate client-side error handling.
// If a CodeError is encountered, an HTTP 400 status code should be returned.
// This error type should be used to wrap errors cause from user actions,
// which we expect a corresponding specific client-side error handling.
type CodeError struct {
	ErrorCode string
	err       error
}

func NewCodeError(code string, err error) *CodeError {
	return &CodeError{
		ErrorCode: code,
		err:       err,
	}
}

func (e CodeError) Error() string {
	return e.err.Error()
}

func (e CodeError) Unwrap() error {
	return e.err
}

// StackError adds a stack trace to an error
// or creates a new error with a stack trace if a string is provided.
// It is intended to be used when handling errors from third-party libraries or custom-defined errors,
// allowing for a full stack trace to be included in logs,
// which is will integrate by zerolog.
func StackError(msg interface{}) error {
	switch v := msg.(type) {
	case string:
		return errors.New(v)
	case error:
		return errors.WithStack(v)
	default:
		log.Warn().Type("type", v).Msg("StackError received an unknown type")
		return errors.New(fmt.Sprintf("unexpected error type: %v", v))
	}
}
