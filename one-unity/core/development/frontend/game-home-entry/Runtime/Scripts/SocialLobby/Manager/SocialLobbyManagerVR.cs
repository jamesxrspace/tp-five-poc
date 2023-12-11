using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.SceneFlow;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using UniRx;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IService = TPFive.Game.User.IService;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPFive.Home.Entry.SocialLobby
{
    public class SocialLobbyManagerVR : MonoBehaviour
    {
        [SerializeField]
        private SphereVisibleBound sphereVisibleBound;
        [SerializeField]
        private PanInteractable panInteractable;
        [SerializeField]
        private PanTexture panTexture;
        [SerializeField]
        private EntityManager entityManager;
        [SerializeField]
        private LayoutController layoutController;
        [SerializeField]
        private Rigidbody tableRigidbody;

        /// <summary>
        /// Vector2.x = transform.forward, Vector2.y = transform.right.
        /// </summary>
        [SerializeField]
        private Vector2 maxPanOffset = new Vector2(1, 1);
        [SerializeField]
        private Vector2 minPanOffset = new Vector2(-1, -1);
        [SerializeField]
        private Vector2 currentPanOffset;
        private List<IEntity> cachedEntities;

        [Header("Debug-FakeData")]
        [SerializeField]
        private List<MockEntityDataConfig> mockDataConfigs;
        [SerializeField]
        [Multiline]
        private string avatarFormat;
        private IAvatarFactory avatarFactory;
        private ScheduledExecutor scheduledExecutor;
        private ILoggerFactory loggerFactory;
        private ILogger logger;
        private Settings appEntrySettings;
        private IPublisher<ChangeScene> pubSceneLoading;
        private IPublisher<PostUnityMessage> pubUnityMessage;
        private LifetimeScope lifetimeScope;
        private IService userService;
        private Game.Config.IService configService;
        private IReelApi reelApi;
        private Game.Resource.IService resourceService;
        private Game.SceneFlow.IService sceneFlowService;

        private ILogger Logger => logger ??= loggerFactory.CreateLogger<SocialLobbyManagerVR>();

        [Inject]
        public void Construct(
            IAvatarFactory avatarFactory,
            ILoggerFactory loggerFactory,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<PostUnityMessage> pubUnityMessage,
            Game.User.IService userService,
            Game.Config.IService configService,
            IReelApi reelApi,
            Game.Resource.IService resourceService,
            Game.SceneFlow.IService sceneFlowService,
            ISubscriber<Game.Messages.ContentLevelFullyLoaded> subContentLevelFullyLoaded)
        {
            this.avatarFactory = avatarFactory;
            this.loggerFactory = loggerFactory;
            this.appEntrySettings = appEntrySettings;
            this.pubSceneLoading = pubSceneLoading;
            this.pubUnityMessage = pubUnityMessage;
            this.lifetimeScope = lifetimeScope;
            this.userService = userService;
            this.reelApi = reelApi;
            this.configService = configService;
            this.resourceService = resourceService;
            this.sceneFlowService = sceneFlowService;
            subContentLevelFullyLoaded.Subscribe(_ => tableRigidbody.useGravity = true).AddTo(this);
        }

        [ContextMenu(nameof(DebugGenerateMockEntities))]
        public void DebugGenerateMockEntities()
        {
            var mockData = new List<IEntity>();
            var xrid = "4b60a49b-14fe-4aed-a06b-a19b083b4788";
            _ = userService.GetAvatarProfile(
                xrid,
                destroyCancellationToken).ContinueWith(
                profile =>
                {
                    foreach (var config in mockDataConfigs)
                    {
                        for (int i = 0; i < config.Count; i++)
                        {
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
                                new CategoriesEnum[] { CategoriesEnum.Friends })
                            {
                                ScheduledExecutor = scheduledExecutor,
                                LoggerFactory = loggerFactory,
                                ThumbnailUrl = config.ThumbnailUrl,
                                AvatarProfile = profile,
                                ConfigService = configService,
                                ReelApi = reelApi,
                                ResourceService = resourceService,
                                SceneFlowService = sceneFlowService,
                            };
                            mockData.Add(data);
                        }
                    }

                    entityManager.AddEntities(mockData);

                    // perform layout
                    UpdateEntitiesLayout();

                    ForceUpdateEntitiesVisibility();

                    UpdatePanOffsetMaxValue();
                });
        }

        protected void Start()
        {
            if (panInteractable != null)
            {
                panInteractable.OnPan += OnPan;
            }

            if (entityManager != null)
            {
                entityManager.OnEntitiesChanged += UpdateCachedEntities;
            }

            scheduledExecutor = new ScheduledExecutor(Logger);

            // FIXME: workaround for demo, replace by call service API.
            DebugGenerateMockEntities();

#if UNITY_EDITOR
            SceneView.duringSceneGui += EditorDraw;
#endif
        }

        protected void OnDestroy()
        {
            panInteractable.OnPan -= OnPan;
            entityManager.OnEntitiesChanged -= UpdateCachedEntities;
            scheduledExecutor.Dispose();

#if UNITY_EDITOR
            SceneView.duringSceneGui -= EditorDraw;
#endif
        }

        [ContextMenu(nameof(UpdateEntitiesLayout))]
        private void UpdateEntitiesLayout()
        {
            layoutController.Populate(cachedEntities);
        }

        private void UpdateCachedEntities(IReadOnlyCollection<IEntity> entities)
        {
            cachedEntities = entities.ToList();
        }

        private void OnPan(Vector2 panOffset)
        {
            var clampedPanOffset = GetClampPanOffset(panOffset);

            UpdateTexture(clampedPanOffset);

            UpdateEntitiesPosition(clampedPanOffset);

            UpdateEntitiesVisibility();
        }

        private Vector2 GetClampPanOffset(Vector2 panOffset)
        {
            var oldPanOffset = new Vector2(currentPanOffset.x, currentPanOffset.y);
            currentPanOffset += panOffset;
            currentPanOffset.x = Mathf.Clamp(currentPanOffset.x, minPanOffset.x, maxPanOffset.x);
            currentPanOffset.y = Mathf.Clamp(currentPanOffset.y, minPanOffset.y, maxPanOffset.y);
            return currentPanOffset - oldPanOffset;
        }

        private void UpdateTexture(Vector2 clampedPanOffset)
        {
            if (panTexture == null)
            {
                return;
            }

            panTexture.ApplyPanToTarget(clampedPanOffset);
        }

        private void UpdateEntitiesPosition(Vector2 panOffset)
        {
            if (cachedEntities == null)
            {
                return;
            }

            foreach (var entity in cachedEntities)
            {
                // update local pos
                entity.LocalPosition += new Vector3(panOffset.x, 0, panOffset.y);
            }
        }

        [ContextMenu(nameof(ForceUpdateEntitiesVisibility))]
        private void ForceUpdateEntitiesVisibility()
        {
            UpdateEntitiesVisibility(true);
        }

        private void UpdateEntitiesVisibility()
        {
            UpdateEntitiesVisibility(false);
        }

        private void UpdateEntitiesVisibility(bool isInitialize = false)
        {
            if (cachedEntities == null)
            {
                return;
            }

            foreach (var entity in cachedEntities)
            {
                var entityGlobalPos = entityManager.transform.TransformPoint(entity.LocalPosition);
                entity.IsVisible = sphereVisibleBound.IsInside(entityGlobalPos);

                if (isInitialize)
                {
                    entity.InitializeVisibility();
                }
            }
        }

        private void UpdatePanOffsetMaxValue()
        {
            var layoutAxis = layoutController.MainLayoutAxis;
            var unexpectedLayoutAxisException = new NotImplementedException(
                $"{nameof(UpdatePanOffsetMaxValue)} failed, unexpected layout axis {layoutAxis}");

            // only support x and z axis.
            if (layoutAxis != LayoutAxis.X && layoutAxis != LayoutAxis.Z)
            {
                throw unexpectedLayoutAxisException;
            }

            // select data for calculation by layout axis.
            IEntity entityWithMaxValue = null;
            float maxValue = float.MinValue;
            float sizeOfMaxValue = 0f;
            var entityDataList = cachedEntities.Select(
                entity => layoutAxis switch
                {
                    LayoutAxis.X => new
                    {
                        entity,
                        layoutAxisLocalPosition = entity.LocalPosition.x,
                        size = entity.Size.x,
                    },
                    LayoutAxis.Z => new
                    {
                        entity,
                        layoutAxisLocalPosition = entity.LocalPosition.z,
                        size = entity.Size.z,
                    },
                    _ => throw unexpectedLayoutAxisException
                });

            // find max value
            foreach (var data in entityDataList)
            {
                var max = Mathf.Max(Mathf.Abs(data.layoutAxisLocalPosition), maxValue);
                if (max > maxValue)
                {
                    maxValue = max;
                    entityWithMaxValue = data.entity;
                    sizeOfMaxValue = data.size;
                }
            }

            // add size of max value
            if (entityWithMaxValue != null)
            {
                maxValue += sizeOfMaxValue;
            }

            // update min pan offset
            minPanOffset = layoutAxis switch
            {
                LayoutAxis.X => new Vector2(sphereVisibleBound.Radius - maxValue, minPanOffset.y),
                LayoutAxis.Z => new Vector2(minPanOffset.x, sphereVisibleBound.Radius - maxValue),
                _ => minPanOffset
            };
        }

#if UNITY_EDITOR
        private void EditorDraw(SceneView view)
        {
            entityManager.EditorSceneViewDraw(view);
        }
#endif
    }
}