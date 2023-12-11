package main

import (
	"github.com/rs/zerolog/log"
	"github.com/spf13/viper"
)

type Config struct {
	// AWS settings
	AWSRegion   string  `mapstructure:"AWS_REGION"`
	AWSEndpoint *string `mapstructure:"AWS_ENDPOINT_URL"`

	// DynamoDB
	TableName string `mapstructure:"TABLE_NAME"`
}

func LoadConfig() *Config {
	viper.SetDefault("AWS_REGION", "us-west-1")
	viper.SetDefault("TABLE_NAME", "test-table")

	viper.AutomaticEnv()

	config := Config{}
	if err := viper.Unmarshal(&config); err != nil {
		// cannot unmarshal config file, abort
		log.Fatal().Err(err).Msg("failed to unmarshal config")
	}

	log.Info().Interface("config", config).Msg("loaded config")
	return &config
}
