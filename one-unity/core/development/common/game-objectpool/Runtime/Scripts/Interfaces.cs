using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.ObjectPool
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        T Spawn<T>(string name, T leasing);
        void Despawn<T>(T leased);

        GameObject SpawnFromPrefab(string name, GameObject prefab);
        bool DespawnByGameObject(string name, GameObject inGO);
    }

    public interface IServiceProvider : TPFive.Game.IServiceProvider
    {
        T Spawn<T>(string name, T leasing);
        void Despawn<T>(T leased);

        GameObject SpawnFromPrefab(string name, GameObject prefab);
        bool DespawnByGameObject(string name, GameObject inGO);
    }
}
