package query

import (
	"context"
	"errors"
	"testing"

	"github.com/stretchr/testify/assert"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/dependency/eventbus/inmem"
	"xrspace.io/server/modules/room/application/define"
	domainEvent "xrspace.io/server/modules/room/domain/event"
	tokenValidatorInMem "xrspace.io/server/modules/room/port/output/token_validator/inmem"
)

func TestFusionCustomAuthUseCase_Execute(t *testing.T) {
	bus := inmem.NewInMemEventBus()
	tokenValidator := tokenValidatorInMem.NewTokenValidator()

	type args struct {
		ctx   context.Context
		query *FusionCustomAuthBody
	}
	validArg := args{
		ctx: context.Background(),
		query: &FusionCustomAuthBody{
			Authorization: "this is a token",
		},
	}

	tests := []struct {
		name        string
		expectEvent *event.Event
		args        args
		shouldSucc  bool
	}{
		{
			name:        "when success, should have event room_auth_success",
			shouldSucc:  true,
			expectEvent: domainEvent.NewRoomAuthSuccessEvent(),
			args:        validArg,
		},
		{
			name:        "when invalid token, should have event room_auth_failure",
			shouldSucc:  false,
			expectEvent: domainEvent.NewRoomAuthFailedEvent("token invalid", errors.New("token invalid")),
			args:        validArg,
		},
		{
			name:        "when invalid request, should have event room_auth_failure",
			shouldSucc:  true,
			expectEvent: domainEvent.NewRoomAuthFailedEvent("invalid request", errors.New("invalid request")),
			args: args{
				ctx: context.Background(),
				query: &FusionCustomAuthBody{
					Authorization: "",
				},
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			var events []*event.Event
			_ = bus.SubscribeAll(func(_ context.Context, e *event.Event) error {
				events = append(events, e)
				return nil
			})
			tokenValidator.Should(tt.shouldSucc)

			dep := &define.Dependency{
				Bus:            bus,
				TokenValidator: tokenValidator,
			}

			u := NewFusionCustomAuthUseCase(dep)
			_, _ = u.Execute(tt.args.ctx, tt.args.query)

			assert.Equal(t, 1, len(events))
			assert.Equal(t, tt.expectEvent.Topic, events[0].Topic)
		})
	}
}
