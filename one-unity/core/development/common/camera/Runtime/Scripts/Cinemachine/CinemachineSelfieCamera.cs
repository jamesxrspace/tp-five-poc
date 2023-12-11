using TPFive.Extended.InputSystem;
using TPFive.Game.Camera;
using TPFive.Game.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPFive.Extended.Camera
{
    public sealed class CinemachineSelfieCamera : CinemachineCameraBase
    {
        [SerializeField]
        private InputActionBinder rotateInputActionBinder;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The speed of the camera rotation.")]
        private float rotateSpeed = 1f;

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

        private bool isValid;

        private bool rotateHorizontalTargetIsFollowTarget;

        private float startedVerticalAngle;
        private float startedHorizontalAngle;
        private Vector2 curtRotateInput;
        private Transform selfieAvatarTarget;

        private bool allowInteractWithRotateInput = true;

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

        public void SetRotateHorizontalTargetIsFollowTarget()
        {
            rotateHorizontalTargetIsFollowTarget = true;
        }

        public void SetRotateHorizontalTargetIsSelfieAvatarTarget()
        {
            rotateHorizontalTargetIsFollowTarget = false;
        }

        protected override void Awake()
        {
            base.Awake();

            rotateInputActionBinder.SetupCallbacks(OnRotateStarted, OnRotating, OnRotateCanceled);

            isValid = IsValid();
        }

        protected override void OnStateChanged(CameraState state)
        {
            base.OnStateChanged(state);

            RebindRotateInputEventsIfAllow();
        }

        protected override void OnFollowTargetChanged(Transform newFollowTarget)
        {
            base.OnFollowTargetChanged(newFollowTarget);
            base.OnLookAtTargetChanged(newFollowTarget);

            isValid = IsValid();
        }

        protected override void OnLookAtTargetChanged(Transform newLookAtTarget)
        {
            selfieAvatarTarget = newLookAtTarget;

            isValid = IsValid();
        }

        private void RebindRotateInputEventsIfAllow()
        {
            rotateInputActionBinder.IsBind = allowInteractWithRotateInput &&
                                             stateProperty.Value == CameraState.Live;
        }

        private void Update()
        {
            if (!isValid)
            {
                return;
            }

            UpdateRotateIfNeed();
        }

        private void UpdateRotateIfNeed()
        {
            if (curtRotateInput == Vector2.zero)
            {
                return;
            }

            if (Mathf.Abs(curtRotateInput.y) > Mathf.Abs(curtRotateInput.x))
            {
                float nextVerticalAngle = CalcAngle(
                    curtRotateInput.y,
                    rotateSpeed,
                    invertVertical,
                    clampVertical,
                    verticalMinAngle,
                    verticalMaxAngle,
                    startedVerticalAngle);

                Vector3 prevLookAtEulerAngles = followTargetProperty.Value.transform.eulerAngles;
                followTargetProperty.Value.transform.eulerAngles = new Vector3(nextVerticalAngle, prevLookAtEulerAngles.y, 0f);
                return;
            }

            float nextHorizontalAngle = CalcAngle(
                curtRotateInput.x,
                rotateSpeed,
                invertHorizontal,
                clampHorizontal,
                horizontalMinAngle,
                horizontalMaxAngle,
                startedHorizontalAngle);

            var horizontalTarget = rotateHorizontalTargetIsFollowTarget ? followTargetProperty.Value.transform : selfieAvatarTarget;
            Vector3 prevSelfieAvatarTargetEulerAngles = horizontalTarget.eulerAngles;
            horizontalTarget.eulerAngles = new Vector3(prevSelfieAvatarTargetEulerAngles.x, nextHorizontalAngle, 0f);
        }

        private bool IsValid()
        {
            return virtualCamera != null &&
                followTargetProperty.Value != null &&
                lookAtTargetProperty.Value != null &&
                selfieAvatarTarget != null;
        }

        private void OnRotateStarted(InputAction.CallbackContext obj)
        {
            curtRotateInput = Vector2.zero;

            if (followTargetProperty.Value == null || selfieAvatarTarget == null)
            {
                return;
            }

            var followTargetEulerAngles = followTargetProperty.Value.eulerAngles;
            followTargetEulerAngles = EulerAnglesUtility.GetNormalized(followTargetEulerAngles);

            var avatarTargetEulerAngles = selfieAvatarTarget.eulerAngles;
            avatarTargetEulerAngles = EulerAnglesUtility.GetNormalized(avatarTargetEulerAngles);

            startedVerticalAngle = followTargetEulerAngles.x;
            startedHorizontalAngle = rotateHorizontalTargetIsFollowTarget ? followTargetEulerAngles.y : avatarTargetEulerAngles.y;
        }

        private void OnRotating(InputAction.CallbackContext obj)
        {
            curtRotateInput = obj.ReadValue<Vector2>();
        }

        private void OnRotateCanceled(InputAction.CallbackContext obj)
        {
            curtRotateInput = Vector2.zero;
        }
    }
}
