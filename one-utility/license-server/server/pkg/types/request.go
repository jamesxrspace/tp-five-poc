package types

import (
	"fmt"
	"time"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/service/dynamodb"
	"github.com/aws/aws-sdk-go/service/dynamodb/dynamodbattribute"
	"github.com/rs/zerolog/log"
)

// the action of the request.
const (
	ACQUIRE = "acquire"
	REVOKE  = "revoke"

	EXPIRE_SECONDS = 60 * 60 * 2 // 2 hours
)

// the incoming request from lambda invocation.
type Request struct {
	svc       *dynamodb.DynamoDB
	tableName string

	Action string `json:"action"`
	ID     string `json:"id"`
}

// execute the action and return the response.
func (r Request) Execute(svc *dynamodb.DynamoDB, name string) (*Response, error) {
	r.svc = svc
	r.tableName = name

	switch r.Action {
	case ACQUIRE:
		log.Info().Str("action", r.Action).Msg("acquire license")
		return r.Acquire()
	case REVOKE:
		log.Info().Str("action", r.Action).Msg("revoke license")
		return nil, r.Revoke()
	default:
		log.Warn().Str("action", r.Action).Msg("unknown action")
		return nil, fmt.Errorf("unknown action: %s", r.Action)
	}
}

func (r Request) Acquire() (*Response, error) {
	if r.Action != ACQUIRE {
		err := fmt.Errorf("cannot acquire with action: %s", r.Action)
		log.Error().Err(err).Msg("failed to acquire")
		return nil, err

	}

	resp := r.getFreeLicense()
	if resp == nil {
		err := fmt.Errorf("failed to get free license")
		return nil, err
	}
	log.Debug().Interface("resp", resp).Msg("got free license")

	if err := r.acquire(resp); err != nil {
		log.Error().Err(err).Msg("failed to acquire license")
		return nil, err
	}

	log.Info().Interface("resp", resp).Msg("acquired license")
	return resp, nil
}

func (r Request) getFreeLicense() *Response {
	now := time.Now().Unix()        // the current seconds since EPOCH
	expired := now - EXPIRE_SECONDS // the expired seconds since EPOCH

	input := &dynamodb.ScanInput{
		TableName: aws.String(r.tableName),

		ExpressionAttributeValues: map[string]*dynamodb.AttributeValue{
			":t": {
				N: aws.String(fmt.Sprintf("%d", expired)),
			},
		},
		FilterExpression: aws.String("attribute_not_exists(used) OR acquired_timestamp < :t"),
	}

	result, err := r.svc.Scan(input)
	if err != nil {
		log.Error().Err(err).Msg("failed to scan")
		return nil
	}

	if len(result.Items) == 0 {
		log.Info().Msg("no free licenses")
		return nil
	}

	resp := &Response{}
	if err := dynamodbattribute.UnmarshalMap(result.Items[0], resp); err != nil {
		log.Error().Err(err).Msg("failed to unmarshal")
		return nil
	}

	return resp
}

func (r Request) acquire(resp *Response) error {
	now := time.Now().Unix() // the current seconds since EPOCH

	input := &dynamodb.UpdateItemInput{
		TableName: aws.String(r.tableName),

		Key: map[string]*dynamodb.AttributeValue{
			"id": {
				S: aws.String(resp.ID),
			},
		},
		ExpressionAttributeValues: map[string]*dynamodb.AttributeValue{
			":u": {
				N: aws.String("1"),
			},
			":t": {
				N: aws.String(fmt.Sprintf("%d", now)),
			},
		},
		UpdateExpression: aws.String("SET used = if_not_exists(used, :u), acquired_timestamp = :t"),
	}

	_, err := r.svc.UpdateItem(input)
	return err
}

func (r Request) Revoke() error {
	if r.Action != REVOKE {
		err := fmt.Errorf("cannot revoke with action: %s", r.Action)
		log.Error().Err(err).Msg("failed to revoke")
		return err
	}

	return r.revoke(r.ID)
}

func (r Request) revoke(id string) error {
	input := &dynamodb.UpdateItemInput{
		TableName: aws.String(r.tableName),

		Key: map[string]*dynamodb.AttributeValue{
			"id": {
				S: aws.String(id),
			},
		},
		UpdateExpression: aws.String("REMOVE used, acquired_timestamp"),
	}

	output, err := r.svc.UpdateItem(input)
	if err != nil {
		log.Error().Err(err).Msg("failed to update item")
		return err
	}

	log.Info().Interface("output", output).Msg("updated item")
	return err
}
