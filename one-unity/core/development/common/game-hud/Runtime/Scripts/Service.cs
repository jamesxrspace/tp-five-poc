using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Hud
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using TPFive.Game.Logging;

    //
    using CrossBridge = TPFive.Cross.Bridge;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    // The order of attributes is not important
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Hud.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        private readonly LifetimeScope _lifetimeScope;

        [Inject]
        public Service(//
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            //
            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            //
            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);

            // GetNullServiceProvider = new NullServiceProvider();
            //
            CrossBridge.ShowChatbotDialogue = ShowChatbotDialogue;
            CrossBridge.ShowMessageBoardWindow = ShowMessageBoardWindow;
            CrossBridge.ShowMultiButtonDialogue = ShowMultiButtonDialogue;
            CrossBridge.ShowRedeemDialogue = ShowRedeemDialogue;
            CrossBridge.ShowToast = ShowToast;
            CrossBridge.ShowTutorialDialogue = ShowTutorialDialogue;
            CrossBridge.ShowVoiceMessageBoardWindow = ShowVoiceMessageBoardWindow;
            CrossBridge.ShowVoiceRecordingWindow = ShowVoiceRecordingWindow;
        }

        //
        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

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

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Dispose();

                _utcs.TrySetCanceled(_cancellationTokenSource.Token);
                _utcs = default;
                _disposed = true;
            }
        }

        //
        public void ShowHud(GeneralContext generalContext)
        {
            var serviceProvider = GetServiceProvider(10);

            serviceProvider.ShowHud(generalContext);
        }

        //
        private void ShowChatbotDialogue(string name)
        {
            ShowHud(new GeneralContext
            {

            });
        }

        private void ShowMessageBoardWindow(string name)
        {
            ShowHud(new GeneralContext
            {

            });
        }

        private void ShowMultiButtonDialogue(string name)
        {
            ShowHud(new GeneralContext
            {

            });
        }

        private void ShowRedeemDialogue(string name)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(ShowRedeemDialogue));

            ShowHud(new GeneralContext
            {
                Name = "RedeemDialogue"
            });
        }

        private void ShowToast(string name)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(ShowToast));

            ShowHud(new GeneralContext
            {

            });
        }

        private void ShowTutorialDialogue(string name)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(ShowTutorialDialogue));

            ShowHud(new GeneralContext
            {
                Name = "TutorialDialogue"
            });
        }

        private void ShowVoiceMessageBoardWindow(string name)
        {
            ShowHud(new GeneralContext
            {

            });
        }

        private void ShowVoiceRecordingWindow(string name)
        {
            ShowHud(new GeneralContext
            {

            });
        }
    }
}
