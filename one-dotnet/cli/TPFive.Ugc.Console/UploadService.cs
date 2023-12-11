using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace TPFive.Ugc.Console;

public interface IUploadService
{
    Task UploadContentAsync(
        string projectId,
        string environmentName,
        string assetPath,
        string jsonContent,
        string thumbnailPath,
        IEnumerable<string> tags,
        CancellationToken cancellationToken = default);

    Task UpdateVisibilityAsync(
        string projectId,
        string environmentName,
        string jsonContent,
        string visibility,
        CancellationToken cancellationToken = default);
}

public sealed partial class UploadService : IUploadService
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

    public async Task UploadContentAsync(
        string projectId,
        string environmentName,
        string assetPath,
        string jsonContent,
        string thumbnailPath,
        IEnumerable<string> tags,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "{Method} - projectId: {projectId} environmentName: {environmentName} jsonContent: {jsonContent}",
            nameof(UploadContentAsync),
            projectId,
            environmentName,
            jsonContent);

        try
        {
            var jsonBytes = Convert.FromBase64String(jsonContent);
            var json = Encoding.UTF8.GetString(jsonBytes);

            // var thumbnailBytes = Convert.FromBase64String(thumbnailContent);
            using var imageStreamSource = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var thumbnailBytes = new byte[imageStreamSource.Length];
            await imageStreamSource.ReadAsync(thumbnailBytes, 0, thumbnailBytes.Length);

            _logger.LogInformation(
                "{Method} - json: {json}",
                nameof(UploadContentAsync),
                json);

            var (signInResponseContent, environmentId) = await GetSignInAndEnvironmentId(
                _logger,
                _configuration,
                projectId,
                environmentName,
                cancellationToken);

            if (signInResponseContent is null || string.IsNullOrEmpty(environmentId))
            {
                _logger.LogError("{Method} Sign in failed or can not find environmentId", nameof(UpdateVisibilityAsync));

                return;
            }

            var generatedBdd = Bundle.Generated.BundlDetaileData.FromJson(json);
            if (!string.IsNullOrEmpty(generatedBdd.UgcId))
            {
                var detailContent = GetContent(
                    _logger,
                    _configuration,
                    projectId,
                    environmentId,
                    signInResponseContent.IdToken,
                    generatedBdd.UgcId,
                    cancellationToken);

                if (detailContent != null)
                {
                    _logger.LogInformation(
                        "{Method} - Has UgcId, update content",
                        nameof(UploadContentAsync));

                    var urc = await UpdateContent(
                        _logger,
                        _configuration,
                        projectId,
                        environmentId,
                        signInResponseContent.IdToken,
                        generatedBdd,
                        cancellationToken);

                    if (urc is null)
                    {
                        _logger.LogError("{Method} Update content failed.", nameof(UploadContentAsync));
                    }

                    return;
                }
            }

            await CreateInitialContent(
                _logger,
                _configuration,
                projectId,
                environmentId,
                signInResponseContent.IdToken,
                generatedBdd,
                json,
                thumbnailBytes,
                assetPath,
                cancellationToken);
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
        }
    }

    private static async Task CreateInitialContent(
        ILogger logger,
        IConfiguration configuration,
        string projectId,
        string environmentId,
        string idToken,
        Bundle.Generated.BundlDetaileData generatedBdd,
        string json,
        byte[] thumbnailBytes,
        string assetPath,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "{Method} - UgcId is null or empty. Create new content.",
            nameof(UploadContentAsync));

        var uploadResponse = await CreateInitialContent(
            logger,
            configuration,
            projectId,
            environmentId,
            idToken,
            generatedBdd,
            cancellationToken);

        await UploadContentFile(logger, configuration, uploadResponse, json, cancellationToken);
        await UploadThumbnailFile(logger, configuration, uploadResponse, thumbnailBytes, cancellationToken);

        generatedBdd.UgcId = uploadResponse.Content.Id.ToString();

        var result = await UpdateAsset(
            logger,
            configuration,
            assetPath,
            generatedBdd,
            cancellationToken);

        if (!result)
        {
            logger.LogError("{Method} Update asset failed.", nameof(CreateInitialContent));
        }
    }

    public async Task UpdateVisibilityAsync(
        string projectId,
        string environmentName,
        string jsonContent,
        string visibility,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonBytes = Convert.FromBase64String(jsonContent);
            var json = Encoding.UTF8.GetString(jsonBytes);

            _logger.LogInformation(
                "{Method} - json: {json}",
                nameof(UploadContentAsync),
                json);

            var (signInResponseContent, environmentId) = await GetSignInAndEnvironmentId(
                _logger,
                _configuration,
                projectId,
                environmentName,
                cancellationToken);

            if (signInResponseContent is null || string.IsNullOrEmpty(environmentId))
            {
                _logger.LogError("{Method} Sign in failed or can not find enviornmentId", nameof(UpdateVisibilityAsync));

                return;
            }

            var generatedBdd = Bundle.Generated.BundlDetaileData.FromJson(json);
            var urc =
                await UpdateVisibility(
                    _logger,
                    _configuration,
                    projectId,
                    environmentId,
                    signInResponseContent.IdToken,
                    generatedBdd,
                    visibility,
                    cancellationToken);
            if (urc is null)
            {
                _logger.LogError("{Method} Update visibility failed.", nameof(UploadContentAsync));
            }
        }
        catch (System.Exception e)
        {
            _logger.LogError("{Exception}", e);
        }
    }

    private static async Task<(SignInResponse.Generated.SignInResponseContent, string?)> GetSignInAndEnvironmentId(
        ILogger logger,
        IConfiguration configuration,
        string projectId,
        string environmentName,
        CancellationToken cancellationToken = default)
    {
        var (userName, password) = GetUsernamePassword(logger, configuration);
        if (userName is null || password is null)
        {
            logger.LogError("{Method} Get username or password failed.", nameof(UploadContentAsync));

            return (default, default);
        }

        var signInResponseContent = await SignIn(
            logger,
            configuration,
            userName,
            password,
            projectId,
            environmentName,
            cancellationToken);
        if (signInResponseContent is null)
        {
            logger.LogError("{Method} Sign in failed.", nameof(UploadContentAsync));

            return (default, default);
        }

        var environmentId = await GetEnvironmentId(
            logger,
            configuration,
            signInResponseContent.IdToken,
            environmentName,
            cancellationToken);

        if (string.IsNullOrEmpty(environmentId))
        {
            logger.LogError("{Method} Get environmentId failed.", nameof(UploadContentAsync));

            return (signInResponseContent, default);
        }

        return (signInResponseContent, environmentId);
    }
}
