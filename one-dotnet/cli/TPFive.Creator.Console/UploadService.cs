using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TPFive.Creator.Console;

public interface IUploadService
{
    Task UploadFilesAsync(
        IEnumerable<(string, string)> idWithFilePaths,
        CancellationToken cancellationToken = default);

    Task UploadFoldersAsync(
        IEnumerable<(string, string, string, string)> idVersionPlatformFolderPaths,
        CancellationToken cancellationToken = default);
}

public class UploadService : IUploadService
{
    private readonly ILogger<UploadService> _logger;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public UploadService(
        ILogger<UploadService> logger,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment) =>
        (_logger, _configuration, _hostEnvironment) =
            (logger, configuration, hostEnvironment);

    public async Task UploadFilesAsync(
        IEnumerable<(string, string)> idWithFilePaths,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "{Method} - This starts the upload process.",
            nameof(UploadFilesAsync));

        var s3Client = GetS3Client();

        if (s3Client is null)
        {
            _logger.LogError(
                "{Method} s3Client is null",
                nameof(UploadFilesAsync));
            return;
        }

        var bucketName = _configuration["AWS:S3:BucketName"] ?? "xrspace-world-dev";
        var prefixPath = _configuration["AWS:S3:Prefix"] ?? "xrspace/test-addr";
        var subFolder = _configuration["Content:Unitypackages"] ?? "unitypackages";
        prefixPath = $"{prefixPath}/{subFolder}";

        try
        {
            foreach (var (id, filePath) in idWithFilePaths)
            {
                var transferUtility = new TransferUtility(s3Client);

                var request = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = $"{prefixPath}/{id}.unitypackage",
                    FilePath = filePath
                };

                request.UploadProgressEvent += (sender, args) =>
                {
                    _logger.LogDebug("Progress: {PercentDone}", args.PercentDone);
                };

                await transferUtility!.UploadAsync(request, cancellationToken);

                _logger.LogInformation(
                    "{Method} - Done uploading for id: {Id} unitypackagePath: {UnitypackagePath}",
                    nameof(UploadFilesAsync),
                    id,
                    filePath);
            }
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
        }
    }

    public async Task UploadFoldersAsync(
        IEnumerable<(string, string, string, string)> idVersionPlatformFolderPaths,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "{Method} - This starts the upload process.",
            nameof(UploadFoldersAsync));

        var s3Client = GetS3Client();

        if (s3Client is null)
        {
            _logger.LogError(
                "{Method} s3Client is null",
                nameof(UploadFilesAsync));
            return;
        }

        var bucketName = _configuration["AWS:S3:BucketName"] ?? "xrspace-world-dev";
        var prefixPath = _configuration["AWS:S3:Prefix"] ?? "xrspace/test-addr";
        var subFolder = _configuration["Content:Addressables"] ?? "content";
        prefixPath = $"{prefixPath}/{subFolder}";

        try
        {
            foreach (var (id, version, platform, folderPath) in idVersionPlatformFolderPaths)
            {
                var transferUtility = new TransferUtility(s3Client);
                prefixPath = $"{prefixPath}/{id}/{version}/{platform}";

                var request = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = bucketName,
                    KeyPrefix = prefixPath,
                    Directory = folderPath,
                    SearchOption = SearchOption.AllDirectories
                };

                await transferUtility!.UploadDirectoryAsync(request, cancellationToken);
                _logger.LogInformation(
                    "{Method} - Done uploading for addressable: {Id} version: {version} platform: {platform} folderPath: {folderPath}",
                    nameof(UploadFoldersAsync),
                    id,
                    version,
                    platform,
                    folderPath);
            }
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
        }
    }

    private AmazonS3Client? GetS3Client()
    {
        // AmazonS3Config will throw exception if packing dll into exe, catch the exception and return null
        try
        {
            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1,
                ForcePathStyle = true
            };

            var profileName = _configuration["AWS:Config:Profile"] ?? "";
            var chain = new CredentialProfileStoreChain();
            var result = chain.TryGetAWSCredentials(profileName, out var awsCredentials);
            if (!result)
            {
                return default;
            }

            var s3Client = new AmazonS3Client(awsCredentials, s3Config);

            return s3Client;
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
            return default;
        }
    }
}
