using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using VContainer.Unity;

namespace TPFive.Creator.MessageRepo
{
    using TPFive.Game.Logging;

    using VContainer;
    using CrossBridge = TPFive.Cross.Bridge;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    public sealed partial class Service :
        TPFive.Game.ServiceBase,
        IAsyncStartable,
        IService,
        System.IDisposable
    {
        private const int VisualScriptingEventRepoServiceProvider = (int)Game.ServiceProviderKind.Rank1ServiceProvider;

        private readonly CompositeDisposable _compositeDisposable = new ();

        private bool _disposed = false;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            Cross.Bridge.PublishMessage = PublishMessage;
        }

        public IServiceProvider NullServiceProvider => GetNullServiceProvider as IServiceProvider;

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(StartAsync));

            var utcs = new UniTaskCompletionSource<bool>();
            var success = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                success = true;
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogDebug("{Exception}", e);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }
            finally
            {
                Logger.LogEditorDebug(
                    "{Method} reaches finally block",
                    nameof(StartAsync));

                utcs.TrySetResult(success);
            }

            await UniTask.CompletedTask;
        }

        public void PublishMessage(string name, string stringParam)
        {
            var serviceProvider = GetSpecificServiceProvider(VisualScriptingEventRepoServiceProvider);

            serviceProvider.PublishMessage(name, stringParam);
        }

        public void Dispose()
        {
            HandleDispose(true);
            System.GC.SuppressFinalize(this);
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _compositeDisposable?.Dispose();
                _disposed = true;
            }
        }

        // TODO: These two methods need to refactor
        // TODO: Might be great to extract as utility function
        private IServiceProvider GetSpecificServiceProvider(int index)
        {
            var nullServiceProvider = GetNullServiceProvider as IServiceProvider;
            var result = _serviceProviderTable.TryGetValue(index, out var sp);
            var serviceProvider = ConvertToServiceProvider(sp);

            if (!result || serviceProvider == null)
            {
                serviceProvider = nullServiceProvider;
            }

            return serviceProvider ?? (GetNullServiceProvider as IServiceProvider);
        }
    }
}
