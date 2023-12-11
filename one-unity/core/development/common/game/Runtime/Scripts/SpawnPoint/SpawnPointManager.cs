using System;
using System.Collections.Generic;
using TPFive.Game.Extensions;
using UnityEngine;

using Random = UnityEngine.Random;

namespace TPFive.Game
{
    public abstract class SpawnPointManager<T> : MonoBehaviour, ISpawnPointManager<T>
        where T : Component, ISpawnPoint
    {
        [SerializeField]
        private SpawnSequence sequence;

        [SerializeField]
        private LayerMask blockingLayers;

        [SerializeField]
        private float blockedCheckRadius = 2f;
        private Random.State randomState;

        public enum SpawnSequence
        {
            /// <summary>
            /// player id % spawn point count
            /// </summary>
            PlayerId,

            /// <summary>
            /// Round robin spawn point selection
            /// </summary>
            RoundRobin,

            /// <summary>
            /// Random spawn point selection
            /// </summary>
            Random,
        }

        public int LastSpawnIndex { get; set; } = -1;

        public float BlockedCheckRadius => blockedCheckRadius;

        protected static Collider[] Blocked3D { get; set; } = null;

        protected List<Component> SpawnPoints { get; set; }

        public void CollectSpawnPoints()
        {
            if (SpawnPoints == null)
            {
                var components = gameObject.scene.GetComponentsInScene<T>();
                SpawnPoints = new List<Component>(components);
            }
            else
            {
                SpawnPoints.Clear();
                SpawnPoints.AddRange(gameObject.scene.GetComponentsInScene<T>());
            }
        }

        public virtual Transform GetNextSpawnPoint(int playerId, bool skipIfBlocked = true)
        {
            if (SpawnPoints == null || SpawnPoints.Count == 0)
            {
                return AllSpawnPointsBlockedOrSpawnPointsMissedFallback();
            }

            int spawnCount = SpawnPoints.Count;
            Component next;
            int nextIndex;
            if (sequence == SpawnSequence.PlayerId)
            {
                nextIndex = playerId % spawnCount;
                next = SpawnPoints[nextIndex];
            }
            else if (sequence == SpawnSequence.RoundRobin)
            {
                nextIndex = (LastSpawnIndex + 1) % spawnCount;
                next = SpawnPoints[nextIndex];
            }
            else
            {
                nextIndex = RandomRange(0, spawnCount);
                next = SpawnPoints[nextIndex];
            }

            // Handling for blocked spawn points. By default this never happens, as the IsBlocked test always returns true.
            if (skipIfBlocked && blockingLayers.value != 0 && IsBlocked(next))
            {
                var (unblockedIdx, unblockedSpawnPoint) = GetNextUnblocked(nextIndex);
                if (unblockedIdx > -1)
                {
                    LastSpawnIndex = unblockedIdx;
                    return unblockedSpawnPoint.transform;
                }

                // leave LastSpawnIndex the same since we haven't arrived at a new spawn point.
            }
            else
            {
                LastSpawnIndex = nextIndex;
                return next.transform;
            }

            return AllSpawnPointsBlockedOrSpawnPointsMissedFallback();
        }

        public virtual Transform AllSpawnPointsBlockedOrSpawnPointsMissedFallback()
        {
            return transform;
        }

        /// <summary>
        /// Cycles through all remaining spawn points searching for unblocked. Will return null if all points return <see cref="IsBlocked(Transform)"/> == true.
        /// </summary>
        /// <param name="failedIndex">The index of the first tried SpawnPoints[] element, which was blocked.</param>
        /// <returns>(<see cref="SpawnPoints"/> index, <see cref="ISpawnPointPrototype"/>).</returns>
        public virtual (int, Component) GetNextUnblocked(int failedIndex)
        {
            // search for unblocked spawn points after the failed index
            for (int i = failedIndex + 1, count = SpawnPoints.Count; i < count; ++i)
            {
                var spawnPoint = SpawnPoints[i];
                if (!IsBlocked(spawnPoint))
                {
                    return (i, spawnPoint);
                }
            }

            // search for unblocked spawn points before the failed index
            for (int i = 0, count = failedIndex; i < count; ++i)
            {
                var spawnPoint = SpawnPoints[i];
                if (!IsBlocked(spawnPoint))
                {
                    return (i, spawnPoint);
                }
            }

            return (-1, null);
        }

        public virtual bool IsBlocked(Component spawnPoint)
        {
            var physics3d = spawnPoint.gameObject.scene.GetPhysicsScene();
            if (physics3d != null)
            {
                Blocked3D ??= new Collider[1];

                var blockedCount = physics3d.OverlapSphere(
                    spawnPoint.transform.position,
                    blockedCheckRadius,
                    Blocked3D,
                    blockingLayers.value,
                    QueryTriggerInteraction.UseGlobal);

                return blockedCount > 0;
            }

            return false;
        }

        protected virtual void Awake()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            randomState = Random.state;
        }

        private int RandomRange(int inclusiveMin, int exclusiveMax)
        {
            var state = Random.state;
            Random.state = randomState;
            var value = Random.Range(inclusiveMin, exclusiveMax);
            Random.state = state;
            return value;
        }
    }
}