using Microsoft.Extensions.Logging;
using TPFive.SCG.DisposePattern.Abstractions;
using UniRx;
using VContainer;

namespace TPFive.Game.Record.Scene
{
    [Dispose]
    public partial class Service : IService
    {
        private readonly ILogger log;
        private readonly ReactiveProperty<ReelSceneInfo> sceneInfo;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [Inject]
        public Service(ILoggerFactory loggerFactory)
        {
            this.log = loggerFactory.CreateLogger<Service>();
            this.sceneInfo = new ReactiveProperty<ReelSceneInfo>();
            this.sceneInfo.Subscribe(OnSceneChanged).AddTo(disposables);
        }

        public IReactiveProperty<ReelSceneInfo> SceneInfo => sceneInfo;

        private void OnSceneChanged(ReelSceneInfo info)
        {
            log.LogDebug("{Method}: info: {@info}", nameof(OnSceneChanged), info);
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                disposables.Dispose();
            }

            _disposed = true;
        }
    }
}
