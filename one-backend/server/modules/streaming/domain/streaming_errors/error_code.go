package streaming_errors

import "xrspace.io/server/core/arch/core_error"

const moduleName = "streaming"

var (
	ValidateError = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 40001}
)

func init() {
	core_error.RegisterErrors(
		ValidateError,
	)
}
