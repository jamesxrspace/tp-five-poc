package core_error

import (
	"errors"
	"sync"

	"github.com/rs/zerolog/log"
)

var (
	mu                 sync.Mutex
	RegisterErrorCodes = make(map[ModuleError]struct{}) // a common way to use as set data type. struct{} is a placeholder for a value that occupies no memory
	RegisteredErrCodes = make(map[string]struct{})      // a common way to use as set data type. struct{} is a placeholder for a value that occupies no memory
)

// Deprecated: use registerErrCode
func registerError(mError ModuleError) error {
	mu.Lock()
	defer mu.Unlock()

	if _, exists := RegisterErrorCodes[mError]; exists {
		return errors.New("error code already registered")
	}

	RegisterErrorCodes[mError] = struct{}{}
	return nil
}

// Deprecated: use RegisterErrCodes
func RegisterErrors(mErrors ...ModuleError) {
	for _, mError := range mErrors {
		if err := registerError(mError); err != nil {
			log.Panic().Err(err).Str("module", mError.ModuleName).Int("code", mError.ErrorCode).Msg("cannot register error")
		}
	}
}

func registerErrCode(errCode string) error {
	if _, exists := RegisteredErrCodes[errCode]; exists {
		return errors.New("error code already registered: %s" + errCode)
	}
	RegisteredErrCodes[errCode] = struct{}{}
	return nil
}

func RegisterErrCodes(codes ...string) {
	for _, code := range codes {
		if err := registerErrCode(code); err != nil {
			log.Fatal().Err(err).Str("code", code).Msg("cannot register error code")
		}
	}
}
