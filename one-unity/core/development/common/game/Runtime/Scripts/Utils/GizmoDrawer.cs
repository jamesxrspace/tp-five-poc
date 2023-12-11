using UnityEngine;

namespace TPFive.Game.Utils
{
    /// <summary>
    /// Helper class to draw gizmos from outside MonoBehaviours.
    /// </summary>
    public class GizmoDrawer : MonoBehaviour
    {
#if UNITY_EDITOR
        private readonly float forwardArrowWingAngle = 0.577f; // 60 degree
        private readonly float forwardArrowWingLenRatio = 0.2f;
#endif

        [SerializeField]
        private bool showGizmo = true;

        [SerializeField]
        private Type drawType = Type.Sphere;

        [SerializeField]
        private Color color = new Color(1, 1, 0);

        [SerializeField]
        private Vector3 centerOffest;

        [SerializeField]
        private Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);

        [SerializeField]
        private float radius = 0.1f;

        [SerializeField]
        private bool showForwardArrow = true;

        [SerializeField]
        private float forwardArrowLength = 0.5f;

        public enum Type
        {
            Cube,
            WireCube,
            Sphere,
            WireSphere,
        }

        public bool ShowGizmo => showGizmo;

        public Type DrawType => drawType;

        public Color Color => color;

        public Vector3 CenterOffest => centerOffest;

        public Vector3 Size => size;

        public float Radius => radius;

        public bool ShowForwardArrow => showForwardArrow;

        public float ForwardArrowLength => forwardArrowLength;

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            if (!ShowGizmo)
            {
                return;
            }

            var transform = this.transform;
            var center = transform.position + centerOffest;
            Gizmos.color = color;

            // Draw type
            switch (drawType)
            {
                case Type.Cube:
                    Gizmos.DrawCube(center, size);
                    break;
                case Type.WireCube:
                    Gizmos.DrawWireCube(center, size);
                    break;
                case Type.Sphere:
                    Gizmos.DrawSphere(center, radius);
                    break;
                case Type.WireSphere:
                    Gizmos.DrawWireSphere(center, radius);
                    break;
            }

            // Draw forward arrow
            if (showForwardArrow)
            {
                Vector3 headPoint = center + (transform.forward * forwardArrowLength);
                Vector3 wingCenter = center + ((1 - forwardArrowWingLenRatio) * forwardArrowLength * transform.forward);

                float wingOffest = forwardArrowLength * forwardArrowWingLenRatio * forwardArrowWingAngle;
                Vector3 leftWingPoint = wingCenter - (transform.right * wingOffest);
                Vector3 rightWingPoint = wingCenter + (transform.right * wingOffest);

                Gizmos.DrawLine(headPoint, center);
                Gizmos.DrawLine(headPoint, leftWingPoint);
                Gizmos.DrawLine(headPoint, rightWingPoint);
            }
        }
#endif
    } // END class
} // END namespace
