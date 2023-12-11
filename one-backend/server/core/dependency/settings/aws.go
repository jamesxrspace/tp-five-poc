package settings

type AwsConfig struct {
	S3        *S3Config `mapstructure:"s3" validate:"required"`
	AccessKey string    `mapstructure:"access_key"`
	SecretKey string    `mapstructure:"secret_key"`
	Endpoint  string    `mapstructure:"endpoint"`
	Region    string    `mapstructure:"region" validate:"required"`
}

type S3Config struct {
	Buckets map[string]*Bucket `mapstructure:"buckets"`
	// TODO:Bucket should be removed after refactoring Avatar's storage
	Bucket           string `mapstructure:"bucket"`
	CloudfrontDomain string `mapstructure:"cloudfront_domain"`
}

type Bucket struct {
	Name             string `mapstructure:"name" validate:"required"`
	CloudfrontDomain string `mapstructure:"cloudfront_domain" validate:"required"`
}
