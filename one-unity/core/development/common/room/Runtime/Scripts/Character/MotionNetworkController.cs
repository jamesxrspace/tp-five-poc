using System;
using Fusion;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Motion;
using UnityEngine;
using VContainer;

namespace TPFive.Room
{
    public class MotionNetworkController : NetworkBehaviour
    {
        [SerializeField]
        private AvatarLoader avatarLoader;
        [SerializeField]
        private CharacterNetworkController characterNetworkController;
        private IAvatarMotionManager avatarMotionManager;
        private ILogger<MotionNetworkController> logger;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<MotionNetworkController>();
        }

        public void PlayMotion(Guid uid)
        {
            if (avatarMotionManager == null)
            {
                logger.LogWarning($"{nameof(MotionNetworkController)} is not initialized.");
                return;
            }

            RPC_StartMotion(uid);
        }

        public void StopMotion()
        {
            if (avatarMotionManager == null)
            {
                logger.LogWarning($"{nameof(MotionNetworkController)} is not initialized.");
                return;
            }

            RPC_StopMotion();
        }

        protected void Awake()
        {
            if (avatarLoader == null)
            {
                logger.LogError($"{nameof(MotionNetworkController)} initialize failed: can't get AvatarLoader component.");
                return;
            }

            if (avatarLoader.IsDone)
            {
                Initialize();
                return;
            }

            avatarLoader.OnLoaded.AddListener(Initialize);
        }

        protected void OnDestroy()
        {
            if (characterNetworkController != null)
            {
                characterNetworkController.OnLocomotionStart -= OnLocomotionStarted;
            }
        }

        private void Initialize()
        {
            avatarLoader.OnLoaded.RemoveListener(Initialize);

            var provider = GetComponentInChildren<IAvatarContextProvider>();

            if (!provider.IsAlive())
            {
                logger.LogError($"{nameof(MotionNetworkController)} initialize failed: can't find {nameof(IAvatarContextProvider)} component.");
                return;
            }

            avatarMotionManager = provider.MotionManager;

            if (avatarMotionManager == null)
            {
                logger.LogError($"{nameof(MotionNetworkController)} initialize failed: the {nameof(IAvatarMotionManager)} is null.");
            }

            characterNetworkController.OnLocomotionStart += OnLocomotionStarted;
        }

        private void OnLocomotionStarted()
        {
            if (!HasStateAuthority)
            {
                return;
            }

            avatarMotionManager.Stop();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_StartMotion(Guid uid)
        {
            if (avatarMotionManager == null)
            {
                return;
            }

            avatarMotionManager.Play(uid);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_StopMotion()
        {
            if (avatarMotionManager is not { IsPlaying: true })
            {
                return;
            }

            avatarMotionManager.Stop();
        }
    }
}