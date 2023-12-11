package settings

import (
	"github.com/go-playground/validator/v10"
	"github.com/rs/zerolog/log"
	"github.com/spf13/viper"
)

// Config TODO: ticket TFB-91 use one nested app config instead of using separated config
type Config struct {
	Env      string
	App      AppConfig      `mapstructure:"app"`
	DocDB    DocDBConfig    `mapstructure:"doc_db"`
	OIDC     OIDCConfig     `mapstructure:"oidc"`
	Aws      AwsConfig      `mapstructure:"aws"`
	Avatar   AvatarConfig   `mapstructure:"avatar"`
	Logger   LoggerConfig   `mapstructure:"logger"`
	Redis    RedisConfig    `mapstructure:"redis"`
	Agora    AgoraConfig    `mapstructure:"agora"`
	Otel     OtelConfig     `mapstructure:"otel"`
	AIServer AIServerConfig `mapstructure:"ai_server"`
}

func (c *Config) Validate() error {
	validate := validator.New()
	return validate.Struct(c)
}

func Default() *Config {
	return &Config{
		Logger: LoggerConfig{
			Debug: false,
			Level: "info",
		},
	}
}

func NewConfig(configPath string) (*Config, error) {
	config := Default()

	viper.AutomaticEnv()
	viper.SetConfigName("env")
	viper.SetConfigType("yaml")

	viper.SetConfigFile(configPath)
	if err := viper.ReadInConfig(); err != nil {
		log.Warn().Str("file", configPath).Msg("config file not found")
		return nil, err
	}

	if err := viper.Unmarshal(config); err != nil {
		log.Warn().Str("file", configPath).Msg("cannot unmarshal config file")
		return nil, err
	}

	if err := config.Validate(); err != nil {
		log.Warn().Str("file", configPath).Msg("config file validation failed")
		return nil, err
	}

	return config, nil
}
