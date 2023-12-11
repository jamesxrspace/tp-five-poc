using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace TPFive.Game.Record.Scene.SpawnPoint
{
    /// <summary>
    /// Add/Remove spawn points to <see cref="ISpawnPointService"/> when enabled/disabled.
    /// </summary>
    public sealed class MonoSpawnPointRegistry : MonoBehaviour
    {
        [SerializeField]
        private List<MonoSpawnPoint> spawnPoints;

        [SerializeField]
        [Tooltip("Default spawn point for player. Must be one of the spawn points.")]
        private MonoSpawnPoint defaultSpawnPoint;

        private ISpawnPointService spawnPointService;

        [Inject]
        public void Construct(ISpawnPointService pointService)
        {
            this.spawnPointService = pointService;

            AddPoints();
        }

        private void OnEnable()
        {
            if (spawnPointService == null)
            {
                return;
            }

            AddPoints();
        }

        private void OnDisable()
        {
            if (spawnPointService == null)
            {
                return;
            }

            RemovePoints();
        }

        private void AddPoints()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                bool isDefault = spawnPoint == defaultSpawnPoint;
                spawnPointService.AddPoint(spawnPoint.PointDesc, isDefault);
            }
        }

        private void RemovePoints()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                spawnPointService.RemovePoint(spawnPoint.PointDesc);
            }
        }
    }
}
