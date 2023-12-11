package settings

type AppConfig struct {
	Port   string              `mapstructure:"port" validate:"required"`
	AppIDs []map[string]string `mapstructure:"app_ids" validate:"required"`
}
