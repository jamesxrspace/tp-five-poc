using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace TPFive.Extended.UnityRemoteConfig
{
    using GameConfig = TPFive.Game.Config;

    /// <summary>
    /// This uses Unity remote config to get the config values.
    /// </summary>
    /// <remarks>
    /// Not sure if Unity remote config is used, keep implementation blank for now.
    /// </remarks>
    public sealed partial class ServiceProvider :
        GameConfig.IServiceProvider
    {
        public async UniTask<bool> SetAsync<TKey, TValue>(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            return await _nullServiceProvider.SetAsync(key, value, cancellationToken);
        }

        public async UniTask<(bool, TValue)> GetAsync<TKey, TValue>(
            TKey key,
            CancellationToken cancellationToken = default)
        {
            return await _nullServiceProvider.GetAsync<TKey, TValue>(key, cancellationToken);
        }

        public async UniTask<bool> RemoveAsync<TKey, TValue>(TKey key, CancellationToken cancellationToken = default)
        {
            return await _nullServiceProvider.RemoveAsync<TKey, TValue>(key, cancellationToken);
        }

        public async UniTask<(bool, int)> GetIntValueAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
        {
            return await _nullServiceProvider.GetIntValueAsync(key, cancellationToken);
        }

        public async UniTask<(bool, float)> GetFloatValueAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
        {
            return await _nullServiceProvider.GetFloatValueAsync(key, cancellationToken);
        }

        public bool SetT<TKey, TValue>(TKey key, TValue value)
        {
            return _nullServiceProvider.SetT(key, value);
        }

        public (bool, TValue) GetT<TKey, TValue>(TKey key)
        {
            return _nullServiceProvider.GetT<TKey, TValue>(key);
        }

        public bool RemoveT<TKey, TValue>(TKey key)
        {
            return _nullServiceProvider.RemoveT<TKey, TValue>(key);
        }

        public (bool, int) GetIntValue<TKey>(TKey key)
        {
            return _nullServiceProvider.GetIntValue(key);
        }

        public (bool, float) GetFloatValue<TKey>(TKey key)
        {
            return _nullServiceProvider.GetFloatValue(key);
        }
    }
}
