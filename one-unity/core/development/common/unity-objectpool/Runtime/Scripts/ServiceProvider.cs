using UnityEngine;

namespace TPFive.Extended.UnityObjectPool
{
    //
    using TPFive.Game.Logging;

    //
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using GameObjectPool = TPFive.Game.ObjectPool;

    public sealed partial class ServiceProvider :
        GameObjectPool.IServiceProvider
    {
        //
        public T Spawn<T>(string name, T leasing)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(Spawn));

            return _nullServiceProvider.Spawn<T>(name, leasing);
        }

        public void Despawn<T>(T leased)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(Despawn));

            _nullServiceProvider.Despawn(leased);
        }

        public GameObject SpawnFromPrefab(string name, GameObject prefab)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(Spawn));

            return _nullServiceProvider.SpawnFromPrefab(name, prefab);
        }

        public bool DespawnByGameObject(string name, GameObject prefab)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(Despawn));

            return _nullServiceProvider.DespawnByGameObject(name, prefab);
        }
    }
}
