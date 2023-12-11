using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using BestHTTP;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MimeTypes;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using UploadFileInfo = TPFive.OpenApi.GameServer.Model.UploadFile;

namespace TPFive.Game.Reel
{
    public class AssetAccessHelper
    {
        private readonly ILogger log;
        private readonly IAssetApi assetApi;

        public AssetAccessHelper(ILoggerFactory loggerFactory, IAssetApi assetApi)
        {
            log = loggerFactory.CreateLogger<AssetAccessHelper>();
            this.assetApi = assetApi;
        }

        public async UniTask<(string requestId, string presignedUrl)> GetUploadUrl(
            List<string> tags,
            string type,
            List<CategoriesEnum> categories,
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var (requestId, presignedUrls) = await GetUploadUrls(
                tags,
                type,
                categories,
                new List<string> { filePath },
                cancellationToken);
            return (requestId, presignedUrls?.GetValueOrDefault(Path.GetFileName(filePath)));
        }

        public async UniTask<(string requestId, Dictionary<string, string> presignedUrls)> GetUploadUrls(
            List<string> tags,
            string type,
            List<CategoriesEnum> categories,
            List<string> filePaths,
            CancellationToken cancellationToken = default)
        {
            var createUploadRequest = new CreateUploadRequest
            {
                Tags = tags,
                Type = type,
                Categories = categories,
                Files = filePaths.Select(PrepareFileInfo).ToList(),
            };

            var response = await assetApi.CreateUploadRequestAsync(
                createUploadRequest,
                cancellationToken: cancellationToken);
            if (!response.IsSuccess)
            {
                return (null, null);
            }

            return (response.Data.RequestId, response.Data.PresignedUrls);
        }

        public async UniTask<bool> UploadAsset(string filePath, string uploadUrl, int maxRetries = default)
        {
            log.LogDebug($"{nameof(UploadAsset)}(): prepare upload file: {filePath} , url: {uploadUrl}");
            try
            {
                string fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
                var httpRequest = new HTTPRequest(new Uri(uploadUrl), methodType: HTTPMethods.Put)
                {
                    UploadStream = new FileStream(filePath, FileMode.Open),
                    DisableCache = true,
                    MaxRetries = maxRetries,
                };
                httpRequest.SetHeader("Content-Type", MimeTypeMap.GetMimeType(fileExtension));

                await httpRequest.GetHTTPResponseAsync();
                log.LogDebug($"{nameof(UploadAsset)}(): Upload asset done");

                return true;
            }
            catch (AsyncHTTPException ae)
            {
                log.LogError(
                    $"{nameof(UploadAsset)}(): AsyncHTTPException status_code: {ae.StatusCode} , msg: {ae.Message} , content: {ae.Content}");
                return false;
            }
            catch (Exception e)
            {
                log.LogError($"{nameof(UploadAsset)}(): exception: {e}");
                return false;
            }
        }

        public async UniTask<string> GetDownloadUrl(
            string requestId,
            string fileId,
            CancellationToken cancellationToken = default)
        {
            var downloadUrls = await GetDownloadUrls(requestId, cancellationToken);
            return downloadUrls?.GetValueOrDefault(fileId);
        }

        public async UniTask<Dictionary<string, string>> GetDownloadUrls(
            string requestId,
            CancellationToken cancellationToken = default)
        {
            var checkUploadedResponse = await assetApi.ConfirmUploadedAsync(
                requestId,
                cancellationToken: cancellationToken);
            if (!checkUploadedResponse.IsSuccess)
            {
                log.LogError(
                    $"{nameof(GetDownloadUrl)}(): Confirm upload result returns failure. err_code: {checkUploadedResponse.ErrorCode} , err_msg: {checkUploadedResponse.Message}");
                return null;
            }

            log.LogDebug(
                $"{nameof(GetDownloadUrl)}(): Confirm file upload success. response: {checkUploadedResponse.Data}");

            var downloadUrls = checkUploadedResponse.Data.ToDictionary(
                x => x.Key,
                x => x.Value.Url);

            return downloadUrls;
        }

        public async UniTask<byte[]> Download(
            string fromUrl,
            int maxRetries,
            int connectionTimeoutSeconds,
            int requestTimeoutSeconds,
            IProgress<float> progressCallback,
            CancellationToken cancellationToken)
        {
            var uri = new Uri(fromUrl, UriKind.Absolute);

            byte[] data;

            // download data from fromUrl
            try
            {
                var request = new HTTPRequest(uri)
                {
                    ConnectTimeout = TimeSpan.FromSeconds(connectionTimeoutSeconds),
                    Timeout = TimeSpan.FromSeconds(requestTimeoutSeconds),
                    MaxRetries = maxRetries,
                };

                if (progressCallback != null)
                {
                    request.OnDownloadProgress = (req, downloaded, length) =>
                    {
                        if (length <= 0)
                        {
                            log.LogWarning(
                                $"{nameof(Download)} OnDownloadProgress(): length <= 0, length: {length}");
                            return;
                        }

                        progressCallback.Report((float)downloaded / length);
                    };
                }

                var response = await request.GetHTTPResponseAsync(cancellationToken);
                data = response.Data;

                if (data == null || data.Length == 0)
                {
                    log.LogWarning($"{nameof(Download)} Downloaded data is empty.");
                    return Array.Empty<byte>();
                }

                return data;
            }
            catch (AsyncHTTPException e)
            {
                log.LogWarning(
                    $"{nameof(Download)} AsyncHTTPException status_code: {e.StatusCode} , msg: {e.Message} , content: {e.Content}");
                return Array.Empty<byte>();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                log.LogWarning($"{nameof(Download)} download data from fromUrl Exception {e}");
                return Array.Empty<byte>();
            }
        }

        private UploadFileInfo PrepareFileInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                log.LogError($"{nameof(PrepareFileInfo)}(): failed, file({filePath}) not found.");
                throw new Exception($"file({filePath}) not found.");
            }

            using var fileStream = File.OpenRead(filePath);
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(fileStream);
            string base64Hash = Convert.ToBase64String(hashBytes);
            string fileName = Path.GetFileName(filePath);
            string fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            log.LogDebug($"{nameof(PrepareFileInfo)}(): file_path: {filePath} , ext: {fileExtension}");

            var fileInfo = new UploadFileInfo
            {
                FileId = fileName,
                Checksum = base64Hash,
                ContentLength = (int)fileStream.Length,
                ContentType = MimeTypeMap.GetMimeType(fileExtension),
            };

            log.LogDebug($"{nameof(PrepareFileInfo)}(): asset file: {fileInfo}");
            return fileInfo;
        }
    }
}