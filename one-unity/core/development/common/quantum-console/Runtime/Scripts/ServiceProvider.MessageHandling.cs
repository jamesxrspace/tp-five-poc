using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using TPFive.Game.Logging;
using UniRx;

namespace TPFive.Extended.QuantumConsole
{
    public sealed partial class ServiceProvider
    {
        private async UniTask SetupMessageHandling(CancellationToken cancellationToken = default)
        {
            _subAssistMode
                .Subscribe(x =>
                {
                    Logger.LogEditorDebug(
                        "{Method} AssistMode: {AssistModeOn}",
                        nameof(SetupMessageHandling),
                        x.On);
                })
                .AddTo(_compositeDisposable);

            await UniTask.CompletedTask;
        }
    }
}
