package settings

type RedisConfig struct {
	Address  string `mapstructure:"address" validate:"required"`
	Password string `mapstructure:"password"`
	// Pointer is used to differentiate between a zero value and unset value.
	// More info: https://github.com/go-playground/validator/issues/290
	DB *int `mapstructure:"db" validate:"required"`
}
