package settings

type AvatarConfig struct {
	DefaultAvatar DefaultAvatar `mapstructure:"default_avatar" validate:"required"`
}

type DefaultAvatar struct {
	AvatarUrl string    `mapstructure:"avatar_url" validate:"required"`
	Thumbnail Thumbnail `mapstructure:"thumbnail" validate:"required"`
}

type Thumbnail struct {
	Head      string `mapstructure:"head" validate:"required"`
	UpperBody string `mapstructure:"upper_body" validate:"required"`
	FullBody  string `mapstructure:"full_body" validate:"required"`
}
