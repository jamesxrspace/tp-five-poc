package domain

import "xrspace.io/server/core/arch/core_error"

const ModuleName = "daily_build"

var (
	ListDailyBuildError = core_error.ModuleError{ModuleName: ModuleName, ErrorCode: 10001}
)

func init() {
	core_error.RegisterErrors(ListDailyBuildError)
}
