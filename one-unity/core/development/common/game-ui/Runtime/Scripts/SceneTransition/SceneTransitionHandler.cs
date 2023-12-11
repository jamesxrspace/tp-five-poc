using System;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;

namespace TPFive.Game.UI
{
    public class SceneTransitionHandler : IDisposable
    {
        // focus on the category of game level scene.
        private const int GameLevelCategory = 4;
        private readonly ISubscriber<Messages.SceneLoading> _subSceneLoading;
        private readonly ISubscriber<Messages.SceneLoaded> _subSceneLoaded;
        private readonly CompositeDisposable _disposables;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Microsoft.Extensions.Logging.ILogger _log;
        private ISceneTransition _sceneTransition;

        public SceneTransitionHandler(
            ISubscriber<Messages.SceneLoading> subSceneLoading,
            ISubscriber<Messages.SceneLoaded> subSceneLoaded,
            ISceneTransition sceneTransition,
            ILoggerFactory loggerFactory)
        {
            _subSceneLoading = subSceneLoading;
            _subSceneLoaded = subSceneLoaded;
            _sceneTransition = sceneTransition;
            _disposables = new CompositeDisposable();
            _loggerFactory = loggerFactory;
            _log = Logging.Utility.CreateLogger<SceneTransitionHandler>(loggerFactory);

            var onSceneLoading = this._subSceneLoading.Subscribe(handler =>
            {
                if (handler.CategoryOrder != GameLevelCategory)
                {
                    return;
                }

                _log.LogDebug($"Start FadeIn when SceneLoading. By message={handler}");

                PrepareVFX();
                Fadein();
            });

            var onSceneLoaded = this._subSceneLoaded.Subscribe(handler =>
            {
                if (handler.CategoryOrder != GameLevelCategory)
                {
                    return;
                }

                _log.LogDebug($"Start FadeOut when SceneLoaded. By message={handler}");

                PrepareVFX();
                Fadeout();
            });

            _disposables.Add(onSceneLoading);
            _disposables.Add(onSceneLoaded);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _sceneTransition = null;
        }

        private void Fadein()
        {
            if (_sceneTransition != null)
            {
                _sceneTransition.FadeIn();
            }
        }

        private void Fadeout()
        {
            if (_sceneTransition != null)
            {
                _sceneTransition.FadeOut();
            }
        }

        private void PrepareVFX()
        {
            if (_sceneTransition != null)
            {
                _sceneTransition.Prepare();
            }
        }
    }
}