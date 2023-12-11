using UnityEngine;

namespace TPFive.Room
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private float _distanceToTarget = 10.0f;
        private Transform _cachedTransform;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        public float DistanceToTarget
        {
            get => _distanceToTarget;
            set => _distanceToTarget = Mathf.Max(0.0f, value);
        }

        private void Start()
        {
            SetupMainCamera();

            if (_target == null)
            {
                return;
            }

            _cachedTransform.position = Target.position - (_cachedTransform.forward * DistanceToTarget);
        }

        private void OnValidate()
        {
            DistanceToTarget = _distanceToTarget;
        }

        private void Awake()
        {
            _cachedTransform = transform;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            _cachedTransform.position = Target.position - (_cachedTransform.forward * DistanceToTarget);
        }

        private void SetupMainCamera()
        {
            // Enable the Camera to which this component attaches and set it as the main camera.
            // Disable other Cameras and set those cameras' tag as 'Untagged'.
            if (TryGetComponent<Camera>(out var selfCamera))
            {
                var cameras = UnityEngine.Object.FindObjectsOfType<Camera>();
                foreach (var camera in cameras)
                {
                    if (camera == selfCamera)
                    {
                        camera.enabled = true;
                        camera.tag = "MainCamera";
                    }
                    else
                    {
                        camera.enabled = false;
                        camera.tag = "Untagged";
                    }
                }
            }
        }
    }
}
