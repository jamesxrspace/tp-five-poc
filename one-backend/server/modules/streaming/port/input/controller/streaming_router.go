package controller

import (
	"xrspace.io/server/core/dependency/gin_engine/router"
	rtc_token_provider "xrspace.io/server/core/dependency/streaming/rtc_token_provider/agora"
)

var _ router.IRouter = (*StreamingRouter)(nil)

type StreamingRouter struct {
	streamingController *StreamingController
	dependencies        *router.Dependencies
}

func NewStreamingRouter(dependencies *router.Dependencies) *StreamingRouter {
	rtcTokenProvider := rtc_token_provider.NewRTCTokenProvider(&dependencies.Config.Agora)
	return &StreamingRouter{
		streamingController: NewStreamingController(rtcTokenProvider),
		dependencies:        dependencies,
	}
}

func (r *StreamingRouter) RegisterRoutes(g router.RouterGroup) {
	v1 := g.ApiGroup.Group("/v1")
	streaming := v1.Group("/streaming")
	streaming.POST("/token", r.streamingController.GenerateRTCToken)
}
