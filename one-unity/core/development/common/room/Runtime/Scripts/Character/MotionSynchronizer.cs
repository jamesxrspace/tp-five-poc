using System;
using Fusion;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Motion;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Room
{
    public class MotionSynchronizer : NetworkBehaviour, IBeforeAllTicks, IAfterAllTicks
    {
        [SerializeField]
        private AvatarLoader avatarLoader;
        private ILogger logger;
        private IAvatarMotionManager avatarMotionManager;

        [Networked]
        private ref MotionNetState NetState => ref MakeRef<MotionNetState>();

        [Inject]
        public void Construct(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<MotionSynchronizer>();
        }

        void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
        {
            if (HasStateAuthority)
            {
                return;
            }

            if (!resimulation)
            {
                return;
            }

            if (avatarMotionManager == null)
            {
                return;
            }

            // If the state or index is different, it will be synchronized.
            if (NetState.IsPlaying != avatarMotionManager.IsPlaying || NetState.Uid != avatarMotionManager.CurrentMotionUid)
            {
                if (NetState.IsPlaying)
                {
                    avatarMotionManager.Play(NetState.Uid);
                    avatarMotionManager.Time = NetState.ProgressTime;
                }
            }

            // If the time difference is greater than 200ms, it will be synchronized.
            // This can also solve the sync time problem when playing the same motion.
            if (Math.Abs(avatarMotionManager.Time - NetState.ProgressTime) > 0.2f)
            {
                avatarMotionManager.Time = NetState.ProgressTime;
            }

            avatarMotionManager.Weight = NetState.Weight;
        }

        void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
        {
            if (!HasStateAuthority)
            {
                return;
            }

            if (avatarMotionManager == null)
            {
                return;
            }

            NetState.Uid = avatarMotionManager.CurrentMotionUid;
            NetState.IsPlaying = avatarMotionManager.IsPlaying;
            NetState.ProgressTime = avatarMotionManager.Time;
            NetState.Weight = avatarMotionManager.Weight;
        }

        protected void Awake()
        {
            if (avatarLoader == null)
            {
                logger.LogError($"{nameof(MotionSynchronizer)} initialize failed: can't get AvatarLoader component.");
                return;
            }

            if (avatarLoader.IsDone)
            {
                OnAvatarInitialized();

                return;
            }

            avatarLoader.OnLoaded.AddListener(OnAvatarInitialized);
        }

        private void OnAvatarInitialized()
        {
            avatarLoader.OnLoaded.RemoveListener(OnAvatarInitialized);

            var provider = GetComponentInChildren<IAvatarContextProvider>();

            if (!provider.IsAlive())
            {
                logger.LogError($"{nameof(MotionSynchronizer)} initialize failed: can't find {nameof(IAvatarContextProvider)} component.");
                return;
            }

            avatarMotionManager = provider.MotionManager;

            if (avatarMotionManager == null)
            {
                logger.LogError($"{nameof(MotionSynchronizer)} initialize failed: the {nameof(IAvatarMotionManager)} is null.");
                return;
            }
        }

        private struct MotionNetState : INetworkStruct
        {
            public Guid Uid;
            public bool IsPlaying;
            public double ProgressTime;
            public float Weight;
        }
    }
}