using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TPFive.Extended.InputXREvent;
using TPFive.Game;
using TPFive.Game.App.Entry;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.Config;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.Record;
using TPFive.Game.Resource;
using TPFive.Game.SceneFlow;
using TPFive.Game.User;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;
using IService = TPFive.Game.Config.IService;

namespace TPFive.Home.Entry.SocialLobby
{
    public class PlayerEntityViewModel : ViewModelBase, IEntity
    {
        private const int DefaultSortOrder = 0;
        private static readonly Vector3 DefaultPlayerEntitySize = new Vector3(1, 1, 1);
        private readonly IAvatarFactory avatarFactory;
        private readonly LifetimeScope lifetimeScope;
        private readonly InteractionRequest<bool> onVisibilityChangedRequest;
        private readonly SimpleCommand gotoReelSceneCommand;
        private readonly IPublisher<ChangeScene> pubSceneLoading;
        private readonly IPublisher<PostUnityMessage> pubUnityMessage;
        private readonly Settings appEntrySettings;
        private readonly CategoriesEnum[] categories;
        private readonly string xrid;
        private string nickname;
        private string thumbnailUrl;
        private bool isMe;
        private bool disposed;
        private bool isVisible;
        private bool isThumbnailLoading;
        private int sortOrder;
        private Vector3 size;
        private Vector3 localPosition;
        private Texture2D thumbnail;
        private IAvatarProfile avatarProfile;
        private CancellationTokenSource loadThumbnailCancellation;
        private CancellationTokenSource listReelsCancellation;
        private ILogger<PlayerEntityViewModel> logger;

        public PlayerEntityViewModel(
            IAvatarFactory avatarFactory,
            Settings appEntrySettings,
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<PostUnityMessage> pubUnityMessage,
            LifetimeScope lifetimeScope,
            string nickname,
            string xrid,
            CategoriesEnum[] categories)
            : this(
                size: DefaultPlayerEntitySize,
                sortOrder: DefaultSortOrder,
                avatarFormatJson: null,
                avatarFactory: avatarFactory,
                appEntrySettings: appEntrySettings,
                pubSceneLoading: pubSceneLoading,
                pubUnityMessage: pubUnityMessage,
                lifetimeScope: lifetimeScope,
                nickname: nickname,
                categories: categories)
        {
            this.xrid = xrid;
        }

        public PlayerEntityViewModel(
            Vector3 size,
            int sortOrder,
            string avatarFormatJson,
            IAvatarFactory avatarFactory,
            Settings appEntrySettings,
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<PostUnityMessage> pubUnityMessage,
            LifetimeScope lifetimeScope,
            string nickname,
            CategoriesEnum[] categories)
        {
            this.avatarFactory = avatarFactory;
            this.appEntrySettings = appEntrySettings;
            this.pubSceneLoading = pubSceneLoading;
            this.pubUnityMessage = pubUnityMessage;
            this.lifetimeScope = lifetimeScope;
            Size = size;
            SortOrder = sortOrder;
            AvatarFormatJson = avatarFormatJson;
            Nickname = nickname;
            this.categories = categories;
            onVisibilityChangedRequest = new InteractionRequest<bool>();
            gotoReelSceneCommand = new SimpleCommand(GotoReelScene);
        }

        ~PlayerEntityViewModel()
        {
            Dispose(false);
        }

        public IAvatarFactory AvatarFactory => avatarFactory;

        public ILogger<PlayerEntityViewModel> Logger => logger ??= LoggerFactory.CreateLogger<PlayerEntityViewModel>();

        public IAvatarProfile AvatarProfile
        {
            get => avatarProfile;
            set => avatarProfile = value;
        }

        public IInteractionRequest OnVisibilityChangedRequest => onVisibilityChangedRequest;

        public ICommand GotoReelSceneCommand => gotoReelSceneCommand;

        public ScheduledExecutor ScheduledExecutor { get; set; }

        public ILoggerFactory LoggerFactory { get; set; }

        public Transform EntityManipulationTransform { get; set; }

        public Rigidbody EntityManipulationRigidbody { get; set; }

        public PlatformInputSettings InputSetting { get; set; }

        public string ThumbnailUrl
        {
            get => thumbnailUrl;
            set => Set(ref thumbnailUrl, value);
        }

        public bool IsMe
        {
            get => isMe;
            set => Set(ref isMe, value);
        }

        public Vector3 LocalPosition
        {
            get => localPosition;
            set => Set(ref localPosition, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (Set(ref isVisible, value))
                {
                    onVisibilityChangedRequest.Raise(value);
                }
            }
        }

        public string AvatarFormatJson { get; }

        public string Xrid => xrid;

        public int SortOrder
        {
            get => sortOrder;
            set => Set(ref sortOrder, value);
        }

        public Vector3 Size
        {
            get => size;
            set => Set(ref size, value);
        }

        public string Nickname
        {
            get => nickname;
            set => Set(ref nickname, value);
        }

        public Texture2D Thumbnail
        {
            get => thumbnail;
            set => Set(ref thumbnail, value);
        }

        public CategoriesEnum[] Categories => categories;

        public string ReelId { get; set; }

        public IService ConfigService { get; set; }

        public IReelApi ReelApi { get; set; }

        public Game.Resource.IService ResourceService { get; set; }

        public Game.SceneFlow.IService SceneFlowService { get; set; }

