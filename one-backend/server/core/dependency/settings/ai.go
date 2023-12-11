package settings

type AIServerConfig struct {
	AigcMotion *AigcMotionConfig `mapstructure:"aigc_motion" validate:"required"`
	OpenAi     *OpenAiConfig     `mapstructure:"open_ai" validate:"required"`
}

type AigcMotionConfig struct {
	Endpoint string `mapstructure:"endpoint" validate:"required"`
}

type OpenAiConfig struct {
	ApiKey         string `mapstructure:"api_key" validate:"required"`
	OrganizationID string `mapstructure:"organization_id" validate:"required"`
}
