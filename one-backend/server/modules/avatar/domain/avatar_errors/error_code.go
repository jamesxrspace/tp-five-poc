package avatar_errors

import "xrspace.io/server/core/arch/core_error"

const moduleName = "avatar"

var (
	ValidateError          = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30001}
	GetAvatarError         = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30002}
	AssetUploadError       = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30003}
	SaveAvatarError        = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30004}
	GetAvatarPlayerError   = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30005}
	SetCurrentAvatarError  = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30006}
	SaveAvatarPlayerError  = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30007}
	AvatarNotExistError    = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30008}
	ListAvatarPlayersError = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30009}
	ListAvatarError        = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30010}
	SetDefaultAvatarsError = core_error.ModuleError{ModuleName: moduleName, ErrorCode: 30011}
)

func init() {
	core_error.RegisterErrors(
		ValidateError,
		GetAvatarError,
		AssetUploadError,
		SaveAvatarError,
		GetAvatarPlayerError,
		SetCurrentAvatarError,
		SaveAvatarPlayerError,
		AvatarNotExistError,
		ListAvatarPlayersError,
		ListAvatarError,
		SetDefaultAvatarsError,
	)

}
