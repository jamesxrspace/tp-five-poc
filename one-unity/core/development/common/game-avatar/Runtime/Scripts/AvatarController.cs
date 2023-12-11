using Cysharp.Threading.Tasks;
using EasyCharacterMovement;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Extended.Animancer;
using TPFive.Game.Messages;
using UniRx;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Avatar
{
    /// <summary>
    /// control avatar state and behavior.
    /// </summary>
    public sealed class AvatarController : MonoBehaviour
    {
        private readonly ReactiveProperty<bool> isSitDownProperty = new ();
        private readonly UniTaskCompletionSource isReadySource = new ();

        [SerializeField]
        private Transform playerRoot;

        [SerializeField]
        private AvatarLoader avatarLoader;

        [SerializeField]
        private Character character;

        [SerializeField]
        private PlayerInputTransfer playerInputTransfer;

        private ILogger log;
        private IAvatarContextProvider contextProvider;
        private IPublisher<AvatarSitDownMessage> sitDownMsgPublisher;
        private IPublisher<AvatarStandUpMessage> standUpMsgPublisher;

        private bool isInjected;

        public bool IsReady => isReadySource.Task.Status == UniTaskStatus.Succeeded;

        public IReactiveProperty<bool> IsSitDown => isSitDownProperty;

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IPublisher<AvatarSitDownMessage> sitDownMsgPublisher,
            IPublisher<AvatarStandUpMessage> standUpMsgPublisher)
        {
            this.log = loggerFactory.CreateLogger<AvatarController>();
            this.sitDownMsgPublisher = sitDownMsgPublisher;
            this.standUpMsgPublisher = standUpMsgPublisher;

            isInjected = true;

            if (avatarLoader == null ||
                !avatarLoader.IsDone)
            {
                return;
            }

            Initialize();
        }

        public UniTask WaitUntilReadyAsync()
        {
            return isReadySource.Task;
        }

        public void SitDown(
            CommonTransitionData overrideTransitionData = null,
            bool force = false,
            bool instant = false)
        {
            if (playerRoot == null)
            {
                log.LogError("{Method}: player root is null", nameof(SitDown));
                return;
            }

            var sitPoint = new Pose(playerRoot.position, playerRoot.rotation);
            SitDown(sitPoint, overrideTransitionData, force, instant);
        }

        public void SitDown(
            Pose sitPoint,
            CommonTransitionData overrideTransitionData = null,
            bool force = false,
            bool instant = false)
        {
            if (!IsReady)
            {
                log.LogWarning("{Method}: not ready yet", nameof(SitDown));
                return;
            }

            if (!force &&
                isSitDownProperty.Value)
            {
                log.LogWarning("{Method}: already sit down", nameof(SitDown));
                return;
            }

            if (playerRoot == null ||
                !contextProvider.IsAlive() ||
                contextProvider.SitManager == null)
            {
                log.LogError("{Method}: player root, avatar context provider or sit manager is null", nameof(SitDown));
                return;
            }

            log.LogDebug("{Method}: do sit down", nameof(SitDown));

            contextProvider.SitManager.SitDown(sitPoint, playerRoot, overrideTransitionData, instant);

            sitDownMsgPublisher.Publish(new AvatarSitDownMessage(playerRoot.gameObject, sitPoint));
        }

        public void StandUp(
            CommonTransitionData overrideTransitionData = null,
            bool force = false,
            bool instant = false)
        {
            if (playerRoot == null)
            {
                log.LogError("{Method}: player root is null", nameof(StandUp));
                return;
            }

            var standPoint = new Pose(playerRoot.position, playerRoot.rotation);
            StandUp(standPoint, overrideTransitionData, force, instant);
        }

        public void StandUp(
            Pose standPoint,
            CommonTransitionData overrideTransitionData = null,
            bool force = false,
            bool instant = false)
        {
            if (!IsReady)
            {
                log.LogWarning("{Method}: not ready yet", nameof(StandUp));
                return;
            }

            if (!force &&
                !isSitDownProperty.Value)
            {
                log.LogWarning("{Method}: already stand up", nameof(StandUp));
                return;
            }

            if (playerRoot == null ||
                !contextProvider.IsAlive() ||
                contextProvider.SitManager == null)
            {
                log.LogError("{Method}: player root, avatar context provider or sit manager is null", nameof(StandUp));
                return;
            }

            log.LogDebug("{Method}: do stand up", nameof(StandUp));

            playerRoot.SetPositionAndRotation(standPoint.position, standPoint.rotation);

            // force to set movement mode to walking
            // avoid animation play wrong
            if (character != null)
            {
                character.SetMovementMode(MovementMode.Walking);
            }

            contextProvider.SitManager.StandUp(overrideTransitionData, instant);

            var standUpPoint = new Pose(playerRoot.position, playerRoot.rotation);
            standUpMsgPublisher.Publish(new AvatarStandUpMessage(playerRoot.gameObject, standUpPoint));
        }

        private void Awake()
        {
            if (avatarLoader == null)
            {
                log.LogError("{Method}: avatar loader is null", nameof(Awake));
                return;
            }

            if (!avatarLoader.IsDone)
            {
                avatarLoader.OnLoaded.AddListener(Initialize);
                return;
            }

            Initialize();
        }

        private void OnDestroy()
        {
            if (contextProvider.IsAlive() && contextProvider.SitManager != null)
            {
                contextProvider.SitManager.OnBeforeSitDown -= OnBeforeSitDown;
                contextProvider.SitManager.OnAfterStandUp -= OnAfterStandUp;
            }

            if (!IsReady)
            {
                isReadySource.TrySetCanceled();
            }
        }

        private void Initialize()
        {
            if (!isInjected)
            {
                log.LogWarning("{Method}: not inject yet", nameof(Initialize));
                return;
            }

            avatarLoader.OnLoaded.RemoveListener(Initialize);

            contextProvider = GetComponentInChildren<IAvatarContextProvider>();
            if (!contextProvider.IsAlive())
            {
                log.LogError("{Method}: missing 'IAvatarContextProvider' component", nameof(Initialize));
                return;
            }

            if (contextProvider.SitManager == null)
            {
                log.LogError("{Method}: missing 'IAvatarSitManager'", nameof(Initialize));
                return;
            }

            contextProvider.SitManager.OnBeforeSitDown += OnBeforeSitDown;
            contextProvider.SitManager.OnAfterStandUp += OnAfterStandUp;

            TogglePlayerInputTransfer(!isSitDownProperty.Value);

            log.LogInformation("{Method}: Player controller ready", nameof(Initialize));

            isReadySource.TrySetResult();
        }

        private void OnBeforeSitDown()
        {
            log.LogDebug("{Method}: Avatar prepare sit down", nameof(OnBeforeSitDown));

            isSitDownProperty.Value = true;

            // Turn off character controller when sit down
            if (character != null)
            {
                character.enabled = false;
            }

            // Deactivate `PlayerInputTransfer` when sit down
            TogglePlayerInputTransfer(false);
        }

        private void OnAfterStandUp()
        {
            log.LogDebug("{Method}: Avatar already stand up", nameof(OnAfterStandUp));

            isSitDownProperty.Value = false;

            // Turn on character controller when stand up
            if (character != null)
            {
                character.enabled = true;
            }

            // Activate `PlayerInputTransfer` when stand up
            TogglePlayerInputTransfer(true);
        }

        private void TogglePlayerInputTransfer(bool isActive)
        {
            if (playerInputTransfer == null)
            {
                return;
            }

            playerInputTransfer.AllowInteractWithInput = isActive;
        }
    }
}
