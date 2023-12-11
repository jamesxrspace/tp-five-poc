using System;
using Animancer;
using Microsoft.Extensions.Logging;
using TPFive.Extended.Animancer;
using TPFive.SCG.DisposePattern.Abstractions;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Avatar.Sit
{
    [Dispose]
    public partial class AvatarSitManager : IAvatarSitManager
    {
        private readonly ILogger log;
        private readonly Transform avatarRoot;
        private readonly AnimancerLayer layer;
        private readonly CommonTransitionData defaultTransitionData;

        public AvatarSitManager(
            ILoggerFactory loggerFactory,
            Transform avatarRoot,
            AnimancerLayer layer,
            CommonTransitionData defaultTransitionData)
        {
            this.log = loggerFactory.CreateLogger<AvatarSitManager>();
            this.avatarRoot = avatarRoot;
            this.layer = layer;
            this.defaultTransitionData = defaultTransitionData;
        }

        public event Action OnBeforeSitDown;

        public event Action OnAfterSitDown;

        public event Action OnBeforeStandUp;

        public event Action OnAfterStandUp;

        public void SitDown(CommonTransitionData overrideTransitionData = null, bool instant = false)
        {
            if (avatarRoot == null)
            {
                log.LogError("{Method} Avatar root is null", nameof(SitDown));
                return;
            }

            SitDown(new Pose(avatarRoot.position, avatarRoot.rotation), null, overrideTransitionData, instant);
        }

        public void SitDown(Pose sitPoint, Transform specificTarget = null, CommonTransitionData overrideTransitionData = null, bool instant = false)
        {
            log.LogDebug(
                "{Method}: overrideTransitionData: {OverrideTransitionData}, instant: {Instant}",
                nameof(SitDown),
                overrideTransitionData != null ? overrideTransitionData.name : "<none>",
                instant);

            var targetTransitionData = overrideTransitionData != null ? overrideTransitionData : defaultTransitionData;
            if (targetTransitionData == null)
            {
                log.LogError("{Method} Transition data is null", nameof(SitDown));
                return;
            }

            var target = specificTarget != null ? specificTarget : avatarRoot;
            if (target == null)
            {
                log.LogError("{Method} Avatar root is null", nameof(SitDown));
                return;
            }

            var sitPosition = sitPoint.position + (targetTransitionData.OffsetPosition.HasValue
                ? targetTransitionData.OffsetPosition.Value
                : Vector3.zero);
            var sitEulerAngles = sitPoint.rotation.eulerAngles + (targetTransitionData.OffsetRotation.HasValue
                ? targetTransitionData.OffsetRotation.Value
                : Vector3.zero);
            target.SetPositionAndRotation(sitPosition, Quaternion.Euler(sitEulerAngles));

            OnBeforeSitDown?.Invoke();

            if (instant || !targetTransitionData.OnStartClip.IsValid)
            {
                if (targetTransitionData.OnPerformClip.IsValid)
                {
                    layer.Play(targetTransitionData.OnPerformClip);
                }

                OnAfterSitDown?.Invoke();
                return;
            }

            var state = layer.Play(targetTransitionData.OnStartClip);
            state.Events.OnEnd = () =>
            {
                if (targetTransitionData.OnPerformClip.IsValid)
                {
                    layer.Play(targetTransitionData.OnPerformClip);
                }

                OnAfterSitDown?.Invoke();
            };
        }

        public void StandUp(CommonTransitionData overrideTransitionData = null, bool instant = false)
        {
            log.LogDebug(
                "{Method}: overrideTransitionData: {OverrideTransitionData}, instant: {Instant}",
                nameof(StandUp),
                overrideTransitionData != null ? overrideTransitionData.name : "<none>",
                instant);

            var targetTransitionData = overrideTransitionData != null ? overrideTransitionData : defaultTransitionData;
            if (targetTransitionData == null)
            {
                log.LogError("{Method} Transition data is null", nameof(StandUp));
                return;
            }

            OnBeforeStandUp?.Invoke();

            if (instant || !targetTransitionData.OnEndClip.IsValid)
            {
                layer.StartFade(0, targetTransitionData.LayerFadeDuration);
                OnAfterStandUp?.Invoke();
                return;
            }

            var state = layer.Play(targetTransitionData.OnEndClip);
            state.Events.OnEnd = () =>
            {
                layer.StartFade(0, targetTransitionData.LayerFadeDuration);
                OnAfterStandUp?.Invoke();
            };
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
