package application

import (
	"context"
	"errors"
	"testing"

	"github.com/stretchr/testify/suite"
	"xrspace.io/server/core/arch/core_error"
	dblocal "xrspace.io/server/core/dependency/database/docdb/local"
	"xrspace.io/server/modules/space/application/command"
)

func TestCommandValidation(t *testing.T) {
	s := new(TestSuite)
	suite.Run(t, s)
}

type TestSuite struct {
	suite.Suite
}

func (s *TestSuite) TestCommandValidation() {
	// arrange
	cmd := command.CreateSpaceGroupCommand{Name: ""}

	// act
	facade := NewFacade(nil, nil, dblocal.NewUnitOfWork())
	_, err := facade.Execute(context.Background(), cmd)

	// assert
	var expErr *core_error.CodeError
	s.True(errors.As(err, &expErr))
	s.Equal(core_error.ValidationErrCode, expErr.ErrorCode)
}
