using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPFive.Game
{
    /// <summary>
    /// Generates points at random position around a transform.
    /// </summary>
    /// <typeparam name="T">The type argument of the SpawnPoint.</typeparam>
    public abstract class RandomPointGenerator<T> : MonoBehaviour
        where T : Component, ISpawnPoint
    {
        [SerializeField]
        [Range(3, 25)]
        private int distancePerPoint = 3;

        [SerializeField]
        [Range(1, 64)]
        private int maxPoints = 5;

        [SerializeField]
        [Range(10, 200)]
        private float radius = 10f;

        [SerializeField]
        private bool alignYAxis = true;

#if UNITY_EDITOR
        protected List<Vector3> Positions { get; set; } = new List<Vector3>();

        protected virtual void GeneratePoints()
        {
            Positions.Clear();
            var children = GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child != transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            var sqrDistancePerPoint = this.distancePerPoint * this.distancePerPoint;

            for (int i = 0; i < maxPoints; ++i)
            {
                var position = transform.TransformPoint(Random.insideUnitSphere * radius);
                if (alignYAxis)
                {
                    position.y = transform.position.y;
                }

                // Check distance between points
                bool isValid = true;
                foreach (var existingPosition in Positions)
                {
                    if ((position - existingPosition).sqrMagnitude < sqrDistancePerPoint)
                    {
                        isValid = false;
                        break;
                    }
                }

                // If the point is valid, add it to the list with a random Y rotation
                if (isValid)
                {
                    Positions.Add(position);
                }
            }

            for (int i = 0; i < Positions.Count; i++)
            {
                var position = Positions[i];

                var newPoint = new GameObject($"Point {i}");
                newPoint.AddComponent<T>();
                newPoint.transform.SetParent(transform);
                newPoint.transform.SetPositionAndRotation(position, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            if (alignYAxis)
            {
                Handles.DrawWireDisc(transform.position, Vector3.up, radius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
#endif
    }
}