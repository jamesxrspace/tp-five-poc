using TPFive.Game;
using TPFive.Game.Camera;
using UniRx;
using UnityEngine;

namespace TPFive.Extended.Camera
{
    public abstract class MonoCameraBase : MonoBehaviour, ICamera
    {
#pragma warning disable SA1401
        protected readonly ReactiveProperty<CameraState> stateProperty = new ReactiveProperty<CameraState>(CameraState.Standby);

        protected readonly ReactiveProperty<int?> cullingMaskOverrideProperty = new ReactiveProperty<int?>(null);

        protected CompositeDisposable compositeDisposable = new CompositeDisposable();

        protected Transform transformCache;
#pragma warning restore SA1401

        [SerializeField]
        [Tooltip("Custom name for this camera. If not set, the name of the GameObject will be used.")]
        private Optional<string> customName;

        [SerializeField]
        private Optional<LayerMask> cullingMaskOverride;

        public IReactiveProperty<CameraState> State => stateProperty;

        public IReadOnlyReactiveProperty<int?> CullingMaskOverride => cullingMaskOverrideProperty;

        public abstract bool AllowInteractWithMoveInput { get; set; }

        public abstract bool AllowInteractWithRotateInput { get; set; }

        public virtual string Name => customName.HasValue ? customName.Value : gameObject.name;

        public virtual Pose Pose
        {
            get => new Pose(transformCache.position, transformCache.rotation);
            set => transformCache.SetPositionAndRotation(value.position, value.rotation);
        }

        public virtual void ResetToStartingPose()
        {
        }

        protected abstract void OnStateChanged(CameraState state);

        protected virtual void Awake()
        {
            transformCache = transform;

            stateProperty.Subscribe(OnStateChanged).AddTo(compositeDisposable);

            if (cullingMaskOverride.HasValue)
            {
                cullingMaskOverrideProperty.Value = cullingMaskOverride.Value;
            }
        }

        protected virtual void Start()
        {
            // Nothing to do yet. But we should keep this method for future.
        }

        protected virtual void OnDestroy()
        {
            compositeDisposable?.Dispose();
        }

        protected virtual void OnEnable()
        {
            // Nothing to do yet. But we should keep this method for future.
        }

        protected virtual void OnDisable()
        {
            // Nothing to do yet. But we should keep this method for future.
        }
    }
}
