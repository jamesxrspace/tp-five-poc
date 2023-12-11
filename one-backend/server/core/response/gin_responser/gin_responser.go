package gin_responser

import (
	"io"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog/log"

	"xrspace.io/server/core/arch/core_error"
)

type response struct {
	Data      any    `json:"data,omitempty"`
	Message   string `json:"message,omitempty"`
	ErrorCode int    `json:"error_code,omitempty"`
}

type GinResponser struct {
}

func NewGinResponser() *GinResponser {
	return &GinResponser{}
}

// Response TODO: ticket: TFB-128 Integrate error code and http code, and then remove switch case section in the gin responser.
func (r *GinResponser) Response(ctx *gin.Context, data any, err error) {
	if e, ok := err.(core_error.CoreError); ok {
		errCode := e.MError.ErrorCode
		httpCode := 0
		switch {
		case core_error.RegisterErrorCodes[e.MError] == struct{}{}:
			httpCode = http.StatusBadRequest
		default:
			httpCode = http.StatusInternalServerError
		}
		ctx.JSON(httpCode, &response{
			ErrorCode: errCode,
			Message:   err.Error(),
		})
		return
	}

	if err != nil {
		log.Error().Err(err).Msg("legacy responser")
		ctx.JSON(http.StatusBadRequest, &response{
			Message: err.Error(),
		})
		return
	}

	ctx.PureJSON(http.StatusOK, &response{
		Data: data,
	})
}

func (r *GinResponser) StreamResponse(ctx *gin.Context, msgs chan any, name string) bool {
	return ctx.Stream(func(w io.Writer) bool {
		msg, ok := <-msgs
		if !ok {
			return false
		}
		ctx.SSEvent(name, msg)
		return true
	})
}
