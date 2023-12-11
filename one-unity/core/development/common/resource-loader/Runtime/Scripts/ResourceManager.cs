using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TPFive.Game.Resource;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.ResourceLoader
{
    public class ResourceManager<T> : MonoBehaviour, IResourceManager
        where T : IDisposable
    {
        /// <summary>
        /// Mapping resource url to loader.
        /// Avoid multiple loaders load the same resource.
        /// </summary>
        private readonly Dictionary<string/*url*/, IResourceLoader> urlLoaderDict = new Dictionary<string, IResourceLoader>();

        /// <summary>
        /// Mapping resource loader to request.
        /// For manager dispatch resource to requesters when loader finish loading job.
        /// </summary>
        private readonly Dictionary<IResourceLoader, List<ResourceRequest<T>>> loaderRequestDict = new Dictionary<IResourceLoader, List<ResourceRequest<T>>>();

        /// <summary>
        /// Mapping resource url to loaded resource.
        /// Record all resources have been loaded in memory.
        /// </summary>
        private readonly Dictionary<string/*url*/, ResourceReferenceInfo<T>> loadedResourceDict = new Dictionary<string, ResourceReferenceInfo<T>>();

        private readonly List<IResourceLoader> waitingLoaderQueue = new List<IResourceLoader>();
        private readonly HashSet<IResourceLoader> loadingLoaderSet = new HashSet<IResourceLoader>();
        [Inject]
        private ILoggerFactory loggerFactory;
        private ILogger<ResourceManager<T>> logger;

        public ILogger Logger => logger ??= loggerFactory.CreateLogger<ResourceManager<T>>();

        protected virtual int MAX_LOADING_AMOUNT => 4;

        // TODO: refactor and using UniTask
        public void Load(ResourceRequest<T> resourceRequest)
        {
            if (string.IsNullOrEmpty(resourceRequest.Url))
            {
                return;
            }

            if (loadedResourceDict.TryGetValue(resourceRequest.Url, out ResourceReferenceInfo<T> info))
            {
                // [Resource is in memory]
                // Dispatch resource directly.
                resourceRequest.DispatchResource(info);
            }
            else
            {
                // [Resource is not in memory]
                CreateResourceLoader(resourceRequest);

                // After loader added, check loader queue to make waiting loader work
                ProcessPendingQueue();
            }
        }

        public void Abort(ResourceRequest<T> resourceRequest)
        {
            if (string.IsNullOrEmpty(resourceRequest.Url))
            {
                return;
            }

            if (!urlLoaderDict.TryGetValue(resourceRequest.Url, out IResourceLoader loader))
            {
                return;
            }

            var requestList = loaderRequestDict[loader];
            requestList.Remove(resourceRequest);

            // No request, remove loader.
            if (requestList.Count == 0)
            {
                loader.Abort();
                RemoveLoader(loader);

                // After loader removed, check loader queue to make waiting loader work
                ProcessPendingQueue();
            }
        }

        public void Release(string resourceUrl, object owner)
        {
            if (string.IsNullOrEmpty(resourceUrl) || owner == null)
            {
                return;
            }

            if (!loadedResourceDict.TryGetValue(resourceUrl, out var info))
            {
                return;
            }

            // Remove reference
            info.Release(owner);

            // Release resource from memory
            if (info.IsUnused)
            {
                info.UnloadResource();
                loadedResourceDict.Remove(resourceUrl);
            }
        }

        void IResourceManager.OnLoaderFinished(IResourceLoader loader)
        {
            // Wrap resource with reference count & add to loaded resource dictionary
            var resourceInfo = CreateResourceInfo(loader);

            // Dispatch resource
            if (loaderRequestDict.TryGetValue(loader, out var requestList))
            {
                foreach (var request in requestList)
                {
                    request.DispatchResource(resourceInfo);
                }
            }
            else
            {
                Logger.LogDebug($"No one request loaded resource. (url= {loader.Url})");
                resourceInfo.UnloadResource();
                loadedResourceDict.Remove(loader.Url);
            }

            // Remove loader
            RemoveLoader(loader);

            // After loader removed, check loader queue to make waiting loader work
            ProcessPendingQueue();
        }

        void IResourceManager.OnLoaderFailed(IResourceLoader loader)
        {
            // Simply showing message for failing loader and delegate to finished at this time.
            Logger.LogWarning($"{nameof(IResourceManager.OnLoaderFailed)} - loader: {loader} failed");
            ((IResourceManager)this).OnLoaderFinished(loader);
        }

        protected void OnDestroy()
        {
            // Stop all loaders
            foreach (var loader in urlLoaderDict.Values)
            {
                loader.Dispose();
            }

            urlLoaderDict.Clear();
            loaderRequestDict.Clear();
            waitingLoaderQueue.Clear();
            loadingLoaderSet.Clear();

            // Release all resources from memory
            foreach (var info in loadedResourceDict.Values)
            {
                info.UnloadResource();
            }

            loadedResourceDict.Clear();
        }

        protected void ProcessPendingQueue()
        {
            while (loadingLoaderSet.Count < MAX_LOADING_AMOUNT && waitingLoaderQueue.Count > 0)
            {
                IResourceLoader loader = waitingLoaderQueue[0];
                waitingLoaderQueue.RemoveAt(0);
                loadingLoaderSet.Add(loader);
                loader.Load();
            }
        }

        private T ConvertResource(object resource)
        {
            if (resource is T typedResource)
            {
                return typedResource;
            }

            try
            {
                return (T)Convert.ChangeType(resource, typeof(T));
            }
            catch (InvalidCastException e)
            {
                Logger.LogError("Failed to convert resource.", e);
            }

            return default;
        }

        private IResourceLoader CreateResourceLoader(ResourceRequest<T> request)
        {
            if (urlLoaderDict.TryGetValue(request.Url, out IResourceLoader loader))
            {
                // [Resource is loading via loader]
                // Map request with exist loader.
                loaderRequestDict[loader].Add(request);
            }
            else
            {
                // [No loader is loading resource]
                // Create a new loader to load resource.
                loader = ResourceLoaderFactory.Create<T>(this, request.Context, loggerFactory);
                if (loader == null)
                {
                    Logger.LogError($"Failed to create loader with type: {typeof(T).Name}");
                    return null;
                }

                // Mapping loader with url & request
                urlLoaderDict[request.Url] = loader;
                loaderRequestDict[loader] = new List<ResourceRequest<T>> { request };

                // Add loader to waiting queue
                waitingLoaderQueue.Add(loader);
            }

            return loader;
        }

        private void RemoveLoader(IResourceLoader loader)
        {
            // Remove loader from mapping dictionary
            urlLoaderDict.Remove(loader.Url);
            loaderRequestDict.Remove(loader);

            // Remove loader from loading set or waiting queue
            if (!loadingLoaderSet.Remove(loader))
            {
                waitingLoaderQueue.Remove(loader);
            }

            // Dispose loader
            loader.Dispose();
        }

        private ResourceReferenceInfo<T> CreateResourceInfo(IResourceLoader loader)
        {
            T resource = ConvertResource(loader.GetResource());

            var resourceInfo = new ResourceReferenceInfo<T>(loader.Url, resource);
            loadedResourceDict[loader.Url] = resourceInfo;
            return resourceInfo;
        }
    }
}
