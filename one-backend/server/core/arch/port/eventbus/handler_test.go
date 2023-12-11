package eventbus

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/domain/event"
)

type testCommand struct {
	event *event.Event
}

func (c *testCommand) SetEvent(event *event.Event) error {
	c.event = event
	return nil
}

type facade struct {
	execute func(ctx context.Context, cmd any) (any, error)
}

// Execute implements application.IFacade.
func (f *facade) Execute(ctx context.Context, cmd any) (any, error) {
	return f.execute(ctx, cmd)
}

var _ application.IEventCommand = (*testCommand)(nil)
var _ application.IFacade = (*facade)(nil)

func TestHandlerFunc(t *testing.T) {
	var actualCmd *testCommand
	fa := &facade{
		execute: func(ctx context.Context, cmd any) (any, error) {
			actualCmd = cmd.(*testCommand)
			return true, nil
		},
	}
	fu := HandlerFunc[*testCommand](fa)
	e := &event.Event{
		ID:    "testID",
		Topic: "testTopic",
	}
	fu(context.Background(), e)

	assert.NotNil(t, actualCmd)
	assert.Equal(t, e, actualCmd.event)
}
