package settings

type DocDBConfig struct {
	ConnectionUri       string `mapstructure:"connection_uri" validate:"required"`
	ConnectionTimeoutMS int    `mapstructure:"connection_timeout_ms"`
	SocketTimeoutMS     int    `mapstructure:"socket_timeout_ms"`
	MaxPoolSize         uint64 `mapstructure:"max_pool_size"`
	DefaultDB           string `mapstructure:"default_db" validate:"required"`
	Username            string `mapstructure:"username"`
	Password            string `mapstructure:"password"`
}
