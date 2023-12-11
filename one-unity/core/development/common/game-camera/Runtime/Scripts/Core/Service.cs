using System;
using Microsoft.Extensions.Logging;
using TPFive.SCG.DisposePattern.Abstractions;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Camera
{
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private const int ExtendedProviderIndex = (int)ServiceProviderKind.Rank1ServiceProvider;

        private readonly ILogger log;

        [Inject]
        public Service(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<Service>();

            var nullServiceProvider = new NullServiceProvider((msg, args) =>
            {
                log.LogDebug(msg, args);
            });
            _serviceProviderTable.Add(0, nullServiceProvider);
        }

        public IServiceProvider NullServiceProvider
        {
            get => GetServiceProvider((int)ServiceProviderKind.NullServiceProvider);
        }

        public ICamera GetLiveCamera()
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            return serviceProvider.GetLiveCamera();
        }

        public void SetLiveCamera(string cameraName)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            serviceProvider.SetLiveCamera(cameraName);
        }

        public void UnsetLiveCamera()
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            serviceProvider.UnsetLiveCamera();
        }

        public void AddCamera(ICamera camera)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            serviceProvider.AddCamera(camera);
        }

        public void RemoveCamera(ICamera camera)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            serviceProvider.RemoveCamera(camera);
        }

        public bool ContainsCamera(ICamera camera)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            return serviceProvider.ContainsCamera(camera);
        }

        public bool ContainsCamera(string cameraName)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            return serviceProvider.ContainsCamera(cameraName);
        }

        public bool TryGetCamera(string cameraName, out ICamera camera)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            return serviceProvider.TryGetCamera(cameraName, out camera);
        }

        public bool IsTargetInView(Transform target)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            return serviceProvider.IsTargetInView(target);
        }

        /// <summary>
        /// Handle IDisposable interface.
        /// </summary>
        /// <param name="disposing">disposing or not.</param>
        /// <seealso cref="Dispose"/>
        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var value in _serviceProviderTable.Values)
                {
                    if (value is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            _disposed = true;
        }
    }
}
