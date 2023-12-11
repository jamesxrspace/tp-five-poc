package query

import (
	"context"

	"github.com/go-playground/validator/v10"
	"github.com/rs/zerolog/log"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/arch/domain/eventbus"
	"xrspace.io/server/modules/room/application/define"

	"xrspace.io/server/modules/room/application/query/enum"
	"xrspace.io/server/modules/room/domain"
	domainEvent "xrspace.io/server/modules/room/domain/event"
)

type FusionCustomAuthBody struct {
	Authorization string `json:"authorization" validate:"required"`
}

func (q *FusionCustomAuthBody) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

type FusionCustomAuthResponse struct {
	// reference: https://doc.photonengine.com/fusion/v2/manual/connection-and-matchmaking/custom-authentication
	// keyword: "Server Side", "Returning Data To Client"
	ResultCode enum.FusionResponseCode `json:"ResultCode"`
	Message    string                  `json:"Message"`
}

type FusionCustomAuthUseCase struct {
	eventBus       eventbus.IEventBus
	tokenValidator domain.IPhotonAccessTokenValidator
}

var _ application.IUseCase = (*FusionCustomAuthUseCase)(nil)

func NewFusionCustomAuthUseCase(dep *define.Dependency) *FusionCustomAuthUseCase {
	return &FusionCustomAuthUseCase{
		tokenValidator: dep.TokenValidator,
		eventBus:       dep.Bus,
	}
}

func (u *FusionCustomAuthUseCase) Execute(ctx context.Context, q any) (any, error) {
	query := q.(*FusionCustomAuthBody)
	var events []*event.Event

	defer func() {
		err := u.eventBus.PublishAll(events)
		log.Err(err).Msg("publish events")
	}()

	if err := query.Validate(); err != nil {
		events = append(events, domainEvent.NewRoomAuthFailedEvent("invalid request", err))

		return &FusionCustomAuthResponse{
			ResultCode: enum.FusionResponseCodeIncomplete,
			Message:    "invalid body",
		}, err
	}

	err := u.tokenValidator.PhotonValidate(ctx, query.Authorization)
	if err != nil {
		events = append(events, domainEvent.NewRoomAuthFailedEvent("invalid token", err))
		return &FusionCustomAuthResponse{
			ResultCode: enum.FusionResponseCodeFailure,
			Message:    "failure",
		}, nil
	}

	e := domainEvent.NewRoomAuthSuccessEvent()
	err = u.eventBus.Publish(e)

	return &FusionCustomAuthResponse{
		ResultCode: enum.FusionResponseCodeSuccess,
		Message:    "success",
	}, err
}
