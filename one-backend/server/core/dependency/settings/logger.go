package settings

type SentryConfig struct {
	DSN string `mapstructure:"dsn" validate:"omitempty,url"`

	// the sampling rate, 0.0 ~ 1.0. and 0.0 means no tracing
	Sampling float64 `mapstructure:"sampling" validate:"omitempty,min=0,max=1"`
}

type LoggerConfig struct {
	Debug bool   `mapstructure:"debug"`
	Level string `mapstructure:"level" validate:"oneof=panic fatal error warn info debug"`

	Sentry SentryConfig `mapstructure:"sentry"`
}
