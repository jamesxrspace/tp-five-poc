package eventbus

import (
	"context"
	"reflect"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/domain/event"
	coreEvent "xrspace.io/server/core/arch/domain/eventbus"
)

func HandlerFunc[T application.IEventCommand](facade application.IFacade) coreEvent.Handler {
	return func(ctx context.Context, e *event.Event) error {
		var cmd T
		// create instance of T with reflection
		cmd = reflect.New(reflect.TypeOf(cmd).Elem()).Interface().(T)
		if err := cmd.SetEvent(e); err != nil {
			return err
		}
		_, err := facade.Execute(ctx, cmd)
		return err
	}
}
