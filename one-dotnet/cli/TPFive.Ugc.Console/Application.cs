using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace TPFive.Ugc.Console;

public class Application : IHostedService
{
    private readonly ILogger<Application> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private CancellationTokenSource? _cancellationTokenSource;

    public Application(
        ILogger<Application> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IServiceScopeFactory serviceScopeFactory)
    {
        (_logger, _hostApplicationLifetime, _serviceScopeFactory) =
            (logger, hostApplicationLifetime, serviceScopeFactory);

        _hostApplicationLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogDebug("{Event}", nameof(_hostApplicationLifetime.ApplicationStopping));
        });

        _hostApplicationLifetime.ApplicationStopped.Register(() =>
        {
            _logger.LogDebug("{Event}", nameof(_hostApplicationLifetime.ApplicationStopped));
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Method}", nameof(StartAsync));

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _hostApplicationLifetime.ApplicationStarted.Register(() =>
        {
            _logger.LogDebug("{Event}", nameof(_hostApplicationLifetime.ApplicationStarted));
            Task.Run(Function(cancellationToken), cancellationToken);
        });

        return Task.CompletedTask;
    }

    private System.Func<Task?> Function(CancellationToken cancellationToken = default)
    {
        return async () =>
        {
            try
            {
                _logger.LogInformation("{Method}", nameof(Function));
                var rootCommand = new System.CommandLine.RootCommand()
                {
                    TreatUnmatchedTokensAsErrors = false
                };

                var projectIdOption = new System.CommandLine.Option<string>(
                    "--project-id",
                    "Specify project id.");
                projectIdOption.AddAlias("-p");

                var environmentNameOption = new System.CommandLine.Option<string>(
                    "--env-name",
                    "Specify environment id.");
                environmentNameOption.AddAlias("-e");

                var assetPathOption = new System.CommandLine.Option<string>(
                    "--asset-path",
                    "Specify asset path.");
                assetPathOption.AddAlias("-a");

                var jsonOption = new System.CommandLine.Option<string>(
                    "--json",
                    "Specify json content.");
                jsonOption.AddAlias("-j");

                var thumbnailOption = new System.CommandLine.Option<string>(
                    "--thumbnail",
                    "Specify thumbnail content.");
                jsonOption.AddAlias("-m");

                var visibilityOption = new System.CommandLine.Option<string>(
                    "--thumbnail",
                    "Specify visibility.");
                jsonOption.AddAlias("-v");

                var tagOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--tag",
                    "Using tag as category for now")
                {
                    AllowMultipleArgumentsPerToken = true
                };
                tagOption.AddAlias("-t");

                var uploadContentCommand = new System.CommandLine.Command(
                    "upload-content", "Upload json meta file.")
                {
                    projectIdOption,
                    environmentNameOption,
                    assetPathOption,
                    jsonOption,
                    thumbnailOption,
                    tagOption,
                };

                var updateVisibilityCommand = new System.CommandLine.Command(
                    "update-visibility", "Update content visibility.")
                {
                    projectIdOption,
                    environmentNameOption,
                    jsonOption,
                    visibilityOption,
                };

                uploadContentCommand.SetHandler(
                    HandleUploadContentCommand(cancellationToken),
                    projectIdOption,
                    environmentNameOption,
                    assetPathOption,
                    jsonOption,
                    thumbnailOption,
                    tagOption);

                updateVisibilityCommand.SetHandler(
                    HandleUpdateVisibilityCommand(cancellationToken),
                    projectIdOption,
                    environmentNameOption,
                    jsonOption,
                    visibilityOption);

                // Add sub commands to root command
                rootCommand.AddCommand(uploadContentCommand);
                rootCommand.AddCommand(updateVisibilityCommand);

                // Get command line args
                var args = System.Environment.GetCommandLineArgs();
                var adjustedArgs = args.Skip(1).ToArray();

                _logger.LogInformation("{AdjustedArgs}", string.Join(" ", adjustedArgs));

                // Adjust command line help text
                var parser = new CommandLineBuilder(rootCommand)
                    .UseDefaults()
                    .UseHelp(ctx =>
                    {
                        ctx.HelpBuilder.CustomizeLayout(
                            _ =>
                                HelpBuilder
                                    .Default
                                    .GetLayout()
                                    .Skip(1) // Skip the default command description section.
                                    .Prepend(
                                        _ =>
                                        {
                                            AnsiConsole.Write(
                                                new FigletText("Ugc Console")
                                                    .LeftJustified()
                                                    .Color(Color.Aqua));
                                        }
                                    )
                        );
                    })
                    .Build();

                var exitCode = await parser.InvokeAsync(adjustedArgs);

                _logger.LogInformation(
                    "{Method} exitCode: {exitCode}",
                    nameof(Function),
                    exitCode);
            }
            catch (System.Exception e)
            {
                _logger.LogError("{Exception}", e);
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        };
    }

    private System.Func<string, string, string, string, string, IEnumerable<string>, Task> HandleUploadContentCommand(
        CancellationToken cancellationToken = default)
    {
        return async (projectId, environmentName, assetPath, jsonContent, thumbnailPath, tags) =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<IUploadService>();

            await uploadService!.UploadContentAsync(
                projectId,
                environmentName,
                assetPath,
                jsonContent,
                thumbnailPath,
                tags,
                cancellationToken);
        };
    }

    private System.Func<string, string, string, string, Task> HandleUpdateVisibilityCommand(
        CancellationToken cancellationToken = default)
    {
        return async (projectId, environmentName, jsonContent, visibility) =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<IUploadService>();

            await uploadService!.UpdateVisibilityAsync(
                projectId,
                environmentName,
                jsonContent,
                visibility,
                cancellationToken);
        };
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Method}", nameof(StopAsync));
        return Task.CompletedTask;
    }
}
