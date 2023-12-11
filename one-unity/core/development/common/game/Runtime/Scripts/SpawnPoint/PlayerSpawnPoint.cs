using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPFive.Game
{
    /// <summary>
    /// Flag component to identify GameObjects that should be used as markers for spawn points.
    /// </summary>
    public class PlayerSpawnPoint : MonoBehaviour, ISpawnPoint
    {
#if UNITY_EDITOR
        private static bool spawnPointIsSelected;
        private float radiusForGizmo = .5f;

        public void SetRadiusForGizmo(float radius)
        {
            this.radiusForGizmo = radius;
        }

        protected void OnDrawGizmosSelected()
        {
            // If the selected object contains a spawn point, show gizmos for ALL spawn points.
            CheckIfSpawnPointIsSelected();
        }

        protected void OnDrawGizmos()
        {
            // If one spawn point is selected, all spawn points will draw a gizmo.
            if (!spawnPointIsSelected)
            {
                return;
            }

            // Check if spawn point has since been deselected.
            if (!CheckIfSpawnPointIsSelected())
            {
                return;
            }

            const float forwardLength = 2;
            const float otherLength = 1f;

            var pos = transform.position;
            var forward = transform.forward;
            var right = transform.right;
            var up = transform.up;

            Gizmos.DrawRay(pos, forward * forwardLength);
            Gizmos.DrawRay(pos, right * otherLength);
            Gizmos.DrawRay(pos, up * otherLength);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(pos, radiusForGizmo);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pos + (forward * forwardLength), .25f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos + (right * otherLength), .25f);
            Gizmos.DrawSphere(pos - (right * otherLength), .25f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pos + (up * otherLength), .25f);
        }

        private bool CheckIfSpawnPointIsSelected()
        {
            if (Selection.activeGameObject != null)
            {
                spawnPointIsSelected = Selection.activeGameObject.GetComponent<PlayerSpawnPoint>();
            }
            else
            {
                spawnPointIsSelected = false;
            }

            return spawnPointIsSelected;
        }
#endif
    }
}
