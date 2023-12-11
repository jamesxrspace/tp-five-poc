package core_error

// core error code start with 0

const (
	ModuleName = "core"
	ModuleCode = 99
)

var (
	// Deprecated: use ValidationErrCode
	ValidationError = ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 1}
	// Deprecated: use EntityNotFoundErrCode
	EntityNotFound        = ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 2}
	ValidationErrCode     = "000001"
	EntityNotFoundErrCode = "000002"
	PermissionErrCode     = "000003"
)

func init() {
	RegisterErrors(ValidationError)
	RegisterErrors(EntityNotFound)
	RegisterErrCodes(ValidationErrCode, EntityNotFoundErrCode, PermissionErrCode)
}
