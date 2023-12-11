using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TPFive.Fetcher.Console;

using TPFive.Fetcher.Generated;

public interface IDownloadService
{
    Task DownloadContentOverviewAsync(
        string folderPath,
        CancellationToken cancellationToken = default);

    Task DownloadFilesAsync(
        IEnumerable<(string Id, string Path)> idWithFilePaths,
        string toWritePipe,
        CancellationToken cancellationToken = default);
}

public class DownloadService : IDownloadService
{
    private readonly ILogger<DownloadService> _logger;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public DownloadService(
        ILogger<DownloadService> logger,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment) =>
        (_logger, _configuration, _hostEnvironment) =
            (logger, configuration, hostEnvironment);

    public async Task DownloadContentOverviewAsync(
        string folderPath,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
                "{Method} - This starts the download process.",
                nameof(DownloadContentOverviewAsync));

        var (s3Client, bucketName, prefixPath) = GetS3RelatedContext();

        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefixPath,
            };

            var contentOverviewData = new ContentOverviewData();
            contentOverviewData.Unitypackages = new List<Unitypackage>();
            ListObjectsV2Response response = null;
            do
            {
                response = await s3Client!.ListObjectsV2Async(request);

                // The first one is the folder itself. Skip it.
                foreach (var s3Object in response.S3Objects.Skip(1))
                {
                    contentOverviewData.Unitypackages.Add(new Unitypackage
                    {
                        Id = Path.GetFileNameWithoutExtension(s3Object.Key),
                        Size = s3Object.Size,
                    });
                }
            }
            while (response.IsTruncated);

            var json = contentOverviewData.ToJson();
            var jsonFilePath = Path.Combine(folderPath, "content-overview.json");
            await File.WriteAllTextAsync(jsonFilePath, json, cancellationToken);
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
        }
    }

    public async Task DownloadFilesAsync(
        IEnumerable<(string, string)> idWithFilePaths,
        string toWritePipe,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "{Method} - This starts the download process.",
            nameof(DownloadFilesAsync));

        var (s3Client, bucketName, prefixPath) = GetS3RelatedContext();

        try
        {
#if WINOS
            await using var pipeWrite = new AnonymousPipeClientStream(PipeDirection.Out, toWritePipe);
            await using var streamWriter = new StreamWriter(pipeWrite);
            streamWriter.AutoFlush = true;
#endif

            foreach (var (id, folderPath) in idWithFilePaths)
            {
                var transferUtility = new TransferUtility(s3Client);
                var adjustedPrefixPath = $"{prefixPath}/{id}.unitypackage";
                var filePath = Path.Combine(folderPath, $"{id}.unitypackage");

                var request = new TransferUtilityDownloadRequest
                {
                        BucketName = bucketName, Key = adjustedPrefixPath, FilePath = filePath,
                };

                request.WriteObjectProgressEvent += async (sender, args) =>
                {
                    _logger.LogDebug("Progress: {PercentDone}", args.PercentDone);
#if WINOS
                    await streamWriter.WriteLineAsync(args.PercentDone.ToString());
#endif
                };

                await transferUtility!.DownloadAsync(request, cancellationToken);

                _logger.LogInformation(
                    "{Method} - Done downloading for id: {Id} unitypackagePath: {UnitypackagePath}",
                    nameof(DownloadService),
                    id,
                    filePath);
            }
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
        }
    }

    private (AmazonS3Client? S3Client, string BucketName, string PrefixPath) GetS3RelatedContext()
    {
        var s3Client = GetS3Client();

        if (s3Client is null)
        {
            _logger.LogError(
                    "{Method} - Can not get s3Client.",
                    nameof(DownloadContentOverviewAsync));

            throw new System.NullReferenceException("Can not get s3Client.");
        }

        var bucketName = _configuration["AWS:S3:BucketName"] ?? "xrspace-world-dev";
        var prefixPath = _configuration["AWS:S3:Prefix"] ?? "xrspace/test-addr";
        var subFolder = _configuration["Content:Unitypackages"] ?? "unitypackages";
        prefixPath = $"{prefixPath}/{subFolder}";

        return (s3Client, bucketName, prefixPath);
    }

    private AmazonS3Client? GetS3Client()
    {
        try
        {
            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1,
                ForcePathStyle = true,
            };

            var profileName = _configuration["AWS:Config:Profile"] ?? string.Empty;
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
