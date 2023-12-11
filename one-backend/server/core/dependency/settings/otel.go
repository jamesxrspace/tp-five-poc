package settings

type OtelConfig struct {
	Address string `mapstructure:"address" validate:"omitempty,url"`
	AppName string `mapstructure:"appname" validate:"omitempty"`
}
