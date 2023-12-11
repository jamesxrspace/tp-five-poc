package error_code

import "xrspace.io/server/core/arch/core_error"

const ModuleName = "aigc"

var (
	QueuePopError   = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 60001}
	InferenceError  = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 60002}
	ParseMsgError   = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 60003}
	ParseS3UrlError = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 60004}
	GetUrlError     = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 60005}
)

func init() {
	core_error.RegisterErrors(
		QueuePopError,
		InferenceError,
		ParseMsgError,
		ParseS3UrlError,
		GetUrlError,
	)
}
