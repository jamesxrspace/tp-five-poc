package gin

import (
	"errors"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog"

	"xrspace.io/server/core/arch/core_error"
)

// Deprecated: Using ResponseBodyV2 instead.
// Once all the CoreError are removed, this struct will be removed.
type ResponseBody struct {
	Message   string `json:"message,omitempty"`
	ErrorCode int    `json:"error_code,omitempty"`
	Data      any    `json:"data,omitempty"`
}

type ResponseBodyV2 struct {
	Message   string `json:"message,omitempty"`
	ErrorCode string `json:"error_code,omitempty"`
	Data      any    `json:"data,omitempty"`
}

func response(ctx *gin.Context, data any, err error) {
	log := zerolog.Ctx(ctx.Request.Context())
	switch v := err.(type) {
	case nil:
		ctx.PureJSON(http.StatusOK, &ResponseBody{
			Data: data,
		})
	case *core_error.CoreError:
		handledCoreError(ctx, *v)
	default:
		log.Error().Err(err).Msg("application error")
		var codeErr *core_error.CodeError
		if errors.As(err, &codeErr) {
			ctx.PureJSON(http.StatusBadRequest, &ResponseBodyV2{
				ErrorCode: codeErr.ErrorCode,
				Message:   codeErr.Error(),
			})
			return
		}
		ctx.PureJSON(http.StatusInternalServerError, &ResponseBody{
			Message: err.Error(),
		})
	}
}

// Deprecated: there will be no more CoreError in the future.
func handledCoreError(ctx *gin.Context, err core_error.CoreError) {
	log := zerolog.Ctx(ctx.Request.Context())
	switch {
	case core_error.RegisterErrorCodes[err.MError] == struct{}{}:
		ctx.PureJSON(http.StatusBadRequest, &ResponseBody{
			ErrorCode: err.MError.ErrorCode,
			Message:   err.Error(),
		})
		log.Error().Err(err.Err).Msg("core error")
	default:
		ctx.PureJSON(http.StatusInternalServerError, &ResponseBody{
			ErrorCode: http.StatusInternalServerError,
			Message:   "internal server error",
		})
		log.Error().Err(err).Msg("internal server error")
	}
}
