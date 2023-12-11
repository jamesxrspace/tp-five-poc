using System;
using EasyCharacterMovement;
using Fusion;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using TPFive.Game.Avatar;
using UnityEngine;
using VContainer;

namespace TPFive.Room
{
    public class CharacterNetworkController : NetworkBehaviour
    {
        /// <summary>
        /// Indicates offset of normalized timing in our running animation,
        /// when one leg passes the other at the normalized clip times of 0.0 and 0.5.
        /// </summary>
        [SerializeField]
        private float normalizedRunningCycleOffset = 0.2f;
        [SerializeField]
        private AvatarLoader avatarLoader;
        private ILogger<CharacterNetworkController> logger;
        private Character cachedCharacter;
        private Animator cachedAnimator;
        private ECMAnimatorCommunicator animatorCommunicator;

        public event Action OnLocomotionStart;

        public bool IsInitialized { get; private set; }

        private bool IsServer => Runner == null || Runner.GameMode == GameMode.Server;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<CharacterNetworkController>();
        }

        public override void Spawned()
        {
            avatarLoader.gameObject.SetActive(!IsServer);
        }

        protected void Awake()
        {
            if (avatarLoader == null)
            {
                logger.LogError($"{nameof(CharacterNetworkController)} initialize failed: can't get AvatarLoader component.");
                return;
            }

            if (avatarLoader.IsDone)
            {
                Initialize();
                return;
            }

            avatarLoader.OnLoaded.AddListener(Initialize);
        }

        protected void Update()
        {
            if (IsServer || !IsInitialized)
            {
                return;
            }

            animatorCommunicator.Update(); // Update animator
        }

        protected void OnDestroy()
        {
            if (animatorCommunicator != null)
            {
                animatorCommunicator.LocomotionStarted -= OnLocomotionStarted;
            }
        }

        private void Initialize()
        {
            if (!TryGetComponent(out cachedCharacter))
            {
                logger.LogError($"Avatar initialize failed: can't find Character component.");
                return;
            }

            var provider = GetComponentInChildren<IAvatarContextProvider>();
            if (!provider.IsAlive())
            {
                logger.LogError($"Avatar initialize failed: can't find IAvatarContextProvider.");
                return;
            }

            cachedAnimator = provider.Animator;
            if (animatorCommunicator == null)
            {
                animatorCommunicator = new ECMAnimatorCommunicator(cachedCharacter, transform, cachedAnimator, normalizedRunningCycleOffset);
                animatorCommunicator.LocomotionStarted += OnLocomotionStarted;
            }

            IsInitialized = true;
        }

        private void OnLocomotionStarted()
        {
            if (!HasStateAuthority)
            {
                return;
            }

            OnLocomotionStart?.Invoke();
        }
    }
}
