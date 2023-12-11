using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TPFive.Extended.InputXREvent;
using TPFive.Game.App.Entry;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.SceneFlow;
using TPFive.Game.User;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IService = TPFive.Game.User.IService;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPFive.Home.Entry.SocialLobby
{
    public class SocialLobbyManagerMobile : MonoBehaviour
    {
        [SerializeField]
        private EntitySpawner entitySpawner;
        [SerializeField]
        private CameraManager cameraManager;
        [FormerlySerializedAs("panDetector")]
        [SerializeField]
        private PanEventDispatcher panEventDispatcher;
        [SerializeField]
        private Transform entityManipulationTransform;
        [SerializeField]
        private Rigidbody entityManipulationRigidbody;
        [SerializeField]
        private CategoriesEnum[] defaultFeedTagOfMe;
        private List<IEntity> cachedEntities;

        [Header("Debug-MockData")]
        [SerializeField]
        private List<MockEntityDataConfig> mockDataConfigs;
        [SerializeField]
        [Multiline]
        private string[] avatarFormatList;
        [SerializeField]
        private TextAsset avatarFormats;
        private IAvatarFactory avatarFactory;
        private ScheduledExecutor scheduledExecutor;
        private ILoggerFactory loggerFactory;
        private ILogger logger;
        private PlatformInputSettings inputSetting;
        private Settings appEntrySettings;
        private LifetimeScope lifetimeScope;
        private IPublisher<ChangeScene> pubSceneLoading;
        private IPublisher<PostUnityMessage> pubUnityMessage;
        private List<IEntity> mockServerDataForTestPagination;
        private IFeedApi feedApi;
        private IService userService;
        private Game.Config.IService configService;
        private bool hasMeSpawned;
        private IReelApi reelApi;
        private Game.Resource.IService resourceService;

        private ILogger Logger => logger ??= loggerFactory.CreateLogger<SocialLobbyManagerMobile>();

        [Inject]
        public void Construct(
            IAvatarFactory avatarFactory,
            PlatformInputSettings inputSetting,
            ILoggerFactory loggerFactory,
            IFeedApi feedApi,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<PostUnityMessage> pubPostUnityMessage,
            IService userService,
            Game.Config.IService configService,
            IReelApi reelApi,
            Game.Resource.IService resourceService)
        {
            this.avatarFactory = avatarFactory;
            this.loggerFactory = loggerFactory;
            this.inputSetting = inputSetting;
            this.feedApi = feedApi;
            this.reelApi = reelApi;
            this.appEntrySettings = appEntrySettings;
            this.pubSceneLoading = pubSceneLoading;
            this.pubUnityMessage = pubPostUnityMessage;
            this.lifetimeScope = lifetimeScope;
            this.userService = userService;
            this.configService = configService;
            this.resourceService = resourceService;
        }

        protected void Start()
        {
            cameraManager.EnableCustomCamera(true);
            Assert.IsTrue(cameraManager.IsEnableCustomCamera, nameof(cameraManager.IsEnableCustomCamera));

            panEventDispatcher.OnPan.AddListener(OnPan);
            panEventDispatcher.OnPanEnd.AddListener(OnPanEnd);

            if (entitySpawner != null)
            {
                entitySpawner.OnEntitiesChanged += UpdateCachedEntities;
            }

            scheduledExecutor = new ScheduledExecutor(Logger);

            // FIXME: workaround for demo, replace by call service API.
            DebugGenerateMockEntities();

            var customCamera = cameraManager.CustomCamera;

            // arrange spawn points world position.
            entitySpawner.TryArrangeSpawnPointGroup(customCamera);

            // create myself avatar
            GetMySelfAvatar(destroyCancellationToken).Forget();

            // get dataGroups which in visible buffer.
            if (entitySpawner.TryCreateVisibleFeedRequests(customCamera, out var feedRequests))
            {
                GetFeedDataAsync(
                    feedRequests,
                    fetchResult =>
                    {
                        // perform layout
                        entitySpawner.Populate(fetchResult);

                        UpdateEntitiesVisibility(isInitialize: true);
                    },
                    destroyCancellationToken).Forget();
            }

#if UNITY_EDITOR
            SceneView.duringSceneGui += EditorDraw;
#endif
        }

        protected void OnDestroy()
        {
            entitySpawner.OnEntitiesChanged -= UpdateCachedEntities;
            cameraManager.EnableCustomCamera(false);
            panEventDispatcher.OnPan.RemoveListener(OnPan);
            panEventDispatcher.OnPanEnd.RemoveListener(OnPanEnd);
            scheduledExecutor.Dispose();
#if UNITY_EDITOR
            SceneView.duringSceneGui -= EditorDraw;
#endif
        }

        private void DebugGenerateMockEntities()
        {
            var mockData = new List<IEntity>();
            var avatarFormatIndex = 0;
            var avatarFormatLength = avatarFormatList.Length;
            foreach (var config in mockDataConfigs)
            {
                for (int i = 0; i < config.Count; i++)
                {
                    var avatarFormat = avatarFormatList[avatarFormatIndex++ % avatarFormatLength];

                    var data = new PlayerEntityViewModel(
                        config.Size,
                        config.SortOrder,
                        avatarFormat,
                        avatarFactory,
                        appEntrySettings,
                        pubSceneLoading,
                        pubUnityMessage,
                        lifetimeScope,
                        config.Nickname,
                        config.Categories)
                    {
                        EntityManipulationTransform = entityManipulationTransform,
                        ScheduledExecutor = scheduledExecutor,
                        EntityManipulationRigidbody = entityManipulationRigidbody,
                        LoggerFactory = loggerFactory,
                        InputSetting = inputSetting,
                        ThumbnailUrl = config.ThumbnailUrl,
                        ConfigService = configService,
                        ReelApi = reelApi,
                        ResourceService = resourceService,
                    };

                    mockData.Add(data);
                }
            }

            mockServerDataForTestPagination = mockData;
        }

        private async UniTaskVoid GetMySelfAvatar(CancellationToken cancellationToken)
        {
            try
            {
                var user = await userService.GetUserAsync();
                cancellationToken.ThrowIfCancellationRequested();
                var xrid = user.Uid;
                var nickname = user.Nickame;
                var entity = new PlayerEntityViewModel(
                    avatarFactory,
                    appEntrySettings,
                    pubSceneLoading,
                    pubUnityMessage,
                    lifetimeScope,
                    nickname,
                    xrid,
                    defaultFeedTagOfMe)
                {
                    EntityManipulationTransform = entityManipulationTransform,
                    ScheduledExecutor = scheduledExecutor,
                    EntityManipulationRigidbody = entityManipulationRigidbody,
                    LoggerFactory = loggerFactory,
                    ConfigService = configService,
                    ReelApi = reelApi,
                    InputSetting = inputSetting,
                    IsMe = true,
                    ResourceService = resourceService,
                };

                entitySpawner.PopulateMe(entity);

                UpdateEntitiesVisibility(isInitialize: true);

                var avatarProfile = await userService.GetAvatarProfile(xrid, destroyCancellationToken);
                entity.AvatarProfile = avatarProfile;
                entity.InitializeVisibility();

                // get latest reel
                var size = 1;
                var offset = 0;
                var reelResponse =
                    await reelApi.ListReelsAsync(size, offset, xrid, cancellationToken: destroyCancellationToken);
                if (reelResponse.IsSuccess && reelResponse.Data != null)
                {
                    var latestReel = reelResponse.Data.Items.FirstOrDefault();
                    entity.ThumbnailUrl = latestReel?.Thumbnail;
                    entity.ReelId = latestReel?.Id;

                    // load thumbnail
                    entity.InitializeVisibility();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                logger.LogWarning($"GetMySelfAvatar failed", e.ToString());
            }
        }

        private async UniTaskVoid GetFeedDataAsync(
            Dictionary<EntitySpawnerSetting, FeedQueryParameter> feedQueryParameters,
            Action<Dictionary<EntitySpawnerSetting, List<IEntity>>> callback,
            CancellationToken cancellationToken)
        {
            var fetchResult = new Dictionary<EntitySpawnerSetting, List<IEntity>>();

            foreach (var kvp in feedQueryParameters)
            {
                var setting = kvp.Key;
                var parameter = kvp.Value;
                var dataGroup = setting.DataGroup;

                try
                {
                    // mark data group is fetched, prevent duplicate query.
                    dataGroup.IsFetched = true;
                    var entityDatas = await CreateEntitiesByOpenAPI(parameter);
                    cancellationToken.ThrowIfCancellationRequested();

                    fetchResult[setting] = entityDatas;
                }
                catch (OperationCanceledException)
                {
                    dataGroup.IsFetched = false;
                }
                catch (Exception e)
                {
                    dataGroup.IsFetched = false;
                    Logger.LogWarning($"{nameof(GetFeedDataAsync)} failed, {e}");
                }
            }

            callback?.Invoke(fetchResult);
        }

        private async UniTask<List<IEntity>> CreateEntitiesByOpenAPI(FeedQueryParameter parameter)
        {
            List<IEntity> entities = new List<IEntity>();

            var size = parameter.Size;
            var offset = parameter.Offset;
            var parameterCategories = parameter.Categories;
            try
            {
                var response = await feedApi.GetNewsFeedAsync(
                    size,
                    offset,
                    parameterCategories,
                    cancellationToken: destroyCancellationToken);

                if (!response.IsSuccess)
                {
                    throw new Exception(
                        $"{nameof(CreateEntitiesByOpenAPI)} failed, size={size}, offset={offset}, categories={string.Join(",", parameterCategories)} ErrorCode={response.ErrorCode}, Message={response.Message}");
                }

                // we cannot use response.Data.Total to check if there is no items, because response.Data.Total represent total feed count in DB.
                var dataItems = response.Data?.Items;
                if (dataItems == null || dataItems.Count == 0)
                {
                    Logger.LogDebug(
                        $"{nameof(CreateEntitiesByOpenAPI)} failed because it should contain at least one feed item (my feed), size={size}, offset={offset}, categories={string.Join(",", parameterCategories)} ErrorCode={response.ErrorCode}, Message={response.Message}");
                    return entities;
                }

                foreach (var feed in dataItems)
                {
                    var thumbnailUrl = feed.Content?.ThumbnailUrl;
                    var reelId = feed.Content?.RefId;
                    var nickname = feed.OwnerNickname;

                    var categories = feed.Categories.ToArray();
                    if (categories.Length == 0)
                    {
                        Logger.LogWarning(
                            $"{nameof(CreateEntitiesByOpenAPI)} skip create entity, feed.id={feed.Id} has no tag.");
                        continue;
                    }

                    var entity = new PlayerEntityViewModel(
                        avatarFactory,
                        appEntrySettings,
                        pubSceneLoading,
                        pubUnityMessage,
                        lifetimeScope,
                        nickname,
                        feed.OwnerXrid,
                        categories)
                    {
                        EntityManipulationTransform = entityManipulationTransform,
                        ScheduledExecutor = scheduledExecutor,
                        EntityManipulationRigidbody = entityManipulationRigidbody,
                        LoggerFactory = loggerFactory,
                        ConfigService = configService,
                        InputSetting = inputSetting,
                        ThumbnailUrl = thumbnailUrl,
                        ReelId = reelId,
                        ReelApi = reelApi,
                        ResourceService = resourceService,
                    };

                    entities.Add(entity);
                }

                // get avatar profiles
                GetAvatarProfiles(entities, destroyCancellationToken).Forget();
            }
            catch (OperationCanceledException)
            {
            }

            return entities;
        }

        private async UniTaskVoid GetAvatarProfiles(List<IEntity> entities, CancellationToken cancellationToken)
        {
            var xrids = entities.Where(
                    entity =>
                        entity is PlayerEntityViewModel playerEntity && !string.IsNullOrEmpty(playerEntity.Xrid))
                .Select(entity => ((PlayerEntityViewModel)entity).Xrid)
                .Distinct()
                .ToList();

            try
            {
                var profiles = new List<IAvatarProfile>();
                var profileCount = await userService.GetAvatarProfiles(xrids, profiles, destroyCancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (profileCount == 0)
                {
                    Logger.LogWarning($"{nameof(GetAvatarProfiles)} failed, no profile data.");
                    return;
                }

                // create entities by merge feed data and avatar profile
                var profileDictionary = profiles.ToDictionary(p => p.OwnerId);

                foreach (var entity in entities)
                {
                    if (entity is PlayerEntityViewModel playerEntity)
                    {
                        if (profileDictionary.TryGetValue(playerEntity.Xrid, out var profile))
                        {
                            playerEntity.AvatarProfile = profile;
                            playerEntity.InitializeVisibility();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.LogWarning($"{nameof(GetAvatarProfiles)} failed during get avatar profiles", e);
            }
        }

        [Obsolete("Remove this when Design background is ready")]
        private async UniTask<List<IEntity>> GetEntitiesByMockData(FeedQueryParameter parameter)
        {
            var offset = parameter.Offset;
            var size = parameter.Size;
            var categories = parameter.Categories.ToArray();

            return mockServerDataForTestPagination.Where(entity => entity.Categories.Intersect(categories).Any())
                .Skip(offset)
                .Take(size)
                .ToList();
        }

        private void UpdateCachedEntities(IReadOnlyCollection<IEntity> entities)
        {
            cachedEntities = entities.ToList();
        }

        private void OnPan()
        {
            var isArranged = entitySpawner.TryArrangeSpawnPointGroup(cameraManager.CustomCamera);

            if (isArranged)
            {
                entitySpawner.UpdateEntitiesAtSpawnPoints();
            }

            // get dataGroups which in visible buffer.
            UpdateEntitiesVisibility();
        }

        private void OnPanEnd()
        {
            // query data by current feed tag
            if (entitySpawner.TryCreateVisibleFeedRequests(cameraManager.CustomCamera, out var feedRequests))
            {
                GetFeedDataAsync(
                    feedRequests,
                    entities =>
                    {
                        // perform layout
                        entitySpawner.Populate(entities);

                        UpdateEntitiesVisibility();
                    },
                    destroyCancellationToken).Forget();
            }
        }

        private void UpdateEntitiesVisibility(bool isInitialize = false)
        {
            if (cachedEntities == null)
            {
                return;
            }

            var entityManagerTransform = entitySpawner.transform;
            foreach (var entity in cachedEntities)
            {
                var entityWorldPosition = entityManagerTransform.TransformPoint(entity.LocalPosition);

                var entitySize = entity.Size;
                entity.IsVisible = cameraManager.IsVisible(entityWorldPosition, entitySize);
                if (isInitialize)
                {
                    entity.InitializeVisibility();
                }
            }
        }

        [ContextMenu("Debug Refresh AvatarFormat list")]
        private void DebugRefreshAvatarFormatList()
        {
            var array = JArray.Parse(avatarFormats.text);
            avatarFormatList = array.Select(token => token.ToString()).ToArray();
        }

#if UNITY_EDITOR
        private void EditorDraw(SceneView view)
        {
            if (entitySpawner == null)
            {
                return;
            }

            entitySpawner.EditorSceneViewDraw(view);
        }
#endif
    }
}