using Amazon.Runtime.CredentialManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace TPFive.Ugc.Console;

public sealed partial class UploadService
{
    private const string UnityAuthBaseUrl = "https://player-auth.services.api.unity.com";
    private const string UgcBaseUrl = "https://ugc.services.api.unity.com";
    private const string GcpBaseUrl = "https://storage.googleapis.com";

    private const string CompanyTagId = "9b535347-b536-4548-bbba-0106dcab5411";
    private const string GameEngineTagId = "6f244c70-aab2-4755-98e9-42e3386b664d";
    private const string LevelTagId = "35cfe60a-99aa-4d92-8e22-69b0fabe4245";
    private const string SceneObjectTagId = "f4a12f68-babb-4dec-81e6-f412f0992f40";
    private const string ParticleTagId = "607936ab-d509-4280-a13f-fd565889e620";

    private const string AwsProfileName = "content_use";

    private static async Task<SignInResponse.Generated.SignInResponseContent?> SignIn(
        ILogger logger,
        IConfiguration configuration,
        string userName,
        string password,
        string projectId,
        string environmentName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:UnityAuth"] ?? UnityAuthBaseUrl;

            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/v1/authentication/usernamepassword/sign-in", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("ProjectId", projectId);
            request.AddHeader("UnityEnvironment", environmentName);
            var signInData = new SignInRequest.Generated.SignInData
            {
                Username = userName,
                Password = password,
            };

            var body = SignInRequest.Generated.Serialize.ToJson(signInData);
            request.AddStringBody(body, DataFormat.Json);
            var response = await client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful)
            {
                var content = SignInResponse.Generated.SignInResponseContent.FromJson(response.Content);

                return content;
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return default;
    }

    private static async Task<string?> GetEnvironmentId(
        ILogger logger,
        IConfiguration configuration,
        string signInToken,
        string environmentName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:UgcBase"] ?? UgcBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest($"/v1/environments", Method.Get);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {signInToken}");

            var response = await client.ExecuteAsync(request, cancellationToken);
            logger.LogInformation("{Method} - {Response}", nameof(GetEnvironmentId), response.Content);

            if (response.IsSuccessful)
            {
                var gecList = GetCurrentEnvironment.Generated.GetEnvironmentContent.FromJson(response.Content);

                var environmentId =
                    gecList
                        .Where(x => x.Name.Equals(environmentName))
                        .Select(x => x.Id)
                        .FirstOrDefault();

                return environmentId;
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return default;
    }

    private static async Task<GetResponse.Generated.GetResponseContent?> GetContent(
        ILogger logger,
        IConfiguration configuration,
        string projectId,
        string environmentId,
        string signInToken,
        string ugcId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:UgcBase"] ?? UgcBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest($"/v1/projects/{projectId}/environments/{environmentId}/content/{ugcId}", Method.Get);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {signInToken}");

            var response = await client.ExecuteAsync(request, cancellationToken);
            logger.LogInformation("{Method} - {Response}", nameof(GetContent), response.Content);

            if (response.IsSuccessful)
            {
                var grc = GetResponse.Generated.GetResponseContent.FromJson(response.Content);

                return grc;
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return default;
    }

