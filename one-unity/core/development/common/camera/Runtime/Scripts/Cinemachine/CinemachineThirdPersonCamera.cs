using Cinemachine;
using Microsoft.Extensions.Logging;
using TPFive.Extended.InputSystem.Extensions;
using TPFive.Game.Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using CameraState = TPFive.Game.Camera.CameraState;
using IInputService = TPFive.Game.Input.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Camera
{
    public sealed class CinemachineThirdPersonCamera : CinemachineCameraBase, ICameraRecenterable
    {
        [SerializeField]
        private InputActionProperty rotateInputAction;

        [Header("Limits")]
        [SerializeField]
        [Range(-180f, 180f)]
        [Tooltip("The minimum value of the x-axis of the virtual camera(CinemachineFreeLook).")]
        private float xAxisRangeMin = -180f;

        [SerializeField]
        [Range(-180f, 180f)]
        [Tooltip("The maximum value of the x-axis of the virtual camera(CinemachineFreeLook).")]
        private float xAxisRangeMax = 180f;

        [SerializeField]
        [Range(0, 360f)]
        [Tooltip("The maximum delta of the x-axis of the virtual camera(CinemachineFreeLook) with one swipe.")]
        private float xAxisOneSwipeMaxDelta = 180f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The minimum value of the y-axis of the virtual camera(CinemachineFreeLook).")]
        private float yAxisRangeMin = 0f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The maximum value of the y-axis of the virtual camera(CinemachineFreeLook).")]
        private float yAxisRangeMax = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The maximum delta of the y-axis of the virtual camera(CinemachineFreeLook) with one swipe.")]
        private float yAxisOneSwipeMaxDelta = 0.5f;

        private bool isValid;
        private CinemachineFreeLook cinemachineFreeLook;

        private ILogger log;
        private IInputService inputService;
        private InputAction realRotateInputAction;

        private float startedXAxis;
        private float startedYAxis;
        private Vector2 curtRotateInput;
        private Vector2 curtDeltaRotateInput;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, IInputService inputService)
        {
            this.log = loggerFactory.CreateLogger<CinemachineThirdPersonCamera>();
            this.inputService = inputService;

            // make sure used original input action instance
            // not the one in addressable cloned.
            if (!this.inputService.TryGetInputAction(rotateInputAction.action.id.ToString(), out realRotateInputAction))
            {
                this.log.LogError(
                    "{Method}: Cannot find input action '{ActionId}'",
                    nameof(Construct),
                    rotateInputAction.action.id.ToString());

                return;
            }

            // force trigger `OnStateChanged` to bind/unbind input events
            OnStateChanged(stateProperty.Value);
        }

        public void Recenter()
        {
            if (!isValid)
            {
                return;
            }

            var cameraForward = transform.forward;
            cameraForward.y = 0f;
            var followForward = followTargetProperty.Value.forward;
            followForward.y = 0f;
            float angle = Vector3.Angle(cameraForward.normalized, followForward.normalized);
            cinemachineFreeLook.m_XAxis.Value = angle;
            cinemachineFreeLook.m_YAxis.Value = 0.5f;
        }

        protected override void Awake()
        {
            base.Awake();

            cinemachineFreeLook = virtualCamera as CinemachineFreeLook;

            isValid = IsValid();
        }

        protected override void OnStateChanged(CameraState state)
        {
            base.OnStateChanged(state);

            if (realRotateInputAction == null)
            {
                return;
            }

            // force unbind events avoid duplicate bind
            realRotateInputAction.UnbindEvents(OnRotateStarted, OnRotating, OnRotateCanceled);

            if (state == CameraState.Live)
            {
                realRotateInputAction.BindEvents(OnRotateStarted, OnRotating, OnRotateCanceled);
            }
        }

        protected override void OnFollowTargetChanged(Transform newFollowTarget)
        {
            base.OnFollowTargetChanged(newFollowTarget);

            isValid = IsValid();

            Recenter();
        }

        protected override void OnLookAtTargetChanged(Transform newLookAtTarget)
        {
            base.OnLookAtTargetChanged(newLookAtTarget);

            isValid = IsValid();

            Recenter();
        }

        private void Update()
        {
            if (!isValid)
            {
                return;
            }

            UpdateRotateIfNeed();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (virtualCamera != null && virtualCamera is not CinemachineFreeLook)
            {
                Debug.LogWarning("The virtual camera should be a 'CinemachineFreeLook'");
                virtualCamera = null;
            }
        }
#endif

        private void UpdateRotateIfNeed()
        {
            if (curtRotateInput == Vector2.zero)
            {
                return;
            }

            if (Mathf.Abs(curtRotateInput.x) > Mathf.Abs(curtRotateInput.y))
            {
                float x = curtDeltaRotateInput.x * xAxisOneSwipeMaxDelta;
                x = Mathf.Clamp(x + startedXAxis, xAxisRangeMin, xAxisRangeMax);
                cinemachineFreeLook.m_XAxis.Value = x;
                curtDeltaRotateInput = Vector2.zero;
            }
            else
            {
                float y = curtRotateInput.y * yAxisOneSwipeMaxDelta;
                y = Mathf.Clamp(y + startedYAxis, yAxisRangeMin, yAxisRangeMax);
                cinemachineFreeLook.m_YAxis.Value = y;
            }
        }

        private bool IsValid()
        {
            return cinemachineFreeLook != null &&
                followTargetProperty.Value != null &&
                lookAtTargetProperty.Value != null;
        }

        private void OnRotateStarted(InputAction.CallbackContext obj)
        {
            curtRotateInput = Vector2.zero;
            curtDeltaRotateInput = Vector2.zero;

            if (cinemachineFreeLook == null)
            {
                return;
            }

            startedXAxis = cinemachineFreeLook.m_XAxis.Value;
            startedYAxis = cinemachineFreeLook.m_YAxis.Value;
        }

        private void OnRotating(InputAction.CallbackContext obj)
        {
            var nextRotateInput = obj.ReadValue<Vector2>();
            curtDeltaRotateInput = nextRotateInput - curtRotateInput;
            curtRotateInput = nextRotateInput;
        }

        private void OnRotateCanceled(InputAction.CallbackContext obj)
        {
            curtRotateInput = Vector2.zero;
        }
    }
}
