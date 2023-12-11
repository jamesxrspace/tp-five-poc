using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TPFive.Extended.Addressable
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    using GameConfig = TPFive.Game.Config;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameMessages = TPFive.Game.Messages;
    using GameResource = TPFive.Game.Resource;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider
    {
        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly GameConfig.IService _configService;

        private readonly GameResource.IServiceProvider _nullServiceProvider;

        private readonly IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;
        private readonly IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> _asyncSubMultiPhaseSetupDone;

        private UniTaskCompletionSource<bool> _utcs = new ();
        private string _addressableUrl;

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            GameConfig.IService configService,
            GameResource.IServiceProvider nullServiceProvider,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage,
            IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> asyncSubMultiPhaseSetupDone)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _configService = configService;

            _nullServiceProvider = nullServiceProvider;

            _pubPostUnityMessage = pubPostUnityMessage;
            _asyncSubMultiPhaseSetupDone = asyncSubMultiPhaseSetupDone;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        private static string TransformInternalId(string internalId, string addressableUrl)
        {
            var keyPrefixPath = "PrefixPath";
            var valuePrefixPath = addressableUrl;
            var keyIntermediatePath = "IntermediatePath";
            var valueIntermediatePath = "latest";

            AddressablesRuntimeProperties.SetPropertyValue(keyPrefixPath, valuePrefixPath);
            AddressablesRuntimeProperties.SetPropertyValue(keyIntermediatePath, valueIntermediatePath);

            var evaluated = AddressablesRuntimeProperties.EvaluateString(
                internalId,
                '«',
                '»',
                AddressablesRuntimeProperties.EvaluateProperty);

            var adjustedId = evaluated.Replace("\\", "/");
            return adjustedId;
        }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            // Ids inside catalog have to be resolved before they are used. But, to resolve them,
            // some service has to be set beforehand.
            Addressables.InternalIdTransformFunc = InternalIdTransformFunc;

            _asyncSubMultiPhaseSetupDone
                ?.Subscribe(async (x, ct) =>
                {
                    if (x.Phase.Equals("SetupFinished", StringComparison.Ordinal) &&
                        x.Category.Equals("ConfigService", StringComparison.Ordinal) &&
                        x.Success)
                    {
                        Logger.LogEditorDebug(
                            "{Method} - ConfigService setup done",
                            nameof(SetupBegin));

                        var (r, addressableUrl) = await _configService.GetStringValueAsync("AddressableUrl");
                        if (r)
                        {
                            _addressableUrl = addressableUrl;
                        }
                        else
                        {
                            Logger.LogWarning("{Method} - AddressableUrl is not set", nameof(SetupBegin));
                        }

                        await SetupAddressables(cancellationToken);
                    }
                })
                .AddTo(_compositeDisposable);
        }

        private string InternalIdTransformFunc(IResourceLocation arg)
        {
            return TransformInternalId(arg.InternalId, _addressableUrl);
        }

        private async UniTask SetupEnd(
            bool success,
            CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method} reaches finally block",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            System.Exception e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);

            await UniTask.CompletedTask;
        }

        private void HandleDispose(bool disposing)
        {
            Logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

            if (disposing)
            {
                _compositeDisposable?.Dispose();
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}
