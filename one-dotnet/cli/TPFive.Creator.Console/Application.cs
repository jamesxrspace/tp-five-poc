using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace TPFive.Creator.Console;

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

                // Keep the pipe usage till there is a better way to handle this inside Unity.
                var toWritePipeOption = new System.CommandLine.Option<string>(
                    "--to-write-pipe",
                    "The piep to write to");
                toWritePipeOption.AddAlias("-p");

                var idOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--id",
                    "Specify the id.")
                {
                    AllowMultipleArgumentsPerToken = true
                };
                idOption.AddAlias("-d");

                var versionOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--version",
                    "Specify the version.")
                {
                    AllowMultipleArgumentsPerToken = true
                };
                versionOption.AddAlias("-v");

                var platformOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--platform",
                    "Specify the version.")
                {
                    AllowMultipleArgumentsPerToken = true
                };
                versionOption.AddAlias("-t");

                var filePathOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--file-path",
                    "Specify the path for file(unitypackage).")
                {
                    AllowMultipleArgumentsPerToken = true
                };
                filePathOption.AddAlias("-f");

                var folderPathOption = new System.CommandLine.Option<IEnumerable<string>>(
                    "--folder-path",
                    "Specify the path for unitypackage.")
                {
                    AllowMultipleArgumentsPerToken = true
                };
                folderPathOption.AddAlias("-o");

                var uploadFileCommand = new System.CommandLine.Command(
                    "upload-file", "Upload files that contain ids and unitypackages in order.")
                {
                    // toWritePipeOption,
                    idOption,
                    filePathOption
                };

                // Upload file command
                uploadFileCommand.SetHandler(
                    HandleUploadFileCommand(cancellationToken),
                    // toWritePipeOption,
                    idOption,
                    filePathOption);

                var uploadFolderCommand = new System.CommandLine.Command(
                    "upload-folder", "Upload folders.")
                {
                    // toWritePipeOption,
                    idOption,
                    versionOption,
                    platformOption,
                    folderPathOption
                };

                // Upload folder command
                uploadFolderCommand.SetHandler(
                    HandleUploadFolderCommand(cancellationToken),
                    // toWritePipeOption,
                    idOption,
                    versionOption,
                    platformOption,
                    folderPathOption);

                // Add sub commands to root command
                rootCommand.AddCommand(uploadFileCommand);
                rootCommand.AddCommand(uploadFolderCommand);

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
                                                new FigletText("Creator Console")
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

    private System.Func<IEnumerable<string>, IEnumerable<string>, Task> HandleUploadFileCommand(
        CancellationToken cancellationToken = default)
    {
        return async (ids, filePaths) =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<IUploadService>();

            var idWithFilePaths = ids
                .Zip(filePaths, (id, path) => (id, path))
                .ToList();

            await uploadService!.UploadFilesAsync(idWithFilePaths, cancellationToken);
        };
    }

    private System.Func<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, Task>
        HandleUploadFolderCommand(CancellationToken cancellationToken = default)
    {
        return async (ids, versions, platforms, folderPaths) =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<IUploadService>();

            var idWithVersions = ids
                .Zip(versions, (id, version) => (id, version));

            var idVersionPlatforms = idWithVersions
                .Zip(platforms, (idVersion, platform) => (idVersion.id, idVersion.version, platform));

            var idVersionPlatformFolderPaths = idVersionPlatforms
                .Zip(folderPaths, (idVersionPlatform, folderPath) =>
                    (idVersionPlatform.id, idVersionPlatform.version, idVersionPlatform.platform, folderPath));

            await uploadService!.UploadFoldersAsync(idVersionPlatformFolderPaths.ToList(), cancellationToken);
        };
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Method}", nameof(StopAsync));
        return Task.CompletedTask;
    }
}
