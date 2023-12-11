using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.SCG.DisposePattern.Abstractions;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Video
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

        public UniTask<IVideoPlayer> CreateVideoPlayer(Transform parent = null)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);
            return serviceProvider.CreateVideoPlayer(parent);
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
