// TODO: This pare might be great to be source code generated

using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace TPFive.Creator.MessageRepo
{
    using TPFive.Game.Logging;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    // This null service provider is not auto generated, because creator part is trying to reduce
    // dependency on main game part.
    // This decision might be changed later if more creator related services are defined.
    public partial class NullServiceProvider :
        IServiceProvider,
        IAsyncStartable,
        System.IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new ();

        private bool _disposed = false;

        public NullServiceProvider(
            ILoggerFactory loggerFactory)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<NullServiceProvider>(loggerFactory);
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(StartAsync));

            await UniTask.CompletedTask;
        }

        public void PublishMessage(string name, string stringParam)
        {
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
    }
}
