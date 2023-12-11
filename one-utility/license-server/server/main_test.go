package main

import (
	"context"
	"os"
	"testing"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/dynamodb"
	"github.com/aws/aws-sdk-go/service/dynamodb/dynamodbattribute"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/log"

	"xrspace.io/license-server/pkg/types"
)

var (
	item = types.Response{
		ID:       "1234",
		Email:    "user@example.com",
		Password: "password",
		Serial:   "1234567890",
	}
)

func init() {
	// make log output more readable
	writer := zerolog.ConsoleWriter{Out: os.Stderr}
	log.Logger = zerolog.New(writer).With().Timestamp().Logger()
}

func TestMain(m *testing.M) {
	setUp()
	code := m.Run()
	tearDown()
	os.Exit(code)
}

func TestHandler(t *testing.T) {
	ctx := context.Background()

	reqAcquire := types.Request{
		Action: types.ACQUIRE,
	}

	resp, err := handler(ctx, reqAcquire)
	if err != nil {
		t.Errorf("failed to acquire license: %v", err)
		return
	}

	reqRevoke := types.Request{
		Action: types.REVOKE,
		ID:     resp.ID,
	}
	_, err = handler(ctx, reqRevoke)
	if err != nil {
		t.Errorf("failed to revoke license: %v", err)
		return
	}
}

func TestNoFreeLicense(t *testing.T) {
	ctx := context.Background()

	reqAcquire := types.Request{
		Action: types.ACQUIRE,
	}

	_, _ = handler(ctx, reqAcquire)
	resp, err := handler(ctx, reqAcquire)
	if err == nil {
		t.Errorf("should not acquire license: %v", resp)
		return
	}

	reqRevoke := types.Request{
		Action: types.REVOKE,
		ID:     item.ID,
	}

	_, _ = handler(ctx, reqRevoke)
}

func setUp() {
	log.Info().Msg("global setup ...")

	// override AWS settings
	endpoint := "http://localhost:4566"
	config := &aws.Config{
		Region:   aws.String(config.AWSRegion),
		Endpoint: aws.String(endpoint),
	}
	sess = session.Must(session.NewSession(config))
	log.Info().Interface("endpoint", endpoint).Msg("force setup AWS endpoint")

	svc := dynamodb.New(sess)
	createTable(svc)
	putItem(svc, item)
}

func tearDown() {
	log.Info().Msg("global teardown ...")

	svc := dynamodb.New(sess)
	deleteTable(svc)
}

func createTable(svc *dynamodb.DynamoDB) {
	input := &dynamodb.CreateTableInput{
		TableName: aws.String(config.TableName),

		AttributeDefinitions: []*dynamodb.AttributeDefinition{
			{
				AttributeName: aws.String("id"),
				AttributeType: aws.String("S"),
			},
		},
		KeySchema: []*dynamodb.KeySchemaElement{
			{
				AttributeName: aws.String("id"),
				KeyType:       aws.String("HASH"),
			},
		},
		ProvisionedThroughput: &dynamodb.ProvisionedThroughput{
			ReadCapacityUnits:  aws.Int64(10),
			WriteCapacityUnits: aws.Int64(10),
		},
	}

	if _, err := svc.CreateTable(input); err != nil {
		log.Error().Err(err).Msg("failed to create table")
		return
	}

	log.Info().Str("table", config.TableName).Msg("created table")
}

func deleteTable(svc *dynamodb.DynamoDB) {
	input := &dynamodb.DeleteTableInput{
		TableName: aws.String(config.TableName),
	}

	if _, err := svc.DeleteTable(input); err != nil {
		log.Error().Err(err).Msg("failed to delete table")
		return
	}

	log.Info().Str("table", config.TableName).Msg("deleted table")
}

func putItem(svc *dynamodb.DynamoDB, resp types.Response) {
	item, err := dynamodbattribute.MarshalMap(resp)
	if err != nil {
		log.Error().Err(err).Msg("failed to marshal map")
		return
	}

	input := &dynamodb.PutItemInput{
		TableName: aws.String(config.TableName),
		Item:      item,
	}

	if _, err := svc.PutItem(input); err != nil {
		log.Error().Err(err).Msg("failed to put item")
		return
	}

	log.Info().Str("table", config.TableName).Msg("put test item")
}
