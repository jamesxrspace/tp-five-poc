using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class SphereVisibleBound : MonoBehaviour
    {
        [SerializeField]
        private float radius = 1f;
        [SerializeField]
        private SphereCollider sphereCollider;
        private float? squaredRadius;
        [SerializeField]
        private bool isTransformChanged;

        public float Radius => radius;

        private float SquaredRadius
        {
            get
            {
                if (!squaredRadius.HasValue)
                {
                    squaredRadius = radius * radius;
                }

                return squaredRadius.Value;
            }
        }

        /// <summary>
        /// Calculate if a position is inside or outside of the spherical bounds.
        /// </summary>
        public bool IsInside(Vector3 worldPosition)
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;

                CalculateBoundSize();
            }

            var sqrDistance = (transform.position - worldPosition).sqrMagnitude;
            return sqrDistance <= SquaredRadius;
        }

        protected void OnValidate()
        {
            CalculateBoundSize();
        }

        protected void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void CalculateBoundSize()
        {
            sphereCollider.enabled = true;
            radius = sphereCollider.radius;
            var scale = transform.lossyScale;
            radius *= Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            sphereCollider.enabled = false;
            squaredRadius = radius * radius;
        }
    }
}