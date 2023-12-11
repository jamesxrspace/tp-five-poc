using Cinemachine;
using TPFive.Game.Camera;
using TPFive.Game.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using CameraState = TPFive.Game.Camera.CameraState;

namespace TPFive.Extended.Camera
{
    public class CinemachineCameraBase : MonoCameraBase, ICameraFollowTarget, ICameraLookAtTarget
    {
#pragma warning disable SA1401
        protected readonly ReactiveProperty<Transform> followTargetProperty = new ReactiveProperty<Transform>();

        protected readonly ReactiveProperty<Transform> lookAtTargetProperty = new ReactiveProperty<Transform>();

        protected CinemachinePriorityConfig priorityConfig;

        [SerializeField]
        protected CinemachineVirtualCameraBase virtualCamera;
#pragma warning restore SA1401

        public override Pose Pose
        {
            get
            {
                Assert.IsNotNull(virtualCamera, $"'{nameof(virtualCamera)}' is null");

                var cameraTransform = virtualCamera.transform;

                return new Pose(cameraTransform.position, cameraTransform.rotation);
            }

            set
            {
                Assert.IsNotNull(virtualCamera, $"'{nameof(virtualCamera)}' is null");

                virtualCamera.transform.SetPositionAndRotation(value.position, value.rotation);
            }
        }

        public override bool AllowInteractWithMoveInput { get; set; }

        public override bool AllowInteractWithRotateInput { get; set; }

        public IReactiveProperty<Transform> FollowTarget => followTargetProperty;

        public IReactiveProperty<Transform> LookAtTarget => lookAtTargetProperty;

        [Inject]
        public void Construct(CinemachinePriorityConfig priorityConfig)
        {
            this.priorityConfig = priorityConfig;

            OnStateChanged(State.Value);
        }

        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(virtualCamera, $"'{nameof(virtualCamera)}' is null");

            followTargetProperty.Value = virtualCamera.Follow;
            lookAtTargetProperty.Value = virtualCamera.LookAt;

            followTargetProperty.Subscribe(OnFollowTargetChanged).AddTo(compositeDisposable);
            lookAtTargetProperty.Subscribe(OnLookAtTargetChanged).AddTo(compositeDisposable);
        }

        protected override void OnStateChanged(CameraState state)
        {
            Assert.IsNotNull(virtualCamera, $"'{nameof(virtualCamera)}' is null");

            if (priorityConfig == null)
            {
                // Not injected yet.
                return;
            }

            virtualCamera.Priority = priorityConfig.GetPriority(state);
        }

        protected virtual void OnFollowTargetChanged(Transform newFollowTarget)
        {
            Assert.IsNotNull(virtualCamera, $"'{nameof(virtualCamera)}' is null");

            virtualCamera.Follow = newFollowTarget;
        }

        protected virtual void OnLookAtTargetChanged(Transform newLookAtTarget)
        {
            Assert.IsNotNull(virtualCamera, $"'{nameof(virtualCamera)}' is null");

            virtualCamera.LookAt = newLookAtTarget;
        }

        protected float CalcAngle(
            float input,
            float speed,
            bool invert,
            bool clamp,
            float min,
            float max,
            float startedAngle)
        {
            float deltaAngle = input * 180f * speed;
            if (invert)
            {
                deltaAngle *= -1f;
            }

            float nextAngle = EulerAnglesUtility.GetNormalizeDegree(startedAngle + deltaAngle);

            if (!clamp)
            {
                return nextAngle;
            }

            if (EulerAnglesUtility.IsInRange(nextAngle, min, max))
            {
                return nextAngle;
            }

            if (deltaAngle < 0f)
            {
                return min;
            }

            return max;
        }
    }
}
