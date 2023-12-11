using System;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    using TPFive.Game.Avatar;
    using UnityEngine.Timeline;

    public class ReelAvatarMotionController
    {
        private readonly IAvatarContextProvider _avatarContextProvider;
        private readonly ILogger _logger;

        public ReelAvatarMotionController(
            ILoggerFactory loggerFactory,
            IAvatarContextProvider avatarContextProvider)
        {
            _avatarContextProvider = avatarContextProvider;
            _logger = loggerFactory.CreateLogger<ReelAvatarMotionController>();
        }

        public void PlayMotion(Guid guid)
        {
            if (_avatarContextProvider == null)
            {
                _logger.LogWarning(
                    "{MethodName} fail : {Interface} is not ready. Guid : {Guid}",
                    nameof(PlayMotion),
                    nameof(IAvatarContextProvider),
                    guid);
                return;
            }

            _avatarContextProvider.MotionManager.Play(guid);
        }

        public void PlayMotion(TimelineAsset timelineAsset)
        {
            if (timelineAsset == null)
            {
                _logger.LogWarning("{MethodName} fail : TimelineAsset is null.", nameof(PlayMotion));
                return;
            }

            if (_avatarContextProvider == null)
            {
                _logger.LogWarning(
                    "{MethodName} fail : {Interface} is not ready. Timeline : {Name}",
                    nameof(PlayMotion),
                    nameof(IAvatarContextProvider),
                    timelineAsset.name);
                return;
            }

            _avatarContextProvider.MotionManager.Play(timelineAsset);
        }
    }
}