package main

import (
	"context"
	"fmt"
	"os"
	"os/signal"
	"sync"
	"syscall"
	"time"

	"github.com/alecthomas/kong"
	"github.com/getsentry/sentry-go"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/log"
	"go.opentelemetry.io/otel"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/credentials"
	"github.com/aws/aws-sdk-go/aws/session"

	"go.opentelemetry.io/contrib/propagators/b3"

	"xrspace.io/server/core/dependency/auth_service"
	"xrspace.io/server/core/dependency/database"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/eventbus/inmem"
	"xrspace.io/server/core/dependency/gin_engine"
	"xrspace.io/server/core/dependency/index_creator"
	traceLog "xrspace.io/server/core/dependency/log"
	oteltrace "xrspace.io/server/core/dependency/otel/trace"
	"xrspace.io/server/core/dependency/settings"
)

const (
	defaultJobNum = 1
)

// The command-line arguments and flags, parsed by the Go kong package.
type CLI struct {
	// the version flag
	Version kong.VersionFlag `short:"V" name:"version" help:"Show version information and exit."`

	// the customized flags
	Debug  bool   `name:"debug" help:"Enable debug mode (syntax sugar)."`
	Config string `short:"c" name:"config" type:"path" default:"config.local.yaml" help:"The config file path."`
	Action string `short:"a" name:"action" default:"serve" enum:"create-index,serve" help:"The perform action."`
}

func (c *CLI) ParseAndExit(ctx context.Context, wg *sync.WaitGroup, cancelFunc context.CancelFunc) {
	options := []kong.Option{
		kong.Description("The XRSpace Server"),
		kong.Vars{
			"version": "0.0.1",
		},
	}

	// parse the command-line arguments and options
	kong.Parse(c, options...)

	// run the program by the parsed arguments and options
	if err := c.Run(ctx, wg, cancelFunc); err != nil {
		// Fatal will call os.Exit(1) after logging
		log.Fatal().Err(err).Msg("cannot run the command")
	}
}

func (c *CLI) Run(ctx context.Context, wg *sync.WaitGroup, cancelFunc context.CancelFunc) error {
	defer cancelFunc()
	defer wg.Done()
	// init config
	config, err := settings.NewConfig(c.Config)
	if err != nil {
		log.Error().Err(err).Msg("init config error")
		return err
	}

	// setup everything
	c.setupLogger(config)
	c.setupDebugMode()
	c.setupSentry(config)
	c.setupOtel(ctx, config)

	return c.run(ctx, config)
}

func (c *CLI) run(ctx context.Context, config *settings.Config) error {
	defer c.cleanup()
	// init docDB
	docDB, err := docdb.NewDocDB(ctx, &config.DocDB)
	if err != nil {
		log.Error().Err(err).Msg("init docDB error")
		return err
	}

	switch c.Action {
	case "create-index":
		// if create_index is true, then create index
		return index_creator.NewIndexCreator(docDB).CreateIndexes(ctx)
	case "serve":
		// set up auth api proxy
		authAPIProxy, err := auth_service.NewAuthService(&config.OIDC)
		if err != nil {
			log.Error().Err(err).Msg("init auth api proxy error")
			return err
		}

		// init aws awsSession
		awsSession, err := c.setupAWSSession(config)
		if err != nil {
			log.Error().Err(err).Msg("init aws error")
			return err
		}

		// init redis client
		redis, err := database.NewRedis(ctx, &config.Redis)
		if err != nil {
			log.Error().Err(err).Msg("init redis error")
			return err
		}

		eventBus := inmem.NewInMemEventBus()

		// set up gin engine
		ginEngine := gin_engine.NewGinEngine(
			config,
			docDB,
			authAPIProxy,
			awsSession,
			eventBus,
			redis,
		)
		return ginEngine.Serve(ctx)
	default:
		return fmt.Errorf("invalid action: %s", c.Action)
	}
}

