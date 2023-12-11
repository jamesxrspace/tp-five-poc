using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Logging;
using UniRx;
using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.EventRepo
{
    public partial class ServiceProvider
    {
        private async UniTask SetupMessageHandling(CancellationToken cancellationToken)
        {
            _subMarkerMessage
                .Subscribe(x =>
                {
                    Logger.LogDebug("{Message}", x);
                    EventBus.Trigger(
                        new EventHook("OnMarkerMessage"),
                        (x.IntParams, x.FloatParams));
                })
                .AddTo(_compositeDisposable);

            _subHudMessage
                .Subscribe(x =>
                {
                    Logger.LogDebug("{Message}", x);
                    EventBus.Trigger(
                        new EventHook("OnHudMessage"),
                        (x.IntParams, x.FloatParams, x.StringParams, x.GameObjectParams));
                })
                .AddTo(_compositeDisposable);

            _subBackToHome
                .Subscribe(x =>
                {
                    Logger.LogEditorDebug("{Method} - Sending OnBackToHome to VS", nameof(SetupMessageHandling));
                    // Event in visual scripting mainly as string form, will group them gradually.
                    EventBus.Trigger(
                        new EventHook("OnBackToHome"));
                })
                .AddTo(_compositeDisposable);

            await UniTask.CompletedTask;
        }
    }
}
