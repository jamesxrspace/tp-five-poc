package main

import (
	"context"

	"github.com/aws/aws-lambda-go/lambda"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/dynamodb"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/log"

	"xrspace.io/license-server/pkg/types"
)

var (
	config *Config          // the customized config
	sess   *session.Session // the AWS session
)

func init() {
	zerolog.TimeFieldFormat = zerolog.TimeFormatUnix
	log.Trace().Msg("finished initialized")

	// load config from environment variables
	config = LoadConfig()

	// load AWS session
	awsConfig := &aws.Config{
		Region:   aws.String(config.AWSRegion),
		Endpoint: config.AWSEndpoint,
	}
	sess = session.Must(session.NewSession(awsConfig))
}

func handler(ctx context.Context, req types.Request) (*types.Response, error) {
	log.Info().Str("action", req.Action).Msg("handler called")

	svc := dynamodb.New(sess)
	return req.Execute(svc, config.TableName)
}

func main() {
	log.Info().Msg("starting lambda ...")
	defer log.Info().Msg("lambda stopped")

	lambda.Start(handler)
}
