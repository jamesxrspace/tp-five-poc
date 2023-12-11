using System;
using System.Threading;
using System.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;

namespace TPFive.Game.App.Entry
{
    using TPFive.Game.Logging;

    using GameMessages = TPFive.Game.Messages;

    /// <summary>
    /// This part deals with message handling setup.
    /// </summary>
    public sealed partial class Bootstrap
    {
        private async Task SetupMessageHandling(CancellationToken cancellationToken = default)
        {
            _subSceneLoading
                .Subscribe(HandleSceneLoading())
                .AddTo(_compositeDisposable);

            _subSceneLoaded
                .Subscribe(HandleSceneLoaded())
                .AddTo(_compositeDisposable);

            _subSceneUnloading
                .Subscribe(HandleSceneUnloading())
                .AddTo(_compositeDisposable);

            _subSceneUnloaded
                .Subscribe(HandleSceneUnloaded())
                .AddTo(_compositeDisposable);
        }

        // Placing the method above does not make code readable.
        private static Action<GameMessages.SceneLoading> HandleSceneLoading()
        {
            return async x =>
            {
            };
        }

        private static Action<GameMessages.SceneUnloading> HandleSceneUnloading()
        {
            return async x =>
            {
            };
        }

        private static Action<GameMessages.SceneUnloaded> HandleSceneUnloaded()
        {
            return async x =>
            {
            };
        }

        private Action<GameMessages.SceneLoaded> HandleSceneLoaded()
        {
            return async x =>
            {
                if (string.IsNullOrEmpty(x.Category))
                {
                    Logger.LogDebug("Category is null or empty. Ignoring.");
                    return;
                }

                if (x.Category.Equals(_settings.category, StringComparison.Ordinal))
                {
                    // Receive message from self. Currently not dealing anything.
                }
                else
                {
                    Logger.LogEditorDebug(
                        "Receive {Message} from {Category}",
                        nameof(GameMessages.SceneLoaded),
                        x.Category);
                }
            };
        }
    }
}
