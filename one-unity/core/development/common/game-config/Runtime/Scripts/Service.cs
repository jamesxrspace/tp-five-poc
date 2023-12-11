using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Config
{
    public sealed partial class Service
    {
        // Get in async manner.
        public async UniTask<(bool, int)> GetIntValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var (r, l) = await InternalGetIntValueAsync(key, cancellationToken);
            return (r, l.FirstOrDefault());
        }

        public async UniTask<(bool, float)> GetFloatValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var (r, l) = await InternalGetFloatValueAsync(key, cancellationToken);
            return (r, l.FirstOrDefault());
        }

        public async UniTask<(bool, string)> GetStringValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var (r, l) = await GetTValueAsync<string>(key, cancellationToken);
            return (r, l.FirstOrDefault());
        }

        public async UniTask<(bool, ScriptableObject)> GetScriptableObjectValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var (r, l) = await GetTValueAsync<ScriptableObject>(key, cancellationToken);
            return (r, l.FirstOrDefault());
        }

        public async UniTask<(bool, object)> GetSystemObjectValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var (r, l) = await GetTValueAsync<object>(key, cancellationToken);
            return (r, l.FirstOrDefault());
        }

        // Get from specific provider in async manner.
        public async UniTask<(bool, int)> GetSpecificProviderIntValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await InternalGetSpecificProviderIntValueAsync(kind, key, cancellationToken);

        public async UniTask<(bool, float)> GetSpecificProviderFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await InternalGetSpecificProviderFloatValueAsync(kind, key, cancellationToken);

        public async UniTask<(bool, string)> GetSpecificProviderStringValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await GetSpecificProviderTValueAsync<string>(kind, key, cancellationToken);

        public async UniTask<(bool, ScriptableObject)> GetSpecificProviderScriptableObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await GetSpecificProviderTValueAsync<ScriptableObject>(kind, key, cancellationToken);

        public async UniTask<(bool, object)> GetSpecificProviderSystemObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await GetSpecificProviderTValueAsync<object>(kind, key, cancellationToken);

        // Set in async manner.
        public async UniTask<bool> SetIntValueAsync(
            ServiceProviderKind kind,
            string key,
            int value,
            CancellationToken cancellationToken = default) =>
            await SetTValueAsync(kind, key, value, cancellationToken);

        public async UniTask<bool> SetFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            float value,
            CancellationToken cancellationToken = default) =>
            await SetTValueAsync(kind, key, value, cancellationToken);

        public async UniTask<bool> SetStringValueAsync(
            ServiceProviderKind kind,
            string key,
            string value,
            CancellationToken cancellationToken = default) =>
            await SetTValueAsync(kind, key, value, cancellationToken);

        public async UniTask<bool> SetScriptableObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            ScriptableObject value,
            CancellationToken cancellationToken = default) =>
            await SetTValueAsync(kind, key, value, cancellationToken);

        public async UniTask<bool> SetSystemObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            object value,
            CancellationToken cancellationToken = default) =>
            await SetTValueAsync(kind, key, value, cancellationToken);

        public async UniTask<bool> RemoveIntValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await RemoveTValueAsync<int>(kind, key, cancellationToken);

        public async UniTask<bool> RemoveFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await RemoveTValueAsync<float>(kind, key, cancellationToken);

        public async UniTask<bool> RemoveStringValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await RemoveTValueAsync<string>(kind, key, cancellationToken);

        public async UniTask<bool> RemoveScriptableObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await RemoveTValueAsync<ScriptableObject>(kind, key, cancellationToken);

        public async UniTask<bool> RemoveSystemObjectValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default) =>
            await RemoveTValueAsync<object>(kind, key, cancellationToken);

        // Get in sync manner.
        public void GetIntValueFromCallback(
            string key,
            System.Action<(bool, int)> withValueCallback = null) =>
            GetTValue(GetIntValueAsync, key, withValueCallback);

        public void GetFloatValueFromCallback(
            string key,
            System.Action<(bool, float)> withValueCallback = null) =>
            GetTValue(GetFloatValueAsync, key, withValueCallback);

        public void GetStringValueFromCallback(
            string key,
            System.Action<(bool, string)> withValueCallback = null) =>
            GetTValue(GetStringValueAsync, key, withValueCallback);

        public void GetScriptableObjectFromCallback(
            string key,
            System.Action<(bool, ScriptableObject)> withValueCallback = null) =>
            GetTValue(GetScriptableObjectValueAsync, key, withValueCallback);

        public void GetSystemObjectValueFromCallback(
            string key,
            System.Action<(bool, object)> withValueCallback = null) =>
            GetTValue(GetSystemObjectValueAsync, key, withValueCallback);

        public (bool, int) GetIntValue(string key)
        {
            var (r, l) = InternalGetIntValue(key);
            return (r, l.FirstOrDefault());
        }

        public (bool, float) GetFloatValue(string key)
        {
            var (r, l) = InternalGetFloatValue(key);
            return (r, l.FirstOrDefault());
        }

        public (bool, string) GetStringValue(string key)
        {
            var (r, l) = GetTValue<string>(key);
            return (r, l.FirstOrDefault());
        }

        public (bool, ScriptableObject) GetScriptableObject(string key)
        {
            var (r, l) = GetTValue<ScriptableObject>(key);
            return (r, l.FirstOrDefault());
        }

        public (bool, object) GetSystemObjectValue(string key)
        {
            var (r, l) = GetTValue<object>(key);
            return (r, l.FirstOrDefault());
        }

        public void GetSpecificProviderIntValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, int)> withValueCallback = null) =>
            GetSpecificProviderTValue(InternalGetSpecificProviderIntValueAsync, kind, key, withValueCallback);

        public void GetSpecificProviderFloatValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, float)> withValueCallback = null) =>
            GetSpecificProviderTValue(InternalGetSpecificProviderFloatValueAsync, kind, key, withValueCallback);

        public void GetSpecificProviderStringValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, string)> withValueCallback = null) =>
            GetSpecificProviderTValue(GetSpecificProviderStringValueAsync, kind, key, withValueCallback);

        public void GetSpecificProviderScriptableObjectValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, ScriptableObject)> withValueCallback = null) =>
            GetSpecificProviderTValue(GetSpecificProviderScriptableObjectValueAsync, kind, key, withValueCallback);

        public void GetSpecificProviderSystemObjectValueFromCallback(
            ServiceProviderKind kind,
            string key,
            System.Action<(bool, object)> withValueCallback = null) =>
            GetSpecificProviderTValue(GetSpecificProviderSystemObjectValueAsync, kind, key, withValueCallback);

        public (bool, int) GetSpecificProviderIntValue(
            ServiceProviderKind kind,
            string key) =>
            InternalGetSpecificProviderIntValue(kind, key);

        public (bool, float) GetSpecificProviderFloatValue(
            ServiceProviderKind kind,
            string key) =>
            InternalGetSpecificProviderFloatValue(kind, key);

        public (bool, string) GetSpecificProviderStringValue(
            ServiceProviderKind kind,
            string key) =>
            GetSpecificProviderTValue<string>(kind, key);

        public (bool, ScriptableObject) GetSpecificProviderScriptableObjectValue(
            ServiceProviderKind kind,
            string key) =>
            GetSpecificProviderTValue<ScriptableObject>(kind, key);

        public (bool, object) GetSpecificProviderSystemObjectValue(
            ServiceProviderKind kind,
            string key) =>
            GetSpecificProviderTValue<object>(kind, key);

        public void SetIntValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            int value,
            System.Action<bool> resultCallback = null) =>
            SetTValueCallbackWithResult(kind, key, value, resultCallback);

        public void SetFloatValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            float value,
            System.Action<bool> resultCallback = null) =>
            SetTValueCallbackWithResult(kind, key, value, resultCallback);

        public void SetStringValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            string value,
            System.Action<bool> resultCallback = null) =>
            SetTValueCallbackWithResult(kind, key, value, resultCallback);

        public void SetScriptableObjectCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            UnityEngine.ScriptableObject value,
            System.Action<bool> resultCallback = null) =>
            SetTValueCallbackWithResult(kind, key, value, resultCallback);

        public void SetSystemObjectValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            object value,
            System.Action<bool> resultCallback = null) =>
            SetTValueCallbackWithResult(kind, key, value, resultCallback);

        /// <summary>
        /// Generic version to set value to specific service provider.
        /// </summary>
        /// <remarks>
        /// Underlying, only support int, float, string, scriptableobject, and object.
        /// </remarks>
        public void SetValueCallbackWithResult<T>(
            ServiceProviderKind kind,
            string key,
            T value,
            System.Action<bool> resultCallback = null) =>
            SetTValueCallbackWithResult(kind, key, value, resultCallback);

        public bool SetIntValue(ServiceProviderKind kind, string key, int value) =>
            SetTValue(kind, key, value);

        public bool SetFloatValue(ServiceProviderKind kind, string key, float value) =>
            SetTValue(kind, key, value);

        public bool SetStringValue(ServiceProviderKind kind, string key, string value) =>
            SetTValue(kind, key, value);

        public bool SetScriptableObject(
            ServiceProviderKind kind,
            string key,
            ScriptableObject value) =>
            SetTValue(kind, key, value);

        public bool SetSystemObjectValue(
            ServiceProviderKind kind,
            string key,
            object value) =>
            SetTValue(kind, key, value);

        public void RemoveIntValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null) =>
            RemoveTValueCallbackWithResult<int>(kind, key, resultCallback);

        public void RemoveFloatValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null) =>
            RemoveTValueCallbackWithResult<float>(kind, key, resultCallback);

        public void RemoveStringValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null) =>
            RemoveTValueCallbackWithResult<string>(kind, key, resultCallback);

        public void RemoveScriptableObjectValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null) =>
            RemoveTValueCallbackWithResult<ScriptableObject>(kind, key, resultCallback);

        public void RemoveSystemObjectValueCallbackWithResult(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null) =>
            RemoveTValueCallbackWithResult<object>(kind, key, resultCallback);

        public bool RemoveIntValue(
            ServiceProviderKind kind,
            string key) =>
            RemoveTValue<int>(kind, key);

        public bool RemoveFloatValue(
            ServiceProviderKind kind,
            string key) =>
            RemoveTValue<float>(kind, key);

        public bool RemoveStringValue(
            ServiceProviderKind kind,
            string key) =>
            RemoveTValue<string>(kind, key);

        public bool RemoveScriptableObjectValue(
            ServiceProviderKind kind,
            string key) =>
            RemoveTValue<ScriptableObject>(kind, key);

        public bool RemoveSystemObjectValue(
            ServiceProviderKind kind,
            string key) =>
            RemoveTValue<object>(kind, key);
    }
}
