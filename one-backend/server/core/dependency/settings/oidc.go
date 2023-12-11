package settings

type Endpoints struct {
	Userinfo   string `mapstructure:"userinfo" validate:"required"`
	Jwks       string `mapstructure:"jwks" validate:"required"`
	Management string `mapstructure:"management" validate:"required"`
	CreateUser string `mapstructure:"create_user" validate:"required"`
}

type Guest struct {
	EmailPrefix string `mapstructure:"email_prefix" validate:"required"`
	EmailSuffix string `mapstructure:"email_suffix" validate:"required"`
	Company     string `mapstructure:"company" validate:"required"`
}

type OIDCConfig struct {
	Endpoints           Endpoints `mapstructure:"endpoints" validate:"required"`
	Guest               Guest     `mapstructure:"guest" validate:"required"`
	DomainUrl           string    `mapstructure:"domain_url" validate:"required"`
	RedirectUrl         string    `mapstructure:"redirect_url" validate:"required"`
	AuthCodeRedirectUrl string    `mapstructure:"auth_code_redirect_url" validate:"required"`
	PoolID              string    `mapstructure:"pool_id" validate:"required"`
	Secret              string    `mapstructure:"secret" validate:"required"`
	HttpTimeout         int       `mapstructure:"http_timeout"`
}
