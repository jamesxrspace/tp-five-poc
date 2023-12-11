using TPFive.Extended.Camera;
using TPFive.Extended.InputSystem;
using TPFive.Game.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPFive.Game.Record.Entry
{
    public sealed class CinemachineFreeCamera : CinemachineCameraBase
    {
        [SerializeField]
        private InputActionBinder rotateInputActionBinder;

        [SerializeField]
        private InputActionBinder moveInputActionBinder;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The speed of the camera rotation.")]
        private float rotateSpeed = 0.2f;

        [SerializeField]
        [Range(1f, 10f)]
        [Tooltip("The speed of the camera movement.")]
        private float moveSpeed = 1f;

        [Header("Vertical Limits")]
        [SerializeField]
        [Tooltip("Whether clamp the vertical axis of the virtual camera.")]
        private bool clampVertical;

        [SerializeField]
        [Tooltip("Whether invert the vertical input value.")]
        private bool invertVertical;

        [SerializeField]
        [Range(-90f, 0f)]
        [Tooltip("The minimum angle of the x-axis of the virtual camera.")]
        private float verticalMinAngle = -45f;

        [SerializeField]
        [Range(0f, 90f)]
        [Tooltip("The maximum angle of the x-axis of the virtual camera.")]
        private float verticalMaxAngle = 45f;

        [Header("Horizontal Limits")]
        [SerializeField]
        [Tooltip("Whether invert the horizontal input value.")]
        private bool invertHorizontal;

        [SerializeField]
        [Tooltip("Whether clamp the horizontal axis of the virtual camera.")]
        private bool clampHorizontal;

        [SerializeField]
        [Range(-90f, 0f)]
        [Tooltip("The minimum angle of the y-axis of the virtual camera.")]
        private float horizontalMinAngle = -45f;

        [SerializeField]
        [Range(0f, 90f)]
        [Tooltip("The maximum angle of the y-axis of the virtual camera.")]
        private float horizontalMaxAngle = 45f;

        private Vector2 currentRotateInput;
        private Vector2 currentMoveInput;
        private Vector2 startEulerAngle;

        private bool allowInteractWithRotateInput = true;
        private bool allowInteractWithMoveInput = true;

        public override bool AllowInteractWithRotateInput
        {
            get => allowInteractWithRotateInput;
            set
            {
                if (allowInteractWithRotateInput == value)
                {
                    return;
                }

                allowInteractWithRotateInput = value;
                RebindRotateInputEventsIfAllow();
            }
        }

        public override bool AllowInteractWithMoveInput
        {
            get => allowInteractWithMoveInput;
            set
            {
                if (allowInteractWithMoveInput == value)
                {
                    return;
                }

                allowInteractWithMoveInput = value;
                RebindMoveInputEventsIfAllow();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            rotateInputActionBinder.SetupCallbacks(OnRotateStarted, OnRotating, OnRotateCanceled);
            moveInputActionBinder.SetupCallbacks(OnMoveActionStarted, OnMoveActionPerformed, OnMoveActionCanceled);
        }

        protected override void OnStateChanged(CameraState state)
        {
            base.OnStateChanged(state);

            RebindRotateInputEventsIfAllow();
            RebindMoveInputEventsIfAllow();
        }

        private void RebindRotateInputEventsIfAllow()
        {
            rotateInputActionBinder.IsBind = allowInteractWithRotateInput &&
                                             stateProperty.Value == CameraState.Live;
        }

        private void RebindMoveInputEventsIfAllow()
        {
            moveInputActionBinder.IsBind = allowInteractWithMoveInput &&
                                           stateProperty.Value == CameraState.Live;
        }

        private void Update()
        {
            UpdateRotateIfNeed();
            UpdateMoveIfNeed();
        }

        private void UpdateRotateIfNeed()
        {
            if (currentRotateInput == Vector2.zero)
            {
                return;
            }

            float nextVerticalAngle = CalcAngle(
                currentRotateInput.y,
                rotateSpeed,
                invertVertical,
                clampVertical,
                verticalMinAngle,
                verticalMaxAngle,
                startEulerAngle.x);

            float nextHorizontalAngle = CalcAngle(
                currentRotateInput.x,
                rotateSpeed,
                invertHorizontal,
                clampHorizontal,
                horizontalMinAngle,
                horizontalMaxAngle,
                startEulerAngle.y);

            transform.eulerAngles = new Vector3(nextVerticalAngle, nextHorizontalAngle, 0f);
        }

        private void UpdateMoveIfNeed()
        {
            if (currentMoveInput == Vector2.zero)
            {
                return;
            }

            Vector3 moveDirection = new Vector3(
                Time.deltaTime * moveSpeed * currentMoveInput.x,
                0f,
                Time.deltaTime * moveSpeed * currentMoveInput.y);
            Vector3 move = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);
            move.y = 0f;
            transform.position += move;
        }

        private void OnMoveActionStarted(InputAction.CallbackContext context)
        {
            currentMoveInput = Vector2.zero;
        }

        private void OnMoveActionPerformed(InputAction.CallbackContext context)
        {
            currentMoveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveActionCanceled(InputAction.CallbackContext context)
        {
            currentMoveInput = Vector2.zero;
        }

        private void OnRotateStarted(InputAction.CallbackContext context)
        {
            startEulerAngle = transform.eulerAngles;
            currentRotateInput = Vector2.zero;
        }

        private void OnRotating(InputAction.CallbackContext context)
        {
            currentRotateInput = context.ReadValue<Vector2>();
        }

        private void OnRotateCanceled(InputAction.CallbackContext context)
        {
            currentRotateInput = Vector2.zero;
        }
    }
}
