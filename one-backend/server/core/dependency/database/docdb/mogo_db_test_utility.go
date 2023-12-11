package docdb

import (
	"context"

	"github.com/stretchr/testify/suite"
	"github.com/tryvium-travels/memongo"
)

type InmemMongoBasicTestSuite struct {
	suite.Suite
	mongoServer *memongo.Server
	DbDoc       *DocDB
}

// SetupSuite initializes the in-memory mongo for each test.
func (suite *InmemMongoBasicTestSuite) SetupTest() {
	var err error
	suite.DbDoc, suite.mongoServer, err = NewMemgoDocDB(context.Background())
	if err != nil {
		panic(err)
	}
}

// TearDownSuite cleans up resources by stopping the in-memory MongoDB after each test.
func (suite *InmemMongoBasicTestSuite) TearDownSuite() {
	suite.mongoServer.Stop()
}