        public async UniTaskVoid LoadThumbnail()
        {
            if (string.IsNullOrEmpty(thumbnailUrl) || isThumbnailLoading || Thumbnail != null)
            {
                return;
            }

            if (Thumbnail != null)
            {
                return;
            }

            isThumbnailLoading = true;

            if (loadThumbnailCancellation != null)
            {
                loadThumbnailCancellation.Cancel();
                loadThumbnailCancellation.Dispose();
            }

            loadThumbnailCancellation = new CancellationTokenSource();

            try
            {
                var result = await ResourceService.LoadTexture(
                    this,
                    new TextureRequestContext()
                    {
                        MaxRetry = 3,
                        Url = thumbnailUrl,
                    },
                    loadThumbnailCancellation.Token);

                Thumbnail = result.Texture;
            }
            finally
            {
                isThumbnailLoading = false;
            }
        }

        public void ReleaseThumbnail()
        {
            if (loadThumbnailCancellation != null)
            {
                loadThumbnailCancellation.Cancel();
                loadThumbnailCancellation.Dispose();
                loadThumbnailCancellation = null;
            }

            if (Thumbnail != null)
            {
                ResourceService.ReleaseTexture(thumbnailUrl, this);
            }
        }

        public void InitializeVisibility()
        {
            onVisibilityChangedRequest.Raise(IsVisible);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                ReleaseThumbnail();

                if (listReelsCancellation != null)
                {
                    listReelsCancellation.Cancel();
                    listReelsCancellation.Dispose();
                }
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private void GotoReelScene()
        {
            if (listReelsCancellation != null)
            {
                listReelsCancellation.Cancel();
                listReelsCancellation.Dispose();
            }

            listReelsCancellation = new CancellationTokenSource();
            GotoReelSceneAsync(listReelsCancellation.Token).Forget();
        }

        private async UniTaskVoid GotoReelSceneAsync(CancellationToken token)
        {
            try
            {
                pubUnityMessage.Publish(new PostUnityMessage()
                {
                    UnityMessage = new Model.UnityMessage()
                    {
                        Type = Model.UnityMessageTypeEnum.ShowLoading,
                        Data = JsonConvert.SerializeObject(
                            new Dictionary<string, object>
                            {
                                { "alpha", 1f },
                            }),
                    },
                });

                gotoReelSceneCommand.Enabled = false;

                var (isCanceled, reel) = await QueryOneReelContextAsync(ReelId, token)
                    .SuppressCancellationThrow();

                if (isCanceled || reel == null)
                {
                    return;
                }

                var entryParam = new ReelSceneEntryParameter()
                {
                    Entry = ReelSceneEntryParameter.EntryType.Browse,
                    ReelUrl = reel.Xrs,
                    Reel = reel,
                };

                ConfigService.SetSystemObjectValue(
                    Constants.RuntimeLocalProviderKind,
                    nameof(ReelSceneEntryParameter),
                    entryParam);

#if OCULUS_VR
                await UnloadVRHomeScene();
#endif

                var fromEntryProperty = GetSceneProperty("HomeEntry");
                var toEntryProperty = GetSceneProperty("RecordEntry");

                pubSceneLoading.Publish(
                    new ChangeScene()
                    {
                        FromCategory = fromEntryProperty.category,
                        FromTitle = fromEntryProperty.addressableKey,
                        FromCategoryOrder = fromEntryProperty.categoryOrder,
                        FromSubOrder = fromEntryProperty.subOrder,
                        ToCategory = toEntryProperty.category,
                        ToTitle = toEntryProperty.addressableKey,
                        ToCategoryOrder = toEntryProperty.categoryOrder,
                        ToSubOrder = toEntryProperty.subOrder,
                        LifetimeScope = lifetimeScope,
                    });
            }
            catch (Exception e)
            {
                Logger.LogWarning($"{nameof(GotoReelSceneAsync)} failed, {e}");
                pubUnityMessage.Publish(new PostUnityMessage()
                {
                    UnityMessage = new Model.UnityMessage()
                    {
                        Type = Model.UnityMessageTypeEnum.HideLoading,
                        Data = string.Empty,
                    },
                });
            }
            finally
            {
                gotoReelSceneCommand.Enabled = true;
            }
        }

        private async UniTask<Reel> QueryOneReelContextAsync(
            string reelId,
            CancellationToken cancellationToken)
        {
            var response = await ReelApi.ListReelsAsync(size: 1, reelId: reelId, cancellationToken: cancellationToken);

            if (!response.IsSuccess)
            {
                Logger.LogWarning(
                    $"{nameof(GotoReelSceneAsync)}.{nameof(ReelApi.ListReelsAsync)} failed, {response}");
                return default;
            }

            return response.Data?.Items?.FirstOrDefault();
        }

        private async UniTask UnloadVRHomeScene()
        {
            if (!appEntrySettings.TryGetSceneProperty("Home", out var property))
            {
                return;
            }

            await SceneFlowService.UnloadSceneAsync(
                $"{property.addressableKey}.asset",
                property.categoryOrder,
                property.subOrder,
                loadFromScriptableObject: true);
            await ResourceService.UnloadBundleDataAsync(property.addressableKey);
        }

        private SceneProperty GetSceneProperty(string title)
        {
            if (appEntrySettings.TryGetSceneProperty(title, out var property))
            {
                return property;
            }

            return default;
        }
    }
}