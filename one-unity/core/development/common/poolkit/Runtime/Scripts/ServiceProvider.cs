using HellTap.PoolKit;
using UnityEngine;

namespace TPFive.Extended.PoolKit
{
    using GameObjectPool = TPFive.Game.ObjectPool;

    public sealed partial class ServiceProvider :
        GameObjectPool.IServiceProvider,
        HellTap.PoolKit.IPoolKitListener
    {
        //
        private readonly GameObjectPool.IServiceProvider _nullServiceProvider;
        private IPoolKitListener _poolKitListenerImplementation;

        //
        public T Spawn<T>(string name, T leasing)
        {
            // Logger.LogEditorDebug(
            //     "{Method}",
            //     nameof(Spawn));

            // returpool.SpawnGO(name);
            return _nullServiceProvider.Spawn<T>(name, leasing);
        }

        public void Despawn<T>(T leased)
        {
            // Logger.LogEditorDebug(
            //     "{Method}",
            //     nameof(Despawn));

            _nullServiceProvider.Despawn(leased);
        }

        public GameObject SpawnFromPrefab(string name, GameObject prefab)
        {
            // Logger.LogEditorDebug(
            //     "{Method}",
            //     nameof(SpawnFromPrefab));

            var pool = HellTap.PoolKit.PoolKit.FindPool(name);
            if (pool == null)
            {
                return _nullServiceProvider.SpawnFromPrefab(name, prefab);
            }

            var spawnedGO = pool.SpawnGO(prefab);
            if (spawnedGO != null)
            {
                spawnedGO.transform.SetParent(null);
            }

            return spawnedGO;
        }

        public bool DespawnByGameObject(string name, GameObject inGO)
        {
            // Logger.LogEditorDebug(
            //     "{Method}",
            //     nameof(DespawnByGameObject));

            var pool = HellTap.PoolKit.PoolKit.FindPool(name);
            if (pool == null)
            {
                return _nullServiceProvider.DespawnByGameObject(name, inGO);
            }

            inGO.transform.SetParent(pool.transform);
            var result = pool.Despawn(inGO);
            return result;
        }

        //
        public void OnSpawn(Pool pool)
        {
            // Logger.LogEditorDebug(
            //     "{Method} - {Pool}",
            //     nameof(OnSpawn),
            //     pool);
            // // _poolKitListenerImplementation.OnSpawn(pool);
        }

        public void OnDespawn()
        {
            // Logger.LogEditorDebug(
            //     "{Method}",
            //     nameof(OnDespawn));
            // // _poolKitListenerImplementation.OnDespawn();
        }
    }
}
