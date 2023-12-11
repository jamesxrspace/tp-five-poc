package otel

import (
	"context"
	"fmt"
	"net/url"
	"strings"
	"time"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"

	"go.opentelemetry.io/otel/exporters/otlp/otlptrace"
	"go.opentelemetry.io/otel/exporters/otlp/otlptrace/otlptracegrpc"
	"go.opentelemetry.io/otel/exporters/otlp/otlptrace/otlptracehttp"
	"go.opentelemetry.io/otel/sdk/resource"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
	semconv "go.opentelemetry.io/otel/semconv/v1.12.0"
	"go.opentelemetry.io/otel/trace"
)

const (
	defaultAppName = "backend"
	defaultTimeOut = 1 * time.Second
)

// Initializes an OTLP exporter, and configures the corresponding trace and
// metric providers.
func InitProvider(ctx context.Context, appname string, endpoint string) (trace.TracerProvider, error) {
	if len(appname) == 0 {
		appname = defaultAppName
	}

	res, err := resource.New(ctx,
		resource.WithAttributes(
			// the service name used to display traces in backends
			semconv.ServiceNameKey.String(appname),
		),
	)
	if err != nil {
		return nil, fmt.Errorf("failed to create resource: %w", err)
	}

	ctx, cancel := context.WithTimeout(ctx, defaultTimeOut)
	defer cancel()

	exporter, err := traceExporter(ctx, endpoint)
	if err != nil {
		return nil, fmt.Errorf("failed to create trace exporter: %w", err)
	}

	// Register the trace exporter with a TracerProvider, using a batch
	// span processor to aggregate spans before export.
	bsp := sdktrace.NewBatchSpanProcessor(exporter)
	tracerProvider := sdktrace.NewTracerProvider(
		sdktrace.WithSampler(sdktrace.AlwaysSample()),
		sdktrace.WithResource(res),
		sdktrace.WithSpanProcessor(bsp),
	)

	return tracerProvider, nil
}

func traceExporter(ctx context.Context, endpoint string) (*otlptrace.Exporter, error) {
	ur, err := url.Parse(endpoint)
	if err != nil {
		return nil, err
	}

	var exporter *otlptrace.Exporter

	switch strings.ToLower(ur.Scheme) {
	case "http", "https":
		exporter, err = httpExporter(ctx, ur.Host)
		if err != nil {
			return nil, err
		}

	case "grpc":
		fallthrough
	default:
		exporter, err = grpcExpoter(ctx, ur.Host)
		if err != nil {
			return nil, err
		}
	}

	return exporter, nil
}

func grpcExpoter(ctx context.Context, endpoint string) (*otlptrace.Exporter, error) {
	conn, err := grpc.DialContext(ctx, endpoint,
		grpc.WithTransportCredentials(insecure.NewCredentials()),
		grpc.WithBlock(),
	)

	if err != nil {
		return nil, fmt.Errorf("failed to create gRPC connection to collector: %w", err)
	}

	// Set up a trace exporter
	traceExporter, err := otlptracegrpc.New(
		ctx,
		otlptracegrpc.WithGRPCConn(conn),
	)
	if err != nil {
		return nil, fmt.Errorf("failed to create trace exporter: %w", err)
	}
	return traceExporter, nil
}

func httpExporter(ctx context.Context, endpoint string) (*otlptrace.Exporter, error) {
	opts := []otlptracehttp.Option{
		otlptracehttp.WithTimeout(defaultTimeOut),
		otlptracehttp.WithEndpoint(endpoint),
		otlptracehttp.WithInsecure(),
	}

	trace, err := otlptracehttp.New(ctx, opts...)

	return trace, err
}
