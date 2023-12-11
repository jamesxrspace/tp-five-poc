using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Room
{
    public class NetworkObjectPool : INetworkObjectPool
    {
        [Inject]
        private IObjectResolver objectResolver;

        public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
        {
            // Instantiate the prefab using VContainer to perform dependency injection
            if (runner.Config.PrefabTable.TryGetPrefab(info.Prefab, out var prefab))
            {
                return objectResolver.Instantiate(prefab);
            }

            return null;
        }

        public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
        {
            // Scene objects are the NetworkObjects existing in .unity and not those spawned using NetworkRunner during Fusion session.
            // Scene objects aren't destroyed here in order for NetSceneLoader to avoid reloading the same scene when Fusion session is reestablished.
            if (isSceneObject)
            {
                return;
            }

            Object.Destroy(instance.gameObject);
        }
    }
}
