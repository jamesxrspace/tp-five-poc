package error_code

import "xrspace.io/server/core/arch/core_error"

const ModuleName = "space"

var (
	CreateSpaceGroupError = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50001}
	ListSpaceGroupError   = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50002}
	UpdateSpaceGroupError = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50003}
	DeleteSpaceGroupError = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50004}
	CreateSpaceError      = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50005}
	UpdateSpaceError      = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50006}
	DeleteSpaceError      = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50007}
	ListSpaceError        = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 50008}
)

func init() {
	core_error.RegisterErrors(
		CreateSpaceGroupError,
		ListSpaceGroupError,
		UpdateSpaceGroupError,
		DeleteSpaceGroupError,
		CreateSpaceError,
		UpdateSpaceError,
		DeleteSpaceError,
		ListSpaceError,
	)
}
