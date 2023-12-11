using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.Messages;
using TPFive.Game.Reel;
using TPFive.Model;
using TPFive.OpenApi.GameServer;
using TPFive.SCG.DisposePattern.Abstractions;
using UniRx;
using VContainer;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace TPFive.Game.Record.Entry
{
    [Dispose]
    public partial class Bootstrap : IAsyncStartable
    {
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        private readonly ILogger log;
        private readonly ILoggerFactory loggerFactory;
        private readonly UI.IService uiService;
        private readonly Game.Config.IService configService;
        private readonly ISubscriber<ContentLevelFullyLoaded> subContentLoaded;
        private readonly ISubscriber<FlutterMessage> subFlutterMessage;
        private readonly IPublisher<PostUnityMessage> pubUnityMessage;
        private readonly MusicToMotionGenerator musicToMotionGenerator;
        private readonly ReelManager reelManager;
        private ReelWindowFlutterMessenger reelWindowFlutterMessenger;
        private UnityMessageTypeEnum unityMessageTypeEnum;
        private UniTaskCompletionSource contentLoadedTask = new ();

        [Inject]
        public Bootstrap(
            ILoggerFactory loggerFactory,
            UI.IService uiService,
            Config.IService configService,
            ReelManager reelManager,
            ISubscriber<ContentLevelFullyLoaded> subContentLoaded,
            ISubscriber<FlutterMessage> subFlutterMessage,
            IPublisher<PostUnityMessage> pubUnityMessage,
            IAssetApi assetApi,
            MusicToMotionService musicToMotionService)
        {
            log = loggerFactory.CreateLogger<Bootstrap>();

            this.loggerFactory = loggerFactory;
            this.uiService = uiService;
            this.configService = configService;
            this.reelManager = reelManager;
            this.subContentLoaded = subContentLoaded;
            this.subFlutterMessage = subFlutterMessage;
            this.pubUnityMessage = pubUnityMessage;

            subContentLoaded.Subscribe(OnContentLoaded).AddTo(compositeDisposable);
            var assetAccessHelper = new AssetAccessHelper(loggerFactory, assetApi);
            musicToMotionGenerator = new MusicToMotionGenerator(loggerFactory, assetAccessHelper, musicToMotionService);
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                pubUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(StartAsync)}");

            var (isGetSuccess, entryParamObj) = configService.GetSystemObjectValue(nameof(ReelSceneEntryParameter));
            if (!isGetSuccess)
            {
                log.LogError(
                    "{Method}: Failed to get {EntryParameter} from config service",
                    nameof(StartAsync),
                    nameof(ReelSceneEntryParameter));

                throw new Exception($"Failed to get '{nameof(ReelSceneEntryParameter)}' from config service");
            }

            configService.RemoveSystemObjectValue(Config.Constants.RuntimeLocalProviderKind, nameof(ReelSceneEntryParameter));

            if (entryParamObj is not ReelSceneEntryParameter entryParam)
            {
                log.LogError(
                    "{Method}: The entry parameter type is not {EntryParameter} type",
                    nameof(StartAsync),
                    nameof(ReelSceneEntryParameter));

                throw new Exception($"The entry parameter type is not '{nameof(ReelSceneEntryParameter)}' type");
            }

            log.LogDebug(
                "{Method}: entry parameter: {EntryParameter}",
                nameof(StartAsync),
                entryParam);

            switch (entryParam.Entry)
            {
                case ReelSceneEntryParameter.EntryType.Browse:
                    {
                        var setupOption = new ReelSetupOption { SceneDesc = entryParam.SceneDesc, ReelUrl = entryParam.ReelUrl };
                        await reelManager.Setup(setupOption);
                        if (!string.IsNullOrEmpty(entryParam.ReelUrl))
                        {
                            await reelManager.StartSession(SessionStartOption.Watch());
                        }

                        await ShowReelView(entryParam.Reel, cancellationToken);
                        unityMessageTypeEnum = UnityMessageTypeEnum.SwitchedToCocreatePage;
                        break;
                    }

                case ReelSceneEntryParameter.EntryType.Create:
                    {
                        var setupOption = new ReelSetupOption { ShowUserAvatar = true, SceneDesc = entryParam.SceneDesc };
                        await reelManager.Setup(setupOption);
                        await ShowRecordView(cancellationToken);
                        unityMessageTypeEnum = UnityMessageTypeEnum.SwitchedToReelPage;
                        break;
                    }

                default:
                    throw new NotSupportedException($"Unknown ReelSceneEntryParameter.EntryType: {entryParam.Entry})");
            }

            await contentLoadedTask.Task;
            pubUnityMessage.Publish(new PostUnityMessage()
            {
                UnityMessage = new UnityMessage()
                {
                    Type = unityMessageTypeEnum,
                    Data = entryParam.Reel?.ToString() ?? string.Empty,
                    SessionId = string.Empty,
                },
            });

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                pubUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(StartAsync)}");
        }

        private void OnContentLoaded(ContentLevelFullyLoaded data)
        {
            log.LogDebug("OnContentLoaded");
            contentLoadedTask.TrySetResult();
        }

        private void CreateFlutterMessenger(bool isRecord)
        {
            reelWindowFlutterMessenger = new ReelWindowFlutterMessenger(
                log,
                reelManager,
                musicToMotionGenerator,
                isRecord,
                subFlutterMessage,
                pubUnityMessage);
        }

        private async UniTask ShowRecordView(CancellationToken cancellationToken = default)
        {
            CreateFlutterMessenger(true);

#if UNITY_EDITOR
            var (isCanceled, window) = await uiService.ShowWindow<RecordWindow>()
                .AttachExternalCancellation(cancellationToken)
                .SuppressCancellationThrow();

            if (isCanceled)
            {
                if (window != null)
                {
                    _ = window.Dismiss(true);
                }

                return;
            }

            window.SetDataContext(new RecordWindowViewModel(log, reelWindowFlutterMessenger));
#endif
        }

        private async UniTask ShowReelView(OpenApi.GameServer.Model.Reel reel, CancellationToken cancellationToken)
        {
            CreateFlutterMessenger(false);

#if UNITY_EDITOR
            var (isCanceled, window) = await uiService.ShowWindow<ReelSelectWindow>()
                .AttachExternalCancellation(cancellationToken)
                .SuppressCancellationThrow();

            if (isCanceled)
            {
                if (window != null)
                {
                    _ = window.Dismiss(true);
                }

                return;
            }

            var vm = new ReelSelectWindowViewModel(log, reelManager, reelWindowFlutterMessenger, reel);
            window.SetDataContext(vm);
#endif
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }

            reelWindowFlutterMessenger?.Dispose();
            reelWindowFlutterMessenger = null;
            reelWindowFlutterMessenger?.Dispose();
            reelWindowFlutterMessenger = null;
            compositeDisposable?.Dispose();

            _disposed = true;
        }
    }
}
