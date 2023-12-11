// See https://aka.ms/new-console-template for more information

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var boostrapLogger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Logger = boostrapLogger;

// Build starts
var currentDirectory = System.AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
Log.Logger.Information("Current directory: {CurrentDirectory}", currentDirectory);
var builder = Host.CreateDefaultBuilder();
builder
    .UseContentRoot(currentDirectory)
    .ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        var appSettingsPath = System.IO.Path.Combine(currentDirectory, "appsettings.json");

        // Although embedding sensitive information in the appsettings.json file is not recommended, until better
        // alternatives are available, this approach is used to allow the app to run.
        configurationBuilder.AddJsonFile(appSettingsPath);
    })
    .ConfigureHostConfiguration(configurationBuilder =>
    {

    })
    .ConfigureLogging((_, loggingBuilder) =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddConsole();
    })
    .ConfigureServices((context, collection) =>
    {
        var loggingLevel = (context.Configuration["Logging:LogLevel:Default"] ?? "Information").ToLower();

        // This logger is used throughout the entire app
        var loggerConfig = new LoggerConfiguration()
            .WriteTo.Console();

        if (string.Equals(loggingLevel, "information"))
        {
            loggerConfig.MinimumLevel.Information();
        }
        else if (string.Equals(loggingLevel, "debug"))
        {
            loggerConfig.MinimumLevel.Debug();
        }

        var logger = loggerConfig.CreateLogger();

        // Binding for log related references
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSerilog(logger);
        collection.AddSingleton<ILoggerFactory>(loggerFactory);

        // Binding for application related references
        collection.AddHostedService<TPFive.Fetcher.Console.Application>();
        collection.AddScoped<TPFive.Fetcher.Console.IDownloadService, TPFive.Fetcher.Console.DownloadService>();
    });

using var host = builder.Build();
try
{
    await host.RunAsync();
}
catch (System.Exception e)
{
    Log.Logger.Error("General Exception: {Exception}", e);
}
finally
{
    Log.Logger.Information("Exiting...");
    Log.CloseAndFlush();
}
