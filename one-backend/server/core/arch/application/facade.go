package application

import (
	"context"
	"reflect"

	"github.com/go-playground/validator/v10"
	"github.com/rs/zerolog"

	"xrspace.io/server/core/arch/core_error"
)

type IFacade interface {
	Execute(ctx context.Context, cmd any) (any, error)
}

// AbsFacade implicit implement IFacade
type AbsFacade struct {
	useCases map[reflect.Type]IUseCase
}

func NewAbsFacade() *AbsFacade {
	return &AbsFacade{
		useCases: make(map[reflect.Type]IUseCase),
	}
}

func (a *AbsFacade) RegisterUseCase(cmd any, uc IUseCase) {
	if _, ok := a.useCases[reflect.TypeOf(cmd)]; ok {
		panic("use case already registered")
	}

	a.useCases[reflect.TypeOf(cmd)] = uc
}

func (a *AbsFacade) Execute(ctx context.Context, cmd any) (any, error) {
	if c, ok := cmd.(ICommand); ok {
		if err := c.Validate(); err != nil {
			return nil, core_error.NewCodeError(core_error.ValidationErrCode, err)
		}
	} else {
		v := validator.New(validator.WithRequiredStructEnabled())
		if err := v.Struct(cmd); err != nil {
			return nil, core_error.NewCodeError(core_error.ValidationErrCode, err)
		}
	}

	uc := a.useCases[reflect.TypeOf(cmd)]

	if uc == nil {
		log := zerolog.Ctx(ctx)
		log.Fatal().Msgf("use case not found: %s", reflect.TypeOf(cmd).String())
	}

	return uc.Execute(ctx, cmd)
}
