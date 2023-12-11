using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Actor
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    //
    using TPFive.Game.Logging;

    //
    using CrossBridge = TPFive.Cross.Bridge;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Actor.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        [Inject]
        public Service(
            //
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            // _serviceProviderTable.TryAdd(0, new NullServiceProvider(loggerFactory));

            //
            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            //
            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);

            //
            CrossBridge.TeleportActor = TeleportTo;
            CrossBridge.MoveActorTo = MoveTo;
        }

        //
        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        //
        // public TPFive.Game.IServiceProvider GetNullServiceProvider => _serviceProviderTable[0];
        // public IServiceProvider NullServiceProvider => GetNullServiceProvider as IServiceProvider;

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e, CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);
        }

        private async Task HandleStartAsyncException(
            System.Exception e, CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
        }

        //
        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Dispose();
            }
        }

        //
        public bool TeleportTo(GameObject actor, Vector3 position)
        {
            var serviceProvider = GetServiceProvider(10);

            return serviceProvider.TeleportTo(actor, position);
        }

        public void MoveTo(GameObject actor, Vector3 position)
        {
            var serviceProvider = GetServiceProvider(10);

            serviceProvider.MoveTo(actor, position);
        }
    }
}
