package gin

import (
	"github.com/gin-gonic/gin"

	"github.com/prometheus/client_golang/prometheus/promhttp"
)

type MetricController struct {
}

func NewMetricController() *MetricController {
	return &MetricController{}
}

func (c *MetricController) Register(engine *gin.Engine) *gin.Engine {
	engine.GET("/metrics", c.handler())
	return engine
}

func (c *MetricController) handler() gin.HandlerFunc {
	h := promhttp.Handler()
	return func(ctx *gin.Context) {
		h.ServeHTTP(ctx.Writer, ctx.Request)
	}
}
