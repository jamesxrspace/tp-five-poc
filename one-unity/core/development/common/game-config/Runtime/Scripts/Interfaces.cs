using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Config
{
    /// <summary>
    /// Provide config use api.
    /// </summary>
    /// <remarks>
    /// Providing both non-generic and generic and sync, async methods for now, and see if
    /// any is redundant and remove later.
    /// Since basic type get is implemented differently, so no generic version provided.
    /// Align the use of return value using bool instead of nullable.
    /// </remarks>
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        /// <summary>
        /// Get int value from all service providers.
        /// </summary>
        UniTask<(bool, int)> GetIntValueAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get float value from all service providers.
        /// </summary>
        UniTask<(bool, float)> GetFloatValueAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get string value from all service providers.
        /// </summary>
        UniTask<(bool, string)> GetStringValueAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get scriptableobject value from all service providers.
        /// </summary>
        UniTask<(bool, ScriptableObject)> GetScriptableObjectValueAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get object value from all service providers.
        /// </summary>
        UniTask<(bool, object)> GetSystemObjectValueAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get int value from specific service provider.
        /// </summary>
        UniTask<(bool, int)> GetSpecificProviderIntValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get float value from specific service provider.
        /// </summary>
        UniTask<(bool, float)> GetSpecificProviderFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get string value from specific service provider.
        /// </summary>
        UniTask<(bool, string)> GetSpecificProviderStringValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get scriptableobject value from specific service provider.
        /// </summary>
        UniTask<(bool, ScriptableObject)> GetSpecificProviderScriptableObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get object value from specific service provider.
        /// </summary>
        UniTask<(bool, object)> GetSpecificProviderSystemObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set int value to specific service provider.
        /// </summary>
        UniTask<bool> SetIntValueAsync(
            ServiceProviderKind kind,
            string key,
            int value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set float value to specific service provider.
        /// </summary>
        UniTask<bool> SetFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            float value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set string value to specific service provider.
        /// </summary>
        UniTask<bool> SetStringValueAsync(
            ServiceProviderKind kind,
            string key,
            string value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set scriptableobject value to specific service provider.
        /// </summary>
        UniTask<bool> SetScriptableObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            ScriptableObject value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set object value to specific service provider.
        /// </summary>
        UniTask<bool> SetSystemObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            object value,
            CancellationToken cancellationToken = default);

        UniTask<bool> RemoveIntValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        UniTask<bool> RemoveFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        UniTask<bool> RemoveStringValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        UniTask<bool> RemoveScriptableObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        UniTask<bool> RemoveSystemObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get int value by checking all service providers, but only get the first value.
        /// </summary>
        void GetIntValueFromCallback(string key, System.Action<(bool, int)> withValueCallback = null);

        /// <summary>
        /// Get float value by checking all service providers, but only get the first value.
        /// </summary>
        void GetFloatValueFromCallback(string key, System.Action<(bool, float)> withValueCallback = null);

        /// <summary>
        /// Get string value by checking all service providers, but only get the first value.
        /// </summary>
        void GetStringValueFromCallback(string key, System.Action<(bool, string)> withValueCallback = null);

        /// <summary>
        /// Get scriptableobject value by checking all service providers, but only get the first value.
        /// </summary>
        void GetScriptableObjectFromCallback(string key, System.Action<(bool, ScriptableObject)> withValueCallback = null);

        /// <summary>
        /// Get object value by checking all service providers, but only get the first value.
        /// </summary>
        /// <remarks>
        /// This is convenient but not recommended, because it will cause boxing/unboxing thus performance issue.
        /// </remarks>
        void GetSystemObjectValueFromCallback(string key, System.Action<(bool, object)> withValueCallback = null);

        /// <summary>
        /// Get int value from specific service provider.
        /// </summary>
        void GetSpecificProviderIntValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, int)> withValueCallback = null);

        /// <summary>
        /// Get float value from specific service provider.
        /// </summary>
        void GetSpecificProviderFloatValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, float)> withValueCallback = null);

        /// <summary>
        /// Get string value from specific service provider.
        /// </summary>
        void GetSpecificProviderStringValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, string)> withValueCallback = null);

        /// <summary>
        /// Get scriptableobject value from specific service provider.
        /// </summary>
        void GetSpecificProviderScriptableObjectValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, ScriptableObject)> withValueCallback = null);

        /// <summary>
        /// Get object value from specific service provider.
        /// </summary>
        void GetSpecificProviderSystemObjectValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, object)> withValueCallback = null);

        /// <summary>
        /// Get int value by checking all service providers, but only get the first value.
        /// </summary>
        (bool, int) GetIntValue(string key);

        /// <summary>
        /// Get float value by checking all service providers, but only get the first value.
        /// </summary>
        (bool, float) GetFloatValue(string key);

        /// <summary>
        /// Get string value by checking all service providers, but only get the first value.
        /// </summary>
        (bool, string) GetStringValue(string key);

        /// <summary>
        /// Get scriptableobject value by checking all service providers, but only get the first value.
        /// </summary>
        (bool, ScriptableObject) GetScriptableObject(string key);

        /// <summary>
        /// Get object value by checking all service providers, but only get the first value.
        /// </summary>
        /// <remarks>
        /// This is convenient but not recommended, because it will cause boxing/unboxing thus performance issue.
        /// </remarks>
        (bool, object) GetSystemObjectValue(string key);

        /// <summary>
        /// Get int value from specific service provider.
        /// </summary>
        (bool, int) GetSpecificProviderIntValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Get float value from specific service provider.
        /// </summary>
        (bool, float) GetSpecificProviderFloatValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Get string value from specific service provider.
        /// </summary>
        (bool, string) GetSpecificProviderStringValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Get scriptableobject value from specific service provider.
        /// </summary>
        (bool, ScriptableObject) GetSpecificProviderScriptableObjectValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Get object value from specific service provider.
        /// </summary>
        (bool, object) GetSpecificProviderSystemObjectValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Set int value to specific service provider.
        /// </summary>
        void SetIntValueCallbackWithResult(ServiceProviderKind kind, string key, int value, System.Action<bool> resultCallback = null);

        /// <summary>
        /// Set float value to specific service provider.
        /// </summary>
        void SetFloatValueCallbackWithResult(ServiceProviderKind kind, string key, float value, System.Action<bool> resultCallback = null);

        /// <summary>
        /// Set string value to specific service provider.
        /// </summary>
        void SetStringValueCallbackWithResult(ServiceProviderKind kind, string key, string value, System.Action<bool> resultCallback = null);

        /// <summary>
        /// Set scriptableobject value to specific service provider.
        /// </summary>
        void SetScriptableObjectCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            ScriptableObject value,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Set object value to specific service provider.
        /// </summary>
        /// <remarks>
        /// This is convenient but not recommended, because it will cause boxing/unboxing thus performance issue.
        /// </remarks>
        void SetSystemObjectValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            object value,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Set int value to specific service provider.
        /// </summary>
        bool SetIntValue(ServiceProviderKind kind, string key, int value);

        /// <summary>
        /// Set float value to specific service provider.
        /// </summary>
        bool SetFloatValue(ServiceProviderKind kind, string key, float value);

        /// <summary>
        /// Set string value to specific service provider.
        /// </summary>
        bool SetStringValue(ServiceProviderKind kind, string key, string value);

        /// <summary>
        /// Set scriptableobject value to specific service provider.
        /// </summary>
        bool SetScriptableObject(
            ServiceProviderKind kind,
            string key,
            ScriptableObject value);

        /// <summary>
        /// Set object value to specific service provider.
        /// </summary>
        /// <remarks>
        /// This is convenient but not recommended, because it will cause boxing/unboxing thus performance issue.
        /// </remarks>
        bool SetSystemObjectValue(
            ServiceProviderKind kind,
            string key,
            object value);

        /// <summary>
        /// Remove int value from specific service provider.
        /// </summary>
        void RemoveIntValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Remove float value from specific service provider.
        /// </summary>
        void RemoveFloatValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Remove string value from specific service provider.
        /// </summary>
        void RemoveStringValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Remove scriptableobject value from specific service provider.
        /// </summary>
        void RemoveScriptableObjectValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Remove object value from specific service provider.
        /// </summary>
        void RemoveSystemObjectValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null);

        /// <summary>
        /// Remove int value from specific service provider.
        /// </summary>
        bool RemoveIntValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Remove float value from specific service provider.
        /// </summary>
        bool RemoveFloatValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Remove string value from specific service provider.
        /// </summary>
        bool RemoveStringValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Remove scriptableobject value from specific service provider.
        /// </summary>
        bool RemoveScriptableObjectValue(
            ServiceProviderKind kind,
            string key);

        /// <summary>
        /// Remove object value from specific service provider.
        /// </summary>
        bool RemoveSystemObjectValue(
            ServiceProviderKind kind,
            string key);
    }

    public interface IServiceProvider : TPFive.Game.IServiceProvider
    {
        // Async version.
        UniTask<bool> SetAsync<TKey, TValue>(TKey key, TValue value, CancellationToken cancellationToken = default);

        UniTask<(bool, TValue)> GetAsync<TKey, TValue>(TKey key, CancellationToken cancellationToken = default);

        UniTask<bool> RemoveAsync<TKey, TValue>(TKey key, CancellationToken cancellationToken = default);

        UniTask<(bool, int)> GetIntValueAsync<TKey>(TKey key, CancellationToken cancellationToken = default);

        UniTask<(bool, float)> GetFloatValueAsync<TKey>(TKey key, CancellationToken cancellationToken = default);

        // Sync version.
        bool SetT<TKey, TValue>(TKey key, TValue value);

        (bool, TValue) GetT<TKey, TValue>(TKey key);

        bool RemoveT<TKey, TValue>(TKey key);

        (bool, int) GetIntValue<TKey>(TKey key);

        (bool, float) GetFloatValue<TKey>(TKey key);
    }
}
