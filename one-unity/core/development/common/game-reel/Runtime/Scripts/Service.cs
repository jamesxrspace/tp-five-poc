namespace TPFive.Game.Reel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TPFive.OpenApi.GameServer;
    using TPFive.OpenApi.GameServer.Model;
    using TPFive.SCG.ServiceEco.Abstractions;
    using VContainer;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
    using ReelModel = TPFive.OpenApi.GameServer.Model.Reel;

    [RegisterToContainer]
    public class Service : IService
    {
        private readonly ILogger log;
        private readonly IReelApi reelApi;
        private readonly IAssetApi assetApi;
        private readonly AssetAccessHelper assetAccessHelper;

        [Inject]
        private Service(ILoggerFactory loggerFactory, IReelApi reelApi, IAssetApi assetApi)
        {
            log = loggerFactory.CreateLogger<Service>();
            assetAccessHelper = new AssetAccessHelper(loggerFactory, assetApi);
            this.reelApi = reelApi;
            this.assetApi = assetApi;
        }

        public async UniTask<ReelModel> CreateReel(CreateReelData reelData, CancellationToken cancellationToken = default)
        {
            // 1. get file upload url
            List<string> filePaths = new List<string> { reelData.ThumbnailPath, reelData.VideoPath, reelData.XrsPath };
            var (requestId, presignedUrls) = await assetAccessHelper.GetUploadUrls(reelData.Tags, reelData.Type, reelData.Categories, filePaths, cancellationToken);
            log.LogDebug("{Method}(): get upload urls success? {result}", nameof(CreateReel), !string.IsNullOrEmpty(requestId));

            // 2. upload files
            IEnumerable<bool> uploadResults = await UniTask.WhenAll(filePaths.Select(filePath =>
            {
                var fileName = Path.GetFileName(filePath);
                return assetAccessHelper.UploadAsset(filePath, presignedUrls[fileName], maxRetries: 3);
            }));

            // check upload tasks
            bool uploadFail = uploadResults.Any(result => result == false);
            if (uploadFail)
            {
                log.LogDebug("{Method}(): upload files failed", nameof(CreateReel));
                return null;
            }

            log.LogDebug("{Method}(): all files uploaded", nameof(CreateReel));

            // 3. confirm files upload result
            var checkUploadedResult = await assetApi.ConfirmUploadedAsync(requestId, cancellationToken: cancellationToken);
            if (!checkUploadedResult.IsSuccess)
            {
                log.LogError(
                    "{Method}(): Confirm upload result returns failure. err_code: {code} , err_msg: {msg}",
                    nameof(CreateReel),
                    checkUploadedResult.ErrorCode,
                    checkUploadedResult.Message);

                return null;
            }

            // 4. create reel
            CreateReelRequest requestData = PrepareReelRequest(reelData, checkUploadedResult.Data);
            var response = await reelApi.CreateReelAsync(requestData);
            if (!response.IsSuccess)
            {
                log.LogWarning("{Method}(): create reel failed. err_code: {code} , err_msg: {msg}", nameof(CreateReel), response.ErrorCode, response.Message);
                return null;
            }

            if (response.IsSuccess && response.Data == null)
            {
                throw new Exception("Create reel responded with a success code, but the data is empty.");
            }

            log.LogDebug("{Method}(): create reel success.", nameof(CreateReel));
            return response.Data.Reel;
        }

        public async UniTask<bool> PublishReel(string reelId, CancellationToken cancellationToken = default)
        {
            var response = await reelApi.PublishReelAsync(reelId);
            return response.IsSuccess;
        }

        public async UniTask<bool> DeleteReel(string reelId, CancellationToken cancellationToken = default)
        {
            var response = await reelApi.DeleteReelAsync(reelId);
            return response.IsSuccess;
        }

        private CreateReelRequest PrepareReelRequest(CreateReelData reelData, Dictionary<string, S3Object> responseData)
        {
            return new CreateReelRequest
            {
                Description = reelData.Description,
                Thumbnail = responseData[Path.GetFileName(reelData.ThumbnailPath)].Url,
                Video = responseData[Path.GetFileName(reelData.VideoPath)].Url,
                Xrs = responseData[Path.GetFileName(reelData.XrsPath)].Url,
                MusicToMotionUrl = reelData.MusicToMotionUrl,
                ParentReelId = reelData.ParentReelId,
                Categories = reelData.Categories,
                JoinMode = reelData.JoinMode,
            };
        }
    }
}