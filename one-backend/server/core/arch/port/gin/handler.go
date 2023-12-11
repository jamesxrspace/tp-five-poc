package gin

import (
	"github.com/gin-gonic/gin"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/port/gin/binding"
)

type CustomResponseHandler func(ctx *gin.Context, data any, err error)

func HandlerFunc[T any](facade application.IFacade) gin.HandlerFunc {
	return func(context *gin.Context) {
		cmd := new(T)
		Handle(context, facade, cmd)
	}
}

func Handle(ctx *gin.Context, facade application.IFacade, cmd any) {
	HandleCustomResponse(ctx, facade, cmd, response)
}
func HandleCustomResponse(ctx *gin.Context, facade application.IFacade, cmd any, responseHandler CustomResponseHandler) {
	// https://github.com/gin-gonic/gin/pull/2812#issuecomment-901606114

	var result any

	err := binding.ShouldBind(ctx, cmd)

	if err != nil {
		responseHandler(ctx, result, err)
		return
	}

	result, err = facade.Execute(ctx.Request.Context(), cmd)
	responseHandler(ctx, result, err)
}
