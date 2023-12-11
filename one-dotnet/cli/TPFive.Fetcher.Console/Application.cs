using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace TPFive.Fetcher.Console;

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
            Task.Run(RegisterCommand(cancellationToken), cancellationToken);
        });

        return Task.CompletedTask;
    }

    private System.Func<Task?> RegisterCommand(CancellationToken cancellationToken = default)
    {
        return async () =>
        {
            try
            {
                _logger.LogInformation("{Method}", nameof(RegisterCommand));
                var rootCommand = new System.CommandLine.RootCommand()
                {
                    TreatUnmatchedTokensAsErrors = false,
                };

                var toWritePipeOption = new System.CommandLine.Option<string>(
                    "--to-write-pipe",
                    "The piep to write to");
                toWritePipeOption.AddAlias("-p");

                var idOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--id",
                    "Specify the id.")
                {
                    AllowMultipleArgumentsPerToken = true,
                };
                idOption.AddAlias("-d");

                var saveToPathOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--save-to-path",
                    "Specify the path for storing file(unitypackage).")
                {
                    AllowMultipleArgumentsPerToken = true,
                };
                saveToPathOption.AddAlias("-f");

                var folderPathOption = new System.CommandLine.Option<string>(
                    "--folder-path",
                    "Specify the parent folder path.")
                {
                    AllowMultipleArgumentsPerToken = true,
                };
                folderPathOption.AddAlias("-o");

                var downloadFileCommand = new System.CommandLine.Command(
                    "download-file", "Download files that contain ids and unitypackages in order.")
                {
                    toWritePipeOption,
                    idOption,
                    saveToPathOption,
                };

                downloadFileCommand.SetHandler(
                    HandleDownloadFileCommand(cancellationToken),
                    toWritePipeOption,
                    idOption,
                    saveToPathOption);

                var downloadOverviewCommand = new System.CommandLine.Command(
                    "download-overview", "Download folders.")
                {
                    folderPathOption,
                };

                downloadOverviewCommand.SetHandler(
                    HandleDownloadOverviewCommand(cancellationToken),
                    folderPathOption);

                rootCommand.AddCommand(downloadFileCommand);
                rootCommand.AddCommand(downloadOverviewCommand);

                var args = System.Environment.GetCommandLineArgs();
                var adjustedArgs = args.Skip(1).ToArray();

                _logger.LogInformation("{AdjustedArgs}", string.Join(" ", adjustedArgs));

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
                                                new FigletText("Fetcher Console")
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
                    nameof(RegisterCommand),
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

    private System.Func<string, IEnumerable<string>, IEnumerable<string>, Task> HandleDownloadFileCommand(
        CancellationToken cancellationToken = default)
    {
        return async (toWritePipe, ids, filePaths) =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<IDownloadService>();

            var idWithFilePaths = ids
                .Zip(filePaths, (id, path) => (id, path))
                .ToList();

            await uploadService!.DownloadFilesAsync(idWithFilePaths, toWritePipe, cancellationToken);
        };
    }

    private System.Func<string, Task> HandleDownloadOverviewCommand(CancellationToken cancellationToken = default)
    {
        return async (folderPath) =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<IDownloadService>();

            await uploadService!.DownloadContentOverviewAsync(folderPath);
        };
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Method}", nameof(StopAsync));
        return Task.CompletedTask;
    }
}