    private static async Task<UploadResponse.Generated.UploadResponse> CreateInitialContent(
        ILogger logger,
        IConfiguration configuration,
        string projectId,
        string environmentId,
        string signInToken,
        Bundle.Generated.BundlDetaileData generatedBdd,
        CancellationToken cancellationToken = default)
    {
        UploadResponse.Generated.UploadResponse uploadResponse = default;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:UgcBase"] ?? UgcBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var convertedJsonContent = Bundle.Generated.Serialize.ToJson(generatedBdd);
            var generatedTags = GetTags(configuration, generatedBdd);

            var client = new RestClient(options);
            var request = new RestRequest($"/v1/projects/{projectId}/environments/{environmentId}/content", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {signInToken}");
            var uploadDetailData = new UploadRequest.Generated.UploadDetailData
            {
                Name = generatedBdd.Title,
                Description = generatedBdd.Description,
                CustomId = generatedBdd.Id,
                Visibility = "private",
                TagIds = generatedTags.ToArray(),
                Metadata = convertedJsonContent,
            };

            var body = UploadRequest.Generated.Serialize.ToJson(uploadDetailData);
            request.AddStringBody(body, DataFormat.Json);
            var response = await client.ExecuteAsync(request);
            logger.LogInformation("{Method} - {Response}", nameof(UploadContentAsync), response.Content);

            if (response.IsSuccessful)
            {
                uploadResponse = UploadResponse.Generated.UploadResponse.FromJson(response.Content);
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return uploadResponse;
    }

    private static async Task UploadContentFile(
        ILogger logger,
        IConfiguration configuration,
        UploadResponse.Generated.UploadResponse uploadResponse,
        string json,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:GcpBase"] ?? GcpBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var requestUrl = uploadResponse.UploadContentUrl.ToString().Replace(baseUrl, string.Empty);

            logger.LogInformation("{Method} - {requestUrl}", nameof(UploadContentFile), requestUrl);

            var client = new RestClient(options);
            var request = new RestRequest(requestUrl, Method.Put);

            request.AddHeader("x-goog-content-length-range", uploadResponse.UploadContentHeaders.XGoogContentLengthRange[0]);
            request.AddHeader("x-goog-meta-processing-version", uploadResponse.UploadContentHeaders.XGoogMetaProcessingVersion[0]);
            request.AddHeader("x-goog-meta-ugc-entity", uploadResponse.UploadContentHeaders.XGoogMetaUgcEntity[0]);
            request.AddHeader("x-goog-meta-content-id", uploadResponse.UploadContentHeaders.XGoogMetaContentId[0]);
            request.AddHeader("x-goog-meta-add-version-id", uploadResponse.UploadContentHeaders.XGoogMetaAddVersionId[0]);
            request.AddHeader("x-goog-meta-asset-type", uploadResponse.UploadContentHeaders.XGoogMetaAssetType[0]);

            request.AddJsonBody(json);

            var response = await client.ExecuteAsync(request, cancellationToken);
            if (response.IsSuccessful)
            {
                logger.LogInformation("{Method} - {Response}", nameof(UploadContentFile), response.Content);
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }
    }

    private static async Task UploadThumbnailFile(
        ILogger logger,
        IConfiguration configuration,
        UploadResponse.Generated.UploadResponse uploadResponse,
        byte[] thumbnailBytes,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:GcpBase"] ?? GcpBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var requestUrl = uploadResponse.UploadThumbnailUrl.ToString().Replace(baseUrl, string.Empty);

            logger.LogInformation("{Method} - {requestUrl}", nameof(UploadThumbnailFile), requestUrl);

            var client = new RestClient(options);
            var request = new RestRequest(requestUrl, Method.Put);

            request.AddHeader("x-goog-content-length-range", uploadResponse.UploadThumbnailHeaders.XGoogContentLengthRange[0]);
            request.AddHeader("x-goog-meta-processing-version", uploadResponse.UploadThumbnailHeaders.XGoogMetaProcessingVersion[0]);
            request.AddHeader("x-goog-meta-ugc-entity", uploadResponse.UploadThumbnailHeaders.XGoogMetaUgcEntity[0]);
            request.AddHeader("x-goog-meta-content-id", uploadResponse.UploadThumbnailHeaders.XGoogMetaContentId[0]);
            request.AddHeader("x-goog-meta-add-version-id", uploadResponse.UploadThumbnailHeaders.XGoogMetaAddVersionId[0]);
            request.AddHeader("x-goog-meta-asset-type", uploadResponse.UploadThumbnailHeaders.XGoogMetaAssetType[0]);

            request.AddBody(thumbnailBytes, ContentType.Binary);

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                logger.LogInformation("{Method} - {Response}", nameof(UploadThumbnailFile), response.Content);
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }
    }

    private static async Task<bool> UpdateAsset(
        ILogger logger,
        IConfiguration configuration,
        string assetPath,
        Bundle.Generated.BundlDetaileData generatedBdd,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug(
            "{Method} - {assetPath}",
            nameof(UpdateAsset),
            assetPath);

        try
        {
            var allLineInAsset = await File.ReadAllLinesAsync(assetPath, cancellationToken);
            for (var i = 0; i < allLineInAsset.Length; ++i)
            {
                var line = allLineInAsset[i];
                if (line.Contains("ugcId:"))
                {
                    line = $"  ugcId: {generatedBdd.UgcId}";
                    allLineInAsset[i] = line;
                }
            }

            await File.WriteAllLinesAsync(assetPath, allLineInAsset, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("{Exception}", e);
            return false;
        }

        return true;
    }

    private static async Task<UpdateResponse.Generated.UpdateResponseContent?> UpdateContent(
        ILogger logger,
        IConfiguration configuration,
        string projectId,
        string environmentId,
        string signInToken,
        Bundle.Generated.BundlDetaileData generatedBdd,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("{Method} - {generatedBdd}", nameof(UpdateContent), generatedBdd);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:UgcBase"] ?? UgcBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest($"/v1/projects/{projectId}/environments/{environmentId}/content/{generatedBdd.UgcId}/details", Method.Put);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {signInToken}");

            var convertedJsonContent = Bundle.Generated.Serialize.ToJson(generatedBdd);
            var updateDetailData = new UpdateRequest.Generated.UpdateDetailData
            {
                Name = generatedBdd.Title,
                Description = generatedBdd.Description,
                CustomId = generatedBdd.Id,
                Visibility = "private",
                Metadata = convertedJsonContent,
                TagsId = GetTags(configuration, generatedBdd).ToList(),
            };

            var body = UpdateRequest.Generated.Serialize.ToJson(updateDetailData);
            request.AddStringBody(body, DataFormat.Json);
            var response = await client.ExecuteAsync(request, cancellationToken);
            logger.LogInformation("{Method} - {Response}", nameof(UpdateContent), response.Content);

            if (response.IsSuccessful)
            {
                var urc = UpdateResponse.Generated.UpdateResponseContent.FromJson(response.Content);

                return urc;
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return default;
    }

    private static async Task<UpdateResponse.Generated.UpdateResponseContent?> UpdateVisibility(
        ILogger logger,
        IConfiguration configuration,
        string projectId,
        string environmentId,
        string signInToken,
        Bundle.Generated.BundlDetaileData generatedBdd,
        string visibility,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("{Method} - {visibility}", nameof(UpdateContent), visibility);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseUrl = configuration["BaseUrl:UgcBase"] ?? UgcBaseUrl;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest($"/v1/projects/{projectId}/environments/{environmentId}/content/{generatedBdd.UgcId}/visibility", Method.Put);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {signInToken}");

            var body = $@"
{{
    ""visibility"": ""{visibility}""
}}
";

            request.AddStringBody(body, DataFormat.Json);
            var response = await client.ExecuteAsync(request, cancellationToken);
            logger.LogInformation("{Method} - {Response}", nameof(UpdateContent), response.Content);

            if (response.IsSuccessful)
            {
                var urc = UpdateResponse.Generated.UpdateResponseContent.FromJson(response.Content);

                return urc;
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return default;
    }

    private static IEnumerable<string> GetTags(
        IConfiguration configuration,
        Bundle.Generated.BundlDetaileData generatedBdd)
    {
        var tagCompany = configuration["TagIds:Company"] ?? CompanyTagId;
        var tagGameEngine = configuration["TagIds:GameEngine"] ?? GameEngineTagId;
        var tagBundleKind = generatedBdd.BundleKind switch
        {
            "Level" => configuration["TagIds:Level"] ?? LevelTagId,
            "SceneObject" => configuration["TagIds:SceneObject"] ?? SceneObjectTagId,
            "Particle" => configuration["TagIds:SceneObject"] ?? ParticleTagId,
            _ => string.Empty,
        };

        var generatedTags = new List<string>
        {
            tagCompany,
            tagGameEngine,
            tagBundleKind,
        };

        return generatedTags;
    }

    private static (string? UserName, string? Password) GetUsernamePassword(
        ILogger logger,
        IConfiguration configuration)
    {
        // Using aws credential as development username and password.
        try
        {
            var profileName = configuration["AWS:Config:Profile"] ?? AwsProfileName;
            var chain = new CredentialProfileStoreChain();
            var result = chain.TryGetAWSCredentials(profileName, out var awsCredentials);

            if (result)
            {
                var credentials = awsCredentials.GetCredentials();
                if (credentials is not null)
                {
                    // Ugc username max to 20 chars and password max to 30 chars. Using portion of aws credential
                    // string to avoid the length limitation.
                    var username = credentials.AccessKey[..10];
                    var password = credentials.SecretKey[..20];

                    return (username, password);
                }
            }
        }
        catch (System.Exception e)
        {
            logger.LogError("{Exception}", e);
        }

        return (default, default);
    }
}
