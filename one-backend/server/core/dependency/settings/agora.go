package settings

type AgoraConfig struct {
	AppId          string `mapstructure:"app_id" validate:"required"`
	AppCertificate string `mapstructure:"app_certificate" validate:"required"`
}
