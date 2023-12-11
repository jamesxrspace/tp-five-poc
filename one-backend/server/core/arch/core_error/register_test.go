package core_error

import (
	"testing"

	"github.com/stretchr/testify/suite"
)

func TestRegisterErrorTestSuite(t *testing.T) {
	suite.Run(t, new(RegisterErrorTestSuite))
}

type RegisterErrorTestSuite struct {
	suite.Suite
}

func (s *RegisterErrorTestSuite) SetupTest() {
	RegisterErrorCodes = make(map[ModuleError]struct{})
}

func (s *RegisterErrorTestSuite) Test_WhenRegister_ShouldSuccess() {
	s.NotPanics(func() {
		RegisterErrors(ModuleError{ModuleName: "test", ErrorCode: 1})
		RegisterErrors(ModuleError{ModuleName: "test", ErrorCode: 2})
	})
}
func (s *RegisterErrorTestSuite) Test_WhenRegisterDuplicate_ShouldSuccess2() {
	s.NotPanics(func() {
		RegisterErrors(ModuleError{ModuleName: "test", ErrorCode: 1, ModuleCode: 1})
		RegisterErrors(ModuleError{ModuleName: "test", ErrorCode: 1})
	})
}
func (s *RegisterErrorTestSuite) Test_WhenRegisterDuplicate_ShouldFail() {
	s.Panics(func() {
		RegisterErrors(ModuleError{ModuleName: "test", ErrorCode: 1})
		RegisterErrors(ModuleError{ModuleName: "test", ErrorCode: 1})
	})
}
