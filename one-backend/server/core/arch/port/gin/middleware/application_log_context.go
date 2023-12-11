package middleware

import (
	"time"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog/log"
)

var (
	// skip path list for the gin access logger
	skipLogPaths = []string{
		"/livez",
		"/readyz",
	}
)

func SetApplicationLogContext() gin.HandlerFunc {
	// each request will have a unique request id and trace id
	// we expect every log logged in application layer to have these fields
	// this middleware will set these fields to log context

	return func(c *gin.Context) {
		start := time.Now().UTC()
		requestID := c.GetHeader("x-request-id")
		traceID := c.GetHeader("x-b3-traceid")
		appLogger := log.Logger.With().
			Str("x-request-id", requestID).
			Str("x-b3-traceid", traceID).
			Caller().
			Stack().
			Logger()
		c.Request = c.Request.WithContext(appLogger.WithContext(c.Request.Context()))
		c.Next()
		shouldSkip := false
		for _, path := range skipLogPaths {
			if c.Request.URL.Path == path {
				shouldSkip = true
				break
			}
		}
		if !shouldSkip {
			log.Info().
				Str("method", c.Request.Method).
				Str("path", c.Request.URL.Path).
				Str("protocol", c.Request.Proto).
				Str("user_agent", c.Request.UserAgent()).
				Str("client_ip", c.ClientIP()).
				Str("requested_server_name", c.Request.Host).
				Str("authority", c.Request.Host).
				Int("response_code", c.Writer.Status()).
				Int("bytes_sent", c.Writer.Size()).
				Str("trace_id", traceID).
				Str("request_id", requestID).
				Dur("duration", time.Since(start)).
				Str("start_time", start.Format(time.RFC3339)).
				Str("x-forwarded-for", c.GetHeader("X-Forwarded-For")).
				Str("x-real-ip", c.GetHeader("X-Real-IP")).
				Msg("request handled")
		}
	}
}
