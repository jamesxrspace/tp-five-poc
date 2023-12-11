package error

import "xrspace.io/server/core/arch/core_error"

const (
	ModuleName = "feed"
	ModuleCode = 12
)

var (
	ErrRetrieveReel          = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 1}
	ErrSaveReel              = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 2}
	ErrReelNotFound          = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 3}
	ErrNotAllowedPublishReel = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 4}
	ErrReelAlreadyPublished  = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 5}
	ErrPublishReel           = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 6}
	ErrRetrieveFeed          = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 7}
	ErrSaveFeed              = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 8}
	ErrFeedExists            = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 9}
	ErrFeedNotFound          = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 10}
	ErrFeedCategoryNotFound  = core_error.ModuleError{ModuleName: ModuleName, ModuleCode: ModuleCode, ErrorCode: 11}
)

func init() {
	core_error.RegisterErrors(
		ErrRetrieveReel,
		ErrSaveReel,
		ErrReelNotFound,
		ErrNotAllowedPublishReel,
		ErrReelAlreadyPublished,
		ErrRetrieveFeed,
		ErrSaveFeed,
		ErrFeedNotFound,
		ErrFeedExists,
		ErrFeedCategoryNotFound,
	)
}
