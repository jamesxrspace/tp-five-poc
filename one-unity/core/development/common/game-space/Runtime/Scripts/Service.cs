using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VContainer;

namespace TPFive.Game.Space
{
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    // The order of attributes is not important
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Space.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private const int extendedSpaceProviderIndex = (int)ServiceProviderKind.Rank1ServiceProvider;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add((int)ServiceProviderKind.NullServiceProvider, nullServiceProvider);
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[(int)ServiceProviderKind.NullServiceProvider] as IServiceProvider;

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
            }
        }

        public UniTask<List<SpaceGroup>> GetSpaceGroups(CancellationToken cancellationToken)
        {
            var serviceProvider = GetServiceProvider(extendedSpaceProviderIndex);

            return serviceProvider.GetSpaceGroups(cancellationToken);
        }

        public UniTask<List<Space>> GetSpaces(string spaceGroupId, CancellationToken cancellationToken)
        {
            var serviceProvider = GetServiceProvider(extendedSpaceProviderIndex);

            return serviceProvider.GetSpaces(spaceGroupId, cancellationToken);
        }
    }
}
