using System.Collections.Generic;
using System.Linq;
using TPFive.Game.Extensions;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class SpawnPointGroup : MonoBehaviour
    {
        [SerializeField]
        private List<SpawnPoint> spawnPoints;

        [SerializeField]
        private Transform left;
        [SerializeField]
        private Transform right;

        public Vector3 LeftWorldPosition => left.position;

        public Vector3 RightWorldPosition => right.position;

        public void GetSpawnPoints(List<SpawnPoint> result, bool isShuffle = false)
        {
            result.Clear();

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                return;
            }

            result.AddRange(spawnPoints);

            if (isShuffle)
            {
                result.Shuffle();
            }
        }

        protected void OnValidate()
        {
            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                spawnPoints = GetComponentsInChildren<SpawnPoint>(includeInactive: false)?.ToList();
            }
        }
    }
}