using Microsoft.Extensions.Logging;
using TPFive.Extended.InputSystem.Extensions;
using TPFive.Game.UnityEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using VContainer;
using IInputService = TPFive.Game.Input.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Avatar
{
    /// <summary>
    /// Transfers player input to be a <see cref="UnityEngine.Events.UnityEvent"/>
    /// let other components to listen input actions.
    /// </summary>
    public sealed class PlayerInputTransfer : MonoBehaviour
    {
        [Header("Player Input Actions")]
        [SerializeField]
        [Tooltip("The input action to read the movement value of a player. Must be a Vector 2 control type.")]
        private InputActionProperty moveAction;

        [SerializeField]
        [Tooltip("The input action to read the sprint value of a player. Must be a Button control type.")]
        private InputActionProperty sprintAction;

        [SerializeField]
        [Tooltip("The input action to read the jump value of a player. Must be a Button control type.")]
        private InputActionProperty jumpAction;

        [SerializeField]
        [Tooltip("The input action to read the crouch value of a player. Must be a Button control type.")]
        private InputActionProperty crouchAction;

        [Space(10)]
        [Header("Input Action Events")]
        [SerializeField]
        private Vector2UnityEvent onMoveStarted;

        [SerializeField]
        private Vector2UnityEvent onMovePerformed;

        [SerializeField]
        private UnityEvent onMoveCanceled;

        [SerializeField]
        private UnityEvent onSprintStarted;

        [SerializeField]
        private UnityEvent onSprintCanceled;

        [SerializeField]
        private UnityEvent onJumpStarted;

        [SerializeField]
        private UnityEvent onJumpCanceled;

        [SerializeField]
        private UnityEvent onCrouchStarted;

        [SerializeField]
        private UnityEvent onCrouchCanceled;

        private ILogger log;
        private IInputService inputService;
        private InputAction realMoveAction;
        private InputAction realSprintAction;
        private InputAction realJumpAction;
        private InputAction realCrouchAction;

        private bool allowInteractWithInput = true;
        private bool isBindInputEvents;
        private bool isMoveActionStarted;
        private bool isSprintActionStarted;
        private bool isJumpActionStarted;
        private bool isCrouchActionStarted;

        public bool AllowInteractWithInput
        {
            get => allowInteractWithInput;
            set
            {
                if (allowInteractWithInput == value)
                {
                    return;
                }

                allowInteractWithInput = value;
                RefreshBindingInputEvents();
            }
        }

        public Vector2UnityEvent OnMoveStarted => onMoveStarted;

        public Vector2UnityEvent OnMovePerformed => onMovePerformed;

        public UnityEvent OnMoveCanceled => onMoveCanceled;

        public UnityEvent OnSprintStarted => onSprintStarted;

        public UnityEvent OnSprintCanceled => onSprintCanceled;

        public UnityEvent OnJumpStarted => onJumpStarted;

        public UnityEvent OnJumpCanceled => onJumpCanceled;

        public UnityEvent OnCrouchStarted => onCrouchStarted;

        public UnityEvent OnCrouchCanceled => onCrouchCanceled;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, IInputService inputService)
        {
            this.log = loggerFactory.CreateLogger<PlayerInputTransfer>();
            this.inputService = inputService;

            if (!TryGetRealInputAction(moveAction, out realMoveAction) ||
                !TryGetRealInputAction(sprintAction, out realSprintAction) ||
                !TryGetRealInputAction(jumpAction, out realJumpAction) ||
                !TryGetRealInputAction(crouchAction, out realCrouchAction))
            {
                return;
            }

            RefreshBindingInputEvents();

            bool TryGetRealInputAction(InputActionProperty actionProperty, out InputAction inputAction)
            {
                bool result = inputService.TryGetInputAction(actionProperty.action.id.ToString(), out inputAction);
                if (!result)
                {
                    this.log.LogError(
                        "{Method}: Cannot find input action '{ActionId}'",
                        nameof(Construct),
                        actionProperty.action.id.ToString());
                }

                return result;
            }
        }

        private void OnEnable()
        {
            if (inputService == null)
            {
                return;
            }

            RefreshBindingInputEvents();
        }

        private void OnDisable()
        {
            if (inputService == null || !isBindInputEvents)
            {
                return;
            }

            UnbindInputEvents();
        }

        private void RefreshBindingInputEvents()
        {
            bool shouldBind = allowInteractWithInput && enabled;

            if (shouldBind == isBindInputEvents)
            {
                return;
            }

            if (shouldBind)
            {
                BindInputEvents();
                return;
            }

            UnbindInputEvents();
        }

        private void BindInputEvents()
        {
            isBindInputEvents = true;

            realMoveAction.BindEvents(OnMoveActionStarted, OnMoveActionPerformed, OnMoveActionCanceled);
            realSprintAction.BindEvents(OnSprintActionStarted, null, OnSprintActionCanceled);
            realJumpAction.BindEvents(OnJumpActionStarted, null, OnJumpActionCanceled);
            realCrouchAction.BindEvents(OnCrouchActionStarted, null, OnCrouchActionCanceled);
        }

        private void UnbindInputEvents()
        {
            isBindInputEvents = false;

            realMoveAction.UnbindEvents(OnMoveActionStarted, OnMoveActionPerformed, OnMoveActionCanceled);
            realSprintAction.UnbindEvents(OnSprintActionStarted, null, OnSprintActionCanceled);
            realJumpAction.UnbindEvents(OnJumpActionStarted, null, OnJumpActionCanceled);
            realCrouchAction.UnbindEvents(OnCrouchActionStarted, null, OnCrouchActionCanceled);

            if (isMoveActionStarted)
            {
                OnMoveActionCanceled(default);
            }

            if (isSprintActionStarted)
            {
                OnSprintActionCanceled(default);
            }

            if (isJumpActionStarted)
            {
                OnJumpActionCanceled(default);
            }

            if (isCrouchActionStarted)
            {
                OnCrouchActionCanceled(default);
            }
        }

        private void OnMoveActionStarted(InputAction.CallbackContext context)
        {
            isMoveActionStarted = true;
            onMoveStarted.Invoke(context.ReadValue<Vector2>());
        }

        private void OnMoveActionPerformed(InputAction.CallbackContext context)
        {
            if (!isMoveActionStarted)
            {
                return;
            }

            onMovePerformed.Invoke(context.ReadValue<Vector2>());
        }

        private void OnMoveActionCanceled(InputAction.CallbackContext context)
        {
            isMoveActionStarted = false;
            onMoveCanceled.Invoke();
        }

        private void OnSprintActionStarted(InputAction.CallbackContext context)
        {
            isSprintActionStarted = true;
            onSprintStarted.Invoke();
        }

        private void OnSprintActionCanceled(InputAction.CallbackContext context)
        {
            isSprintActionStarted = false;
            onSprintCanceled.Invoke();
        }

        private void OnJumpActionStarted(InputAction.CallbackContext context)
        {
            isJumpActionStarted = true;
            onJumpStarted.Invoke();
        }

        private void OnJumpActionCanceled(InputAction.CallbackContext context)
        {
            isJumpActionStarted = false;
            onJumpCanceled.Invoke();
        }

        private void OnCrouchActionStarted(InputAction.CallbackContext context)
        {
            isCrouchActionStarted = true;
            onCrouchStarted.Invoke();
        }

        private void OnCrouchActionCanceled(InputAction.CallbackContext context)
        {
            isCrouchActionStarted = false;
            onCrouchCanceled.Invoke();
        }
    }
}
