using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Video;
using TPFive.SCG.DisposePattern.Abstractions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Video.AVPro
{
    [Dispose]
    public sealed partial class AVProVideoServiceProvider : TPFive.Game.Video.IServiceProvider
    {
        private const string PrefabPath = "Prefabs/VideoPlayer";

        private readonly ILogger _log;
        private readonly IObjectResolver _objectResolver;

        public AVProVideoServiceProvider(
            ILoggerFactory loggerFactory,
            IObjectResolver objectResolver)
        {
            _log = loggerFactory.CreateLogger<AVProVideoServiceProvider>();
            _objectResolver = objectResolver;
        }

        public async UniTask<IVideoPlayer> CreateVideoPlayer(Transform parent = null)
        {
            var videoPlayerPrefab = await Resources.LoadAsync<VideoPlayer>(PrefabPath) as VideoPlayer;
            if (videoPlayerPrefab == null)
            {
                _log.LogWarning("CreateVideoPlayer fail : Load {PrefabPath} is null.", PrefabPath);
                return null;
            }

            // Use VContainer's IObjectSolver.Instantiate for inject dependencies.
            var videoPlayer = _objectResolver.Instantiate(videoPlayerPrefab, parent);

            videoPlayer.name = parent == null ? "VideoPlayer" : parent.name + "_VideoPlayer";

            // Wait for video player each component ready.
            await UniTask.WaitUntil(() => videoPlayer.IsReady);

            return videoPlayer;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
    }
}