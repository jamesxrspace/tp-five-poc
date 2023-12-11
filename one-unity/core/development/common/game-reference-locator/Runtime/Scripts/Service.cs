using Microsoft.Extensions.Logging;
using TPFive.SCG.DisposePattern.Abstractions;
using TPFive.SCG.ServiceEco.Abstractions;
using UniRx;
using VContainer;
using VContainer.Unity;
using GameLoggingUtility = TPFive.Game.Logging.Utility;

namespace TPFive.Game.ReferenceLocator
{
    [Dispose]
    [RegisterToContainer(Scope = 2)]
    public sealed partial class Service :
        IService
    {
        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly LifetimeScope _lifetimeScope;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);
            _lifetimeScope = lifetimeScope;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public TService GetInstance<TService>()
        {
            return _lifetimeScope.Container.Resolve<TService>();
        }

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Dispose();
                _disposed = true;
            }
        }
    }
}