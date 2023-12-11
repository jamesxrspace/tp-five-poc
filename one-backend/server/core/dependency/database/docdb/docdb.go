package docdb

import (
	"context"
	"time"

	"github.com/rs/zerolog/log"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.opentelemetry.io/contrib/instrumentation/go.mongodb.org/mongo-driver/mongo/otelmongo"

	"xrspace.io/server/core/dependency/settings"
)

type DocDB struct {
	client *mongo.Client
	config *settings.DocDBConfig
}

func NewLocalDocDB(ctx context.Context) (*DocDB, error) {
	config := settings.DocDBConfig{
		ConnectionUri:       "mongodb://localhost:27017",
		ConnectionTimeoutMS: 1000,
		SocketTimeoutMS:     1000,
		MaxPoolSize:         100,
		DefaultDB:           "test",
		Username:            "root",
		Password:            "example",
	}
	return NewDocDB(ctx, &config)
}

func NewDocDB(ctx context.Context, config *settings.DocDBConfig) (*DocDB, error) {
	clientOptions := options.Client()
	clientOptions.Monitor = otelmongo.NewMonitor()
	clientOptions.ApplyURI(config.ConnectionUri)
	clientOptions.SetConnectTimeout(time.Duration(config.ConnectionTimeoutMS) * time.Millisecond)
	clientOptions.SetSocketTimeout(time.Duration(config.SocketTimeoutMS) * time.Millisecond)
	clientOptions.SetRetryWrites(true)
	clientOptions.SetMaxPoolSize(config.MaxPoolSize)
	clientOptions.SetReadPreference(options.Client().ReadPreference)
	clientOptions.SetWriteConcern(options.Client().WriteConcern)
	if config.Username != "" || config.Password != "" {
		clientOptions.SetAuth(options.Credential{
			Username: config.Username,
			Password: config.Password,
		})
	}

	client, err := mongo.Connect(ctx, clientOptions)
	if err != nil {
		return nil, err
	}

	if err := client.Ping(ctx, nil); err != nil {
		return nil, err
	}

	log.Info().Msg("connected to DocumentDB!")
	return &DocDB{
		client: client,
		config: config,
	}, nil
}

func (db *DocDB) Client() *mongo.Client {
	return db.client
}

func (db *DocDB) Ping(ctx context.Context) error {
	return db.client.Ping(ctx, nil)
}

func (db *DocDB) createIndex(ctx context.Context, collection string, keys []string, sorts []int32, unique bool, indexOptions *options.IndexOptions) error {
	c := db.client.Database(db.config.DefaultDB).Collection(collection)
	opts := options.CreateIndexes().SetMaxTime(3 * time.Second)
	index := db.yieldIndexModel(keys, sorts, unique, indexOptions)
	_, err := c.Indexes().CreateOne(ctx, index, opts)
	return err
}

func (db *DocDB) PopulateIndex(ctx context.Context, collection, key string, sort int32, unique bool) error {
	return db.createIndex(ctx, collection, []string{key}, []int32{sort}, unique, nil)
}

func (db *DocDB) PopulateCompoundIndex(ctx context.Context, collection string, keys []string, sorts []int32, unique bool) error {
	return db.createIndex(ctx, collection, keys, sorts, unique, nil)
}

func (db *DocDB) PopulateTTLIndex(ctx context.Context, collection, key string, sort int32, unique bool, ttl time.Duration) error {
	indexOptions := options.Index().SetExpireAfterSeconds(int32(ttl.Seconds()))
	return db.createIndex(ctx, collection, []string{key}, []int32{sort}, unique, indexOptions)
}

func (db *DocDB) yieldIndexModel(keys []string, sorts []int32, unique bool, indexOpt *options.IndexOptions) mongo.IndexModel {
	SetKeysDoc := bson.D{}

	for index := range keys {
		key := keys[index]
		sort := sorts[index]
		SetKeysDoc = append(SetKeysDoc, bson.E{Key: key, Value: sort})
	}
	if indexOpt == nil {
		indexOpt = options.Index()
	}
	indexOpt.SetUnique(unique)
	index := mongo.IndexModel{
		Keys:    SetKeysDoc,
		Options: indexOpt,
	}
	return index
}

func (db *DocDB) Collection(name string) *mongo.Collection {
	return db.client.Database(db.config.DefaultDB).Collection(name)
}