func (c *CLI) setupAWSSession(config *settings.Config) (*session.Session, error) {
	switch config.Aws.AccessKey {
	case "":
		// init AWS session with IAM role. In this case load the system's default credential, like
		// ~/.aws/credentials, environment variables, or EC2 instance profile.
		return session.NewSession(
			&aws.Config{
				Region: aws.String(config.Aws.Region),
			},
		)
	default:
		// init AWS session with access key and secret key
		return session.NewSession(
			&aws.Config{
				Credentials:      credentials.NewStaticCredentials(config.Aws.AccessKey, config.Aws.SecretKey, ""),
				Region:           aws.String(config.Aws.Region),
				Endpoint:         aws.String(config.Aws.Endpoint),
				S3ForcePathStyle: aws.Bool(true),
			},
		)
	}
}

func (c *CLI) setupLogger(config *settings.Config) {
	// UNIX Time is faster and smaller than most timestamps
	zerolog.TimeFieldFormat = zerolog.TimeFormatUnix

	logLevels := map[string]zerolog.Level{
		"panic": zerolog.PanicLevel,
		"fatal": zerolog.FatalLevel,
		"error": zerolog.ErrorLevel,
		"warn":  zerolog.WarnLevel,
		"info":  zerolog.InfoLevel,
		"debug": zerolog.DebugLevel,
		"trace": zerolog.TraceLevel,
	}
	level, exists := logLevels[config.Logger.Level]
	if !exists {
		// set the default log level to info
		level = zerolog.InfoLevel
	}

	zerolog.SetGlobalLevel(level)

	zerolog.ErrorStackMarshaler = traceLog.DebugMarshalStack
}

func (c *CLI) setupDebugMode() {
	if c.Debug {
		// enable the log level to trace and show pretty logger
		zerolog.SetGlobalLevel(zerolog.TraceLevel)

		writer := zerolog.ConsoleWriter{Out: os.Stderr}
		log.Logger = zerolog.New(writer).With().Timestamp().Logger()

		log.Debug().Msg("debug mode enabled")
	}
}

func (c *CLI) setupSentry(config *settings.Config) {
	if config.Logger.Sentry.DSN == "" {
		log.Info().Msg("sentry is not configured, skip")
		return
	}

	err := sentry.Init(sentry.ClientOptions{
		Dsn: config.Logger.Sentry.DSN,
		// setup the tracing and sampling rate
		EnableTracing:      config.Logger.Sentry.Sampling > 0.0,
		TracesSampleRate:   config.Logger.Sentry.Sampling,
		ProfilesSampleRate: config.Logger.Sentry.Sampling,
		// Enable printing of SDK debug messages.
		Debug:            false,
		AttachStacktrace: true,
	})

	if err != nil {
		log.Error().Err(err).Msg("init sentry error, but keep server running")
		return
	}

	log.Debug().Msg("sentry is configured")
}

func (c *CLI) setupOtel(ctx context.Context, config *settings.Config) {
	if config.Otel.Address == "" {
		log.Info().Msg("open telemetry is not configured, skip")
		return
	}

	tracerProvider, err := oteltrace.InitProvider(ctx, config.Otel.AppName, config.Otel.Address)
	if err != nil {
		log.Error().Err(err).Msg("init open telemetry error, but keep server running")
		return
	}
	otel.SetTracerProvider(tracerProvider)
	b3Propagator := b3.New(b3.WithInjectEncoding(b3.B3MultipleHeader))
	otel.SetTextMapPropagator(b3Propagator)

	log.Debug().Msg("open telemetry is configured")
}

func (c *CLI) cleanup() {
	// Flush buffered events before the program terminates.
	// Set the timeout to the maximum duration the program can afford to wait.
	sentry.Flush(2 * time.Second)
}

func main() {
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)

	ctx, cancel := context.WithCancel(context.Background())
	cli := CLI{}
	wg := &sync.WaitGroup{}
	wg.Add(defaultJobNum)
	go cli.ParseAndExit(ctx, wg, cancel)
	select {
	case <-quit:
		cancel()
		log.Info().Msg("server starting gracefully shutting down")
	case <-ctx.Done():
	}
	wg.Wait()
}
