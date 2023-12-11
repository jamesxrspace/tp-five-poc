using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace TPFive.Game.Config
{
    public sealed partial class Service
    {
        private async UniTask<(bool, IList<int>)> InternalGetIntValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var tasks = new List<UniTask<(bool, int)>>();

            foreach (var item in _serviceProviderTable)
            {
                var serviceProvider = item.Value;
                if (serviceProvider is IServiceProvider sp)
                {
                    tasks.Add(sp.GetIntValueAsync(key, cancellationToken));
                }
            }

            var results = await UniTask.WhenAll(tasks);

            var getResults = new List<int>();

            foreach (var (r, v) in results)
            {
                if (r)
                {
                    getResults.Add(v);
                }
            }

            return (getResults.Any(), getResults);
        }

        private async UniTask<(bool, IList<float>)> InternalGetFloatValueAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var tasks = new List<UniTask<(bool, float)>>();

            foreach (var item in _serviceProviderTable)
            {
                var serviceProvider = item.Value;
                if (serviceProvider is IServiceProvider sp)
                {
                    tasks.Add(sp.GetFloatValueAsync(key, cancellationToken));
                }
            }

            var results = await UniTask.WhenAll(tasks);

            var getResults = new List<float>();

            foreach (var (r, v) in results)
            {
                if (r)
                {
                    getResults.Add(v);
                }
            }

            return (getResults.Any(), getResults);
        }

        private async UniTask<(bool, IList<T>)> GetTValueAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
        {
            var tasks = new List<UniTask<(bool, T)>>();

            foreach (var item in _serviceProviderTable)
            {
                var serviceProvider = item.Value;
                if (serviceProvider is IServiceProvider sp)
                {
                    tasks.Add(sp.GetAsync<string, T>(key, cancellationToken));
                }
            }

            var results = await UniTask.WhenAll(tasks);

            var getResults = new List<T>();

            foreach (var (r, v) in results)
            {
                if (r)
                {
                    getResults.Add(v);
                }
            }

            return (getResults.Any(), getResults);
        }

        private async UniTask<(bool, int)> InternalGetSpecificProviderIntValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return await serviceProvider.GetIntValueAsync(key, cancellationToken);
        }

        private async UniTask<(bool, float)> InternalGetSpecificProviderFloatValueAsync(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return await serviceProvider.GetFloatValueAsync(key, cancellationToken);
        }

        private async UniTask<(bool, T)> GetSpecificProviderTValueAsync<T>(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return await serviceProvider.GetAsync<string, T>(key, cancellationToken);
        }

        private async UniTask<bool> SetTValueAsync<T>(
            ServiceProviderKind kind,
            string key,
            T value,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return await serviceProvider.SetAsync<string, T>(key, value, cancellationToken);
        }

        private async UniTask<bool> RemoveTValueAsync<T>(
            ServiceProviderKind kind,
            string key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return await serviceProvider.RemoveAsync<string, T>(key, cancellationToken);
        }

        private void GetTValue<T>(
            System.Func<string, CancellationToken, UniTask<(bool, T)>> inFunc,
            string key,
            System.Action<(bool, T)> withValueCallback)
        {
            inFunc(key, default)
                .ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(
                    x =>
                    {
                        withValueCallback?.Invoke(x);
                    },
                    e =>
                    {
                        withValueCallback?.Invoke(default);
                    })
                .AddTo(_compositeDisposable);
        }

        private void GetSpecificProviderTValue<T>(
            System.Func<ServiceProviderKind, string, CancellationToken, UniTask<T>> inFunc,
            ServiceProviderKind kind,
            string key,
            System.Action<T> withValueCallback)
        {
            inFunc(kind, key, default)
                .ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(
                    x =>
                    {
                        withValueCallback?.Invoke(x);
                    },
                    e =>
                    {
                        withValueCallback?.Invoke(default);
                    })
                .AddTo(_compositeDisposable);
        }

        private (bool, IList<int>) InternalGetIntValue(string key)
        {
            var getResults = new List<int>();

            foreach (var item in _serviceProviderTable)
            {
                var serviceProvider = item.Value;
                if (serviceProvider is IServiceProvider sp)
                {
                    var (result, v) = sp.GetIntValue(key);
                    if (result)
                    {
                        getResults.Add(v);
                    }
                }
            }

            return (getResults.Any(), getResults);
        }

        private (bool, IList<float>) InternalGetFloatValue(string key)
        {
            var getResults = new List<float>();

            foreach (var item in _serviceProviderTable)
            {
                var serviceProvider = item.Value;
                if (serviceProvider is IServiceProvider sp)
                {
                    var (result, v) = sp.GetFloatValue(key);
                    if (result)
                    {
                        getResults.Add(v);
                    }
                }
            }

            return (getResults.Any(), getResults);
        }

        private (bool, IList<T>) GetTValue<T>(string key)
        {
            var getResults = new List<T>();

            foreach (var item in _serviceProviderTable)
            {
                var serviceProvider = item.Value;
                if (serviceProvider is IServiceProvider sp)
                {
                    var (r, v) = sp.GetT<string, T>(key);
                    if (r)
                    {
                        getResults.Add(v);
                    }
                }
            }

            return (getResults.Any(), getResults);
        }

        private (bool, int) InternalGetSpecificProviderIntValue(
            ServiceProviderKind kind,
            string key)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return serviceProvider.GetIntValue(key);
        }

        private (bool, float) InternalGetSpecificProviderFloatValue(
            ServiceProviderKind kind,
            string key)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return serviceProvider.GetFloatValue(key);
        }

        private (bool, T) GetSpecificProviderTValue<T>(
            ServiceProviderKind kind,
            string key)
        {
            var serviceProvider = GetServiceProvider((int)kind);

            return serviceProvider.GetT<string, T>(key);
        }

        private void SetTValueCallbackWithResult<T>(
            ServiceProviderKind kind,
            string key,
            T value,
            System.Action<bool> resultCallback = null)
        {
            var serviceProvider = GetServiceProvider((int)kind);
            serviceProvider.SetAsync<string, T>(key, value)
                .ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(
                    x =>
                    {
                        resultCallback?.Invoke(x);
                    },
                    e =>
                    {
                        resultCallback?.Invoke(false);
                    })
                .AddTo(_compositeDisposable);
        }

        private bool SetTValue<T>(
            ServiceProviderKind kind,
            string key,
            T value)
        {
            var serviceProvider = GetServiceProvider((int)kind);
            var result = serviceProvider.SetT<string, T>(key, value);

            return result;
        }

        private void RemoveTValueCallbackWithResult<T>(
            ServiceProviderKind kind,
            string key,
            System.Action<bool> resultCallback = null)
        {
            RemoveTValueAsync<T>(kind, key)
                .ToObservable()
                .Subscribe(
                    x =>
                    {
                        resultCallback?.Invoke(x);
                    },
                    e =>
                    {
                        resultCallback?.Invoke(false);
                    })
                .AddTo(_compositeDisposable);
        }

        private bool RemoveTValue<TValue>(
            ServiceProviderKind kind,
            string key)
        {
            var serviceProvider = GetServiceProvider((int)kind);
            var result = serviceProvider.RemoveT<string, TValue>(key);

            return result;
        }
    }
}