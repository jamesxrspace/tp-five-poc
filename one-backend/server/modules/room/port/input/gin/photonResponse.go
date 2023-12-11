package gin

import (
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog/log"

	"xrspace.io/server/modules/room/application/query"
	"xrspace.io/server/modules/room/application/query/enum"
)

func photonResponse(ctx *gin.Context, data any, err error) {
	d := data.(*query.FusionCustomAuthResponse)
	switch d.ResultCode {
	case enum.FusionResponseCodeSuccess:
		ctx.JSON(http.StatusOK, data)
	default:
		ctx.JSON(http.StatusBadRequest, data)
		log.Error().Err(err).Msg("fusion auth failed")
	}
}
