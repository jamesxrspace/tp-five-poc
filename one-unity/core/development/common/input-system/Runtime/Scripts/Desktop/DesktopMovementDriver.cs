using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;

namespace TPFive.Extended.InputSystem.Desktop
{
    /// <summary>
    /// The <see cref="UnityEngine.InputSystem.XR.TrackedPoseDriver"/> component not suitable for desktop platform,
    /// so we create a new component to drive the movement by desktop input.
    /// </summary>
    /// <remarks>
    /// For <see cref="positionInput"/>, <see cref="rotationInput"/> and <see cref="rotationActivateInput"/>,
    /// if an action is directly defined
    /// in the <see cref="InputActionProperty"/>, as opposed to a reference to an action externally defined
    /// in an <see cref="InputActionAsset"/>, the action will automatically be enabled and disabled by this
    /// behavior during <see cref="OnEnable"/> and <see cref="OnDisable"/>. The enabled state for actions
    /// externally defined must be managed externally from this behavior.
    /// </remarks>
    public class DesktopMovementDriver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Updates the Transform properties after these phases of Input System event processing.")]
        private TrackedPoseDriver.UpdateType updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;

        [SerializeField]
        [Tooltip("The target transform to move.")]
        private Transform movementTarget;

        [Header("Position")]
        [SerializeField]
        [Tooltip("The input action to read the position value of a tracked device. Must be a Vector 3 control type.")]
        private InputActionProperty positionInput;

        [SerializeField]
        [Tooltip("The speed to move the target transform.")]
        private float positionSpeed = 0.2f;

        [Header("Rotation")]
        [SerializeField]
        [Tooltip("The input action to read the rotation activate value of a tracked device. Must be a Button action type.")]
        private InputActionProperty rotationActivateInput;

        [SerializeField]
        [Tooltip("The input action to read the rotation value of a tracked device. Must be a Vector 2 control type.")]
        private InputActionProperty rotationInput;

        [SerializeField]
        [Tooltip("The speed to rotate the target transform.")]
        private float rotationSpeed = 0.2f;

        private bool positionPerformed;
        private bool rotationPerformed;
        private Vector3 curtPosition;
        private Vector2 curtRotation;

        private Quaternion targetStartedRotation;
        private Vector2 startedRotation;

        public Transform MovementTarget
        {
            get => movementTarget;
            set => movementTarget = value;
        }

        /// <summary>
        /// This method is called after the Input System has completed an update and processed all pending events
        /// when the type of update is not <see cref="InputUpdateType.BeforeRender"/>.
        /// </summary>
        protected virtual void OnUpdate()
        {
            if (updateType == TrackedPoseDriver.UpdateType.Update ||
                updateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
            {
                PerformUpdate();
            }
        }

        /// <summary>
        /// This method is called after the Input System has completed an update and processed all pending events
        /// when the type of update is <see cref="InputUpdateType.BeforeRender"/>.
        /// </summary>
        protected virtual void OnBeforeRender()
        {
            if (updateType == TrackedPoseDriver.UpdateType.BeforeRender ||
                updateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
            {
                PerformUpdate();
            }
        }

        protected void OnEnable()
        {
            UnityEngine.InputSystem.InputSystem.onAfterUpdate += OnInputSystemUpdate;
            BindActions();
        }

        protected void OnDisable()
        {
            UnityEngine.InputSystem.InputSystem.onAfterUpdate -= OnInputSystemUpdate;
            UnbindActions();

            positionPerformed = false;
            rotationPerformed = false;
        }

        private void BindActions()
        {
            BindPosition();
            BindRotation();
        }

        private void UnbindActions()
        {
            UnbindPosition();
            UnbindRotation();
        }

        private void BindPosition()
        {
            var action = positionInput.action;
            if (action == null)
            {
                return;
            }

            action.started += OnPositionStarted;
            action.performed += OnPositionPerformed;
            action.canceled += OnPositionCanceled;

            if (positionInput.reference == null)
            {
                action.Rename($"{gameObject.name} - TPD - Position");
                action.Enable();
            }
        }

        private void BindRotation()
        {
            var activateAction = rotationActivateInput.action;
            if (activateAction != null)
            {
                activateAction.started += OnRotationStarted;
                activateAction.canceled += OnRotationCanceled;

                if (rotationActivateInput.reference == null)
                {
                    activateAction.Rename($"{gameObject.name} - TPD - Rotation Activate");
                    activateAction.Enable();
                }
            }

            var rotateAction = rotationInput.action;
            if (rotateAction != null)
            {
                rotateAction.performed += OnRotationPerformed;

                if (rotationInput.reference == null)
                {
                    rotateAction.Rename($"{gameObject.name} - TPD - Rotation");
                    rotateAction.Enable();
                }
            }
        }

        private void UnbindPosition()
        {
            var action = positionInput.action;
            if (action == null)
            {
                return;
            }

            if (positionInput.reference == null)
            {
                action.Disable();
            }

            action.started -= OnPositionStarted;
            action.performed -= OnPositionPerformed;
            action.canceled -= OnPositionCanceled;
        }

        private void UnbindRotation()
        {
            var activateAction = rotationActivateInput.action;
            if (activateAction != null)
            {
                if (rotationActivateInput.reference == null)
                {
                    activateAction.Disable();
                }

                activateAction.started -= OnRotationStarted;
                activateAction.canceled -= OnRotationCanceled;
            }

            var rotateAction = rotationInput.action;
            if (rotateAction != null)
            {
                if (rotationInput.reference == null)
                {
                    rotateAction.Disable();
                }

                rotateAction.performed -= OnRotationPerformed;
            }
        }

        private void OnInputSystemUpdate()
        {
            if (InputState.currentUpdateType == InputUpdateType.BeforeRender)
            {
                OnBeforeRender();
            }
            else
            {
                OnUpdate();
            }
        }

        private void PerformUpdate()
        {
            if (movementTarget == null)
            {
                return;
            }

            if (positionPerformed)
            {
                // Apply speed
                Vector3 applyPosition = curtPosition * positionSpeed;

                // Apply position
                movementTarget.localPosition += movementTarget.localRotation * applyPosition;
            }

            if (rotationPerformed)
            {
                // Apply speed
                Vector2 applyRotation = (curtRotation - startedRotation) / Screen.dpi * rotationSpeed;

                // Apply rotation
                var eulerAngles = (targetStartedRotation * Quaternion.Euler(-applyRotation.y, applyRotation.x, 0)).eulerAngles;
                eulerAngles.z = 0;
                movementTarget.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            }
        }

        private void OnPositionStarted(InputAction.CallbackContext context)
        {
            positionPerformed = true;
        }

        private void OnPositionPerformed(InputAction.CallbackContext context)
        {
            curtPosition = context.ReadValue<Vector3>();
        }

        private void OnPositionCanceled(InputAction.CallbackContext context)
        {
            positionPerformed = false;
        }

        private void OnRotationStarted(InputAction.CallbackContext context)
        {
            rotationPerformed = true;
            startedRotation = curtRotation;
            targetStartedRotation = movementTarget != null ? movementTarget.localRotation : Quaternion.identity;
        }

        private void OnRotationPerformed(InputAction.CallbackContext context)
        {
            curtRotation = context.ReadValue<Vector2>();
        }

        private void OnRotationCanceled(InputAction.CallbackContext context)
        {
            rotationPerformed = false;
        }
    }
}