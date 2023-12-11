using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;

namespace TPFive.Game.Interactable.Toolkit
{
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using CrossBridge = TPFive.Cross.Bridge;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    /// <summary>
    /// This Service is a facade pattern for all interactable components to use each provider API.
    /// </summary>
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Interactable.Toolkit.IServiceProvider))]
    [RegisterToContainer]
    public partial class Service : IService
    {
        /// <summary>
        /// It is the index of the SnapZoneServiceProvider in the service provider table for get or set.
        /// </summary>
        public const int SnapZoneServiceProviderIndex = 1;
        private const int NullServiceProviderIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// This service is made for manage all service provider.
        /// </summary>
        /// <param name="loggerFactory">The nullServiceProvide need loggerFactory for log.</param>
        [Inject]
        public Service(ILoggerFactory loggerFactory)
        {
            Cross.Bridge.TrySnapObject = this.TrySnapObject;
            Cross.Bridge.TryReleaseSnappedObject = this.TryReleaseSnappedObject;

            var logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            NullServiceProvider = new NullServiceProvider((s, args) =>
                                                          {
                                                              logger.LogDebug(s, args);
                                                          });

            GetNullServiceProvider = NullServiceProvider;

            _serviceProviderTable.Add(NullServiceProviderIndex, NullServiceProvider);
        }

        /// <summary>
        /// Gets NullServiceProvider is according to the Service architecture of the project to avoid NullException.
        /// </summary>
        public IServiceProvider NullServiceProvider { get; }

        /// <inheritdoc/>
        [DelegateFrom(DelegateName = nameof(TrySnapObject))]
        public bool TrySnapObject(Collider other, DG.Tweening.DOTweenAnimation animation)
        {
            var serviceProvider = GetServiceProvider(SnapZoneServiceProviderIndex);
            return serviceProvider.TrySnapObject(other, animation);
        }

        /// <inheritdoc/>
        [DelegateFrom(DelegateName = nameof(TryReleaseSnappedObject))]
        public bool TryReleaseSnappedObject(Collider other)
        {
            var serviceProvider = GetServiceProvider(SnapZoneServiceProviderIndex);
            return serviceProvider.TryReleaseSnappedObject(other);
        }

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
            }
        }
    }
}