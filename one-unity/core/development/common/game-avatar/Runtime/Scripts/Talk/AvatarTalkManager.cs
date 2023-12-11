using Animancer;
using Microsoft.Extensions.Logging;
using TPFive.Extended.Animancer;
using TPFive.SCG.DisposePattern.Abstractions;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Avatar.Talk
{
    [Dispose]
    public partial class AvatarTalkManager : IAvatarTalkManager
    {
        private readonly ILogger log;
        private readonly AnimancerLayer layer;
        private readonly LoopTransitionData transitionData;

        public AvatarTalkManager(
            ILoggerFactory loggerFactory,
            AnimancerLayer layer,
            LoopTransitionData transitionData)
        {
            this.log = loggerFactory.CreateLogger<AvatarTalkManager>();
            this.layer = layer;
            this.transitionData = transitionData;
            this.layer.SetMask(this.transitionData.Mask);
        }

        public void StartTalk()
        {
            log.LogDebug("{Method}: Start", nameof(StartTalk));

            if (transitionData == null)
            {
                log.LogError("{Method}: Transition data is null", nameof(StartTalk));
                return;
            }

            if (!transitionData.OnStartClip.IsValid)
            {
                LoopTalk();
                return;
            }

            var state = layer.Play(transitionData.OnStartClip);
            state.Events.OnEnd = LoopTalk;
        }

        public void StopTalk()
        {
            log.LogDebug("{Method}: Stop", nameof(StopTalk));

            if (transitionData == null)
            {
                log.LogError("{Method}: Transition data is null", nameof(StopTalk));
                return;
            }

            if (!transitionData.OnEndClip.IsValid)
            {
                layer.StartFade(0, transitionData.LayerFadeDuration);
                return;
            }

            var state = layer.Play(transitionData.OnEndClip);
            state.Events.OnEnd = () =>
            {
                layer.StartFade(0, transitionData.LayerFadeDuration);
            };
        }

        private void LoopTalk()
        {
            var clips = transitionData.OnPerformClips;

            int clipLength = clips?.Length ?? 0;
            if (clipLength == 0)
            {
                log.LogError("{Method}: perform clips is empty", nameof(LoopTalk));
                return;
            }

            var clip = clips[Random.Range(0, clips.Length)];

            var state = layer.Play(clip);
            state.Events.OnEnd = LoopTalk;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed state (managed objects).
            }

            _disposed = true;
        }
    }
}
