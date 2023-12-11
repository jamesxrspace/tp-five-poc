using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Reel;
using TPFive.OpenApi.GameServer.Model;

namespace TPFive.Game.Record.Entry
{
    public class MusicToMotionGenerator
    {
        private readonly ILogger log;
        private readonly AssetAccessHelper assetAccessHelper;
        private readonly MusicToMotionService musicToMotionService;

        public MusicToMotionGenerator(
            ILoggerFactory loggerFactory,
            AssetAccessHelper assetApiHelper,
            MusicToMotionService musicToMotionService)
        {
            this.assetAccessHelper = assetApiHelper;
            this.musicToMotionService = musicToMotionService;
            this.log = loggerFactory.CreateLogger<MusicToMotionGenerator>();
        }

        public UniTask<byte[][]> GenerateMotionFromUrl(string url)
        {
            EnsureExtension(url, ".wav");

            var uri = new Uri(url);
            var path = uri.AbsoluteUri.Replace($"{uri.Scheme}://", string.Empty);
            return musicToMotionService.GenerateMotion(path);
        }

        public UniTask<byte[][]> GenerateMotionFromPath(string path)
        {
            EnsureExtension(path, ".wav");

            return musicToMotionService.GenerateMotion(path);
        }

        public async UniTask<byte[][]> GenerateMotionFromFile(string filePath, CancellationToken cancellationToken)
        {
            EnsureExtension(filePath, ".wav");

            try
            {
                log.LogDebug($"GenerateMotionFromFile(): Uploading audio...");

                var (requestId, presignedUrl) = await assetAccessHelper.GetUploadUrl(null, "Audio", new List<CategoriesEnum> { CategoriesEnum.Music }, filePath, cancellationToken);
                var success = await assetAccessHelper.UploadAsset(filePath, presignedUrl);
                if (!success)
                {
                    throw new Exception("Upload failed.");
                }

                var audioUrl = await assetAccessHelper.GetDownloadUrl(requestId, Path.GetFileName(filePath), cancellationToken);
                log.LogDebug($"GenerateMotionFromFile(): Retrieve audio url.");
                log.LogDebug($"GenerateMotionFromFile(): Generating...");
                return await musicToMotionService.GenerateMotion(audioUrl);
            }
            catch (OperationCanceledException)
            {
                log.LogInformation($"GenerateMotionFromFile(): canceled.");
                return null;
            }
            catch (Exception e)
            {
                log.LogError($"GenerateMotionFromFile(): {e}");
                throw;
            }
        }

        private static void EnsureExtension(string url, string extension)
        {
            if (!Path.GetExtension(url).Equals(extension, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Only \"{extension}\" file is supported.");
            }
        }
    }
}
