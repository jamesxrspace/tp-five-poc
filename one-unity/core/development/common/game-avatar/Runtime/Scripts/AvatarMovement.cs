using EasyCharacterMovement;
using Microsoft.Extensions.Logging;
using TPFive.Game.Utils;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Avatar
{
    public sealed class AvatarMovement : MonoBehaviour
    {
        /// <summary>
        /// Indicates offset of normalized timing in our running animation when one leg passes the other at the normalized clip times of 0.0 and 0.5.
        /// </summary>
        [SerializeField]
        private float normalizedRunningCycleOffset = 0.2f;

        [SerializeField]
        private Character character;

        [SerializeField]
        private AvatarLoader avatarLoader;

        [SerializeField]
        private PlayerInputTransfer playerInputTransfer;

        [SerializeField]
        private bool canRun = true;

        [Inject]
        private ILoggerFactory loggerFactory;

        private bool isInited;

        private ECMAnimatorCommunicator animatorCommunicator;

        private Transform mainCameraTransform;

        private ILogger log;

        public bool CanRun
        {
            get => canRun;
            set
            {
                if (canRun == value)
                {
                    return;
                }

                canRun = value;
                if (animatorCommunicator != null)
                {
                    animatorCommunicator.CanRun = value;
                }
            }
        }

        private ILogger Log => log ??= loggerFactory.CreateLogger<AvatarMovement>();

        private void Awake()
        {
            if (playerInputTransfer == null)
            {
                playerInputTransfer = GetComponent<PlayerInputTransfer>();
            }

            if (CameraCache.Main != null)
            {
                mainCameraTransform = CameraCache.Main.transform;
            }
        }

        private void Start()
        {
            if (avatarLoader == null)
            {
                Log.LogError(
                    "{Method}: `AvatarMovement` initialize failed. Missing `AvatarLoader` component",
                    nameof(Start));
                return;
            }

            if (!avatarLoader.IsDone)
            {
                avatarLoader.OnLoaded.AddListener(OnAvatarLoaded);
                return;
            }

            OnAvatarLoaded();
        }

        private void OnEnable()
        {
            if (!isInited)
            {
                return;
            }

            BindPlayerInputs();
        }

        private void OnDisable()
        {
            if (!isInited)
            {
                return;
            }

            UnbindPlayerInputs();
        }

        private void Update()
        {
            if (!isInited)
            {
                return;
            }

            animatorCommunicator.Update();
        }

        private void OnAvatarLoaded()
        {
            var provider = GetComponentInChildren<IAvatarContextProvider>();
            if (!provider.IsAlive())
            {
                Log.LogError(
                    "{Method}: `AvatarMovement` initialize failed. can't find `IAvatarContextProvider`",
                    nameof(OnAvatarLoaded));
                return;
            }

            animatorCommunicator = new ECMAnimatorCommunicator(
                character,
                transform,
                provider.Animator,
                normalizedRunningCycleOffset);

            animatorCommunicator.CanRun = canRun;

            BindPlayerInputs();

            isInited = true;
        }

        private void BindPlayerInputs()
        {
            if (playerInputTransfer == null)
            {
                Log.LogWarning(
                    "{Method}: Binding player input failed. `PlayerInputTransfer` is null",
                    nameof(BindPlayerInputs));
                return;
            }

            playerInputTransfer.OnMoveStarted.AddListener(OnMoveStarted);
            playerInputTransfer.OnMovePerformed.AddListener(OnMovePerformed);
            playerInputTransfer.OnMoveCanceled.AddListener(OnMoveCanceled);
            playerInputTransfer.OnJumpStarted.AddListener(OnJumpStarted);
            playerInputTransfer.OnJumpCanceled.AddListener(OnJumpCanceled);
            playerInputTransfer.OnCrouchStarted.AddListener(OnCrouchStarted);
            playerInputTransfer.OnCrouchCanceled.AddListener(OnCrouchCanceled);
        }

        private void UnbindPlayerInputs()
        {
            if (playerInputTransfer == null)
            {
                Log.LogWarning(
                    "{Method}: Unbinding player input failed. `PlayerInputTransfer` is null",
                    nameof(UnbindPlayerInputs));
                return;
            }

            playerInputTransfer.OnMoveStarted.RemoveListener(OnMoveStarted);
            playerInputTransfer.OnMovePerformed.RemoveListener(OnMovePerformed);
            playerInputTransfer.OnMoveCanceled.RemoveListener(OnMoveCanceled);
            playerInputTransfer.OnJumpStarted.RemoveListener(OnJumpStarted);
            playerInputTransfer.OnJumpCanceled.RemoveListener(OnJumpCanceled);
            playerInputTransfer.OnCrouchStarted.RemoveListener(OnCrouchStarted);
            playerInputTransfer.OnCrouchCanceled.RemoveListener(OnCrouchCanceled);
        }

        private void OnMoveStarted(Vector2 moveInput)
        {
            OnMove(moveInput);
        }

        private void OnMovePerformed(Vector2 moveInput)
        {
            OnMove(moveInput);
        }

        private void OnMoveCanceled()
        {
            OnMove(Vector2.zero);
        }

        private void OnMove(Vector2 moveInput)
        {
            if (character == null)
            {
                return;
            }

            if (!canRun)
            {
                moveInput = Vector2.ClampMagnitude(moveInput, AvatarConfig.MaxWalkVelocity);
            }

            // Add movement input in world space
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

            // If Camera is assigned, add input movement relative to camera look direction
            if (mainCameraTransform != null)
            {
                moveDir = (moveDir.x * mainCameraTransform.right) + (moveDir.z * mainCameraTransform.forward);
                moveDir.y = 0f;
            }

            character.SetMovementDirection(moveDir);
        }

        private void OnJumpStarted()
        {
            OnJump(true);
        }

        private void OnJumpCanceled()
        {
            OnJump(false);
        }

        private void OnJump(bool jumpInput)
        {
            if (character == null)
            {
                return;
            }

            if (jumpInput)
            {
                character.Jump();
                return;
            }

            character.StopJumping();
        }

        private void OnCrouchStarted()
        {
            OnCrouch(true);
        }

        private void OnCrouchCanceled()
        {
            OnCrouch(false);
        }

        private void OnCrouch(bool crouchInput)
        {
            if (character == null)
            {
                return;
            }

            if (crouchInput)
            {
                character.Crouch();
                return;
            }

            character.StopCrouching();
        }
    }
}
