using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Extended.LocalConfig
{
    using GameConfig = TPFive.Game.Config;

    /// <summary>
    /// Provide a service to store and retrieve values.
    /// Expected most case will be string, scriptableobject and object usage.
    /// </summary>
    /// <remarks>
    /// Add non gc version to get int and float value.
    /// </remarks>
    public sealed partial class ServiceProvider :
        GameConfig.IServiceProvider
    {
        private readonly Dictionary<string, int> _intValueTable = new ();
        private readonly Dictionary<string, float> _floatValueTable = new ();
        private readonly Dictionary<string, string> _stringValueTable = new ();
        private readonly Dictionary<string, ScriptableObject> _scriptableObjectValueTable = new ();
        private readonly Dictionary<string, object> _objectValueTable = new ();

        /// <summary>
        /// Set the value by key.
        /// </summary>
        /// <remarks>
        /// For now, key is string type.
        /// </remarks>
        public async UniTask<bool> SetAsync<TKey, TValue>(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            if (key is not string k)
            {
                return await Task.FromResult(false);
            }

            // Here, using (value is SomeType someTypeValue) makes sense because the pass in value is not null,
            // but if it is really null, the result will be false as no case is matched.
            var result = InternalSetT(k, value);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Get the value by key.
        /// </summary>
        /// <remarks>
        /// For now, key is string type.
        /// </remarks>
        public async UniTask<(bool, TValue)> GetAsync<TKey, TValue>(
            TKey key,
            CancellationToken cancellationToken = default)
        {
            if (key is not string k)
            {
                return await Task.FromResult((false, default(TValue)));
            }

            var (result, value) = InternalGetT<TValue>(k);

            return await Task.FromResult((result, value));
        }

        /// <summary>
        /// Remove the value by key.
        /// </summary>
        /// <remarks>
        /// For now, key is string type.
        /// </remarks>
        public async UniTask<bool> RemoveAsync<TKey, TValue>(TKey key, CancellationToken cancellationToken = default)
        {
            if (key is not string k)
            {
                return await Task.FromResult(false);
            }

            var result = InternalRemoveT<TValue>(k);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Get the int value by key without boxing, unboxing.
        /// </summary>
        public async UniTask<(bool, int)> GetIntValueAsync<TKey>(
            TKey key,
            CancellationToken cancellationToken = default)
        {
            if (key is not string k)
            {
                return await Task.FromResult((false, default(int)));
            }

            var r = _intValueTable.TryGetValue(k, out var v);

            return await Task.FromResult((r, v));
        }

        /// <summary>
        /// Get the float value by key without boxing, unboxing.
        /// </summary>
        public async UniTask<(bool, float)> GetFloatValueAsync<TKey>(
            TKey key,
            CancellationToken cancellationToken = default)
        {
            if (key is not string k)
            {
                return await Task.FromResult((false, default(int)));
            }

            var r = _floatValueTable.TryGetValue(k, out var v);

            return await Task.FromResult((r, v));
        }

        public bool SetT<TKey, TValue>(TKey key, TValue value)
        {
            if (key is not string k)
            {
                return false;
            }

            var result = InternalSetT(k, value);

            return result;
        }

        public (bool, TValue) GetT<TKey, TValue>(TKey key)
        {
            if (key is not string k)
            {
                return (false, default(TValue));
            }

            var (result, value) = InternalGetT<TValue>(k);

            return (result, value);
        }

        public bool RemoveT<TKey, TValue>(TKey key)
        {
            if (key is not string k)
            {
                return false;
            }

            var result = InternalRemoveT<TValue>(k);

            return result;
        }

        public (bool, int) GetIntValue<TKey>(TKey key)
        {
            if (key is not string k)
            {
                return (false, default);
            }

            var r = _intValueTable.TryGetValue(k, out var v);

            return (r, v);
        }

        public (bool, float) GetFloatValue<TKey>(TKey key)
        {
            if (key is not string k)
            {
                return (false, default);
            }

            var r = _floatValueTable.TryGetValue(k, out var v);

            return (r, v);
        }
    }
}
