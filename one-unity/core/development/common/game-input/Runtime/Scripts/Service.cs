using Microsoft.Extensions.Logging;
using UnityEngine.InputSystem;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using TPFive.SCG.ServiceEco.Abstractions;

namespace TPFive.Game.Input
{
    [ServiceProviderManagement]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private const int ExtendedProviderIndex = (int)ServiceProviderKind.Rank1ServiceProvider;

        private readonly ILogger log;

        private bool isDisposed;

        [Inject]
        public Service(ILoggerFactory loggerFactory)
        {
            this.log = loggerFactory.CreateLogger<Service>();

            var nullServiceProvider = new NullServiceProvider((msg, args) =>
            {
                log.LogDebug(msg, args);
            });

            _serviceProviderTable.Add((int)ServiceProviderKind.NullServiceProvider, nullServiceProvider);
        }

        public IServiceProvider NullServiceProvider
        {
            get => GetServiceProvider((int)ServiceProviderKind.NullServiceProvider);
        }

        public void RegisterInputActionAsset(InputActionAsset inputActionAsset)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            serviceProvider.RegisterInputActionAsset(inputActionAsset);
        }

        public void UnregisterInputActionAsset(InputActionAsset inputActionAsset)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            serviceProvider.RegisterInputActionAsset(inputActionAsset);
        }

        public bool TryGetInputAction(string actionNameOrId, out InputAction inputAction)
        {
            var serviceProvider = GetServiceProvider(ExtendedProviderIndex);

            return serviceProvider.TryGetInputAction(actionNameOrId, out inputAction);
        }
    }
}
