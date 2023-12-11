using UnityEngine;

namespace TPFive.Game
{
    public interface ISpawnPointManager<T>
        where T : Component, ISpawnPoint
    {
        void CollectSpawnPoints();

        Transform GetNextSpawnPoint(int playerId, bool skipIfBlocked = true);
    }
}
