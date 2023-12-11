namespace TPFive.Extended.ResourceLoader
{
    using TPFive.Game.Resource;
    using UnityEngine.Pool;

    // The use of pool is still in plan, this may be overwhelming
    public interface IPool<T>
    {
        T Get();

        void Release(T inValue);
    }

    public class RemoteResponsePool : IPool<RemoteResponse>
    {
        private ObjectPool<RemoteResponse> _remoteResponsePool;

        public void Setup()
        {
            _remoteResponsePool = new ObjectPool<RemoteResponse>(
                createFunc: () =>
                {
                    return new RemoteResponse
                    {
                        Valid = false,
                    };
                },
                actionOnGet: x =>
                {
                },
                actionOnRelease: x =>
                {
                    x.Valid = false;
                },
                actionOnDestroy: x =>
                {
                },
                collectionCheck: true,
                defaultCapacity: 50);
        }

        public RemoteResponse Get()
        {
            return _remoteResponsePool.Get();
        }

        public void Release(RemoteResponse inValue)
        {
            _remoteResponsePool.Release(inValue);
        }
    }
}