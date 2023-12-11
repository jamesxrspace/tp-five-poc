using System;
using System.Collections.Generic;
using System.Linq;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPFive.Home.Entry.SocialLobby
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField]
        private List<EntitySpawnerSetting> settings;
        [SerializeField]
        private Transform entityLocalTransform;
        [SerializeField]
        private bool isShuffleSpawnPoint;
        [SerializeField]
        private List<GameObject> entityViews = new List<GameObject>();
        [SerializeField]
        private Transform entityParentTransform;
        private int currentGroupIndex = -1;
        private List<SpawnPoint> spawnPointsBuffer = new List<SpawnPoint>();

        [SerializeField]
        private PlayerEntityViewBase playerEntityPrefab;
        [SerializeField]
        private bool showPositionInSceneView = true;

        [Header("Editor Debug Settings")]
        [SerializeField]
        private Color entityVisibleColor = Color.green;
        [SerializeField]
        private Color entityInvisibleColor = Color.gray;
        private List<IEntity> entities = new List<IEntity>();

        public event Action<IReadOnlyCollection<IEntity>> OnEntitiesChanged;

        public void Populate(Dictionary<EntitySpawnerSetting, List<IEntity>> entityInfos)
        {
            if (settings == null)
            {
                return;
            }

            // get valid spawn point by entity tag
            foreach (var info in entityInfos)
            {
                var setting = info.Key;
                var entities = info.Value;
                var spawnPointsDictionary = GetEmptySpawnPoints(setting);
                PopulateEntitiesAtSpawnPoints(entities, spawnPointsDictionary);
            }
        }

        public void PopulateMe(IEntity entity)
        {
            if (entity is PlayerEntityViewModel player && player.IsMe)
            {
                var spawnPoint = GetMySpawnPoint(entity.Categories);
                if (spawnPoint != null)
                {
                    AddEntity(entity);
                    spawnPoint.SetOccupier(entity);
                    spawnPoint.SetOccupierLocalPosition(entityLocalTransform);
                }
            }
        }

        public bool TryArrangeSpawnPointGroup(Camera customCamera)
        {
            if (settings == null)
            {
                return false;
            }

            SpawnPointGroup currentGroup = null;
            bool isGroupChanged = false;
            for (var index = 0; index < settings.Count; ++index)
            {
                var group = settings[index].SpawnPointGroup;

                var left = group.LeftWorldPosition;
                var right = group.RightWorldPosition;

                // find current spawnPointGroup
                var customCameraTransform = customCamera.transform;
                if (left.x < customCameraTransform.position.x && customCameraTransform.position.x < right.x)
                {
                    currentGroup = group;
                    if (currentGroupIndex != index)
                    {
                        currentGroupIndex = index;
                        isGroupChanged = true;
                        break;
                    }
                }
            }

            if (currentGroup == null)
            {
                return false;
            }

            if (!isGroupChanged)
            {
                return false;
            }

            ArrangePreviousAndNextSpawnPointGroup(currentGroup);
            return true;
        }

        public void UpdateEntitiesAtSpawnPoints()
        {
            if (settings == null)
            {
                return;
            }

            foreach (var setting in settings)
            {
                setting.SpawnPointGroup.GetSpawnPoints(spawnPointsBuffer);

                foreach (var spawnPoint in spawnPointsBuffer)
                {
                    spawnPoint.SetOccupierLocalPosition(entityLocalTransform);
                }

                var settingSpawnPointForMe = setting.SpawnPointForMe;
                if (settingSpawnPointForMe != null)
                {
                    settingSpawnPointForMe.SetOccupierLocalPosition(entityLocalTransform);
                }

                spawnPointsBuffer.Clear();
            }
        }

        public bool TryCreateVisibleFeedRequests(
            Camera customCamera,
            out Dictionary<EntitySpawnerSetting, FeedQueryParameter> feedQueryList)
        {
            feedQueryList = settings.Aggregate(
                new Dictionary<EntitySpawnerSetting, FeedQueryParameter>(),
                (result, setting) =>
                {
                    var group = setting.DataGroup;
                    if (group.IsFetched)
                    {
                        return result;
                    }

                    var left = group.LeftWorldPosition;
                    var right = group.RightWorldPosition;

                    // find in buffer data group
                    var customCameraTransform = customCamera.transform;
                    if (left.x < customCameraTransform.position.x && customCameraTransform.position.x < right.x)
                    {
                        result[setting] = new FeedQueryParameter()
                        {
                            Offset = group.Offset,
                            Size = group.Size,
                            Categories = setting.Categories.ToList(),
                        };
                    }

                    return result;
                });

            return feedQueryList.Count > 0;
        }

#if UNITY_EDITOR
        public void EditorSceneViewDraw(SceneView obj)
        {
            var originalColor = Handles.color;
            var originalMatrix = Handles.matrix;
            Handles.matrix = transform.localToWorldMatrix;

            foreach (var entity in entities)
            {
                var handleColor = entity.IsVisible ? entityVisibleColor : entityInvisibleColor;
                Handles.color = handleColor;
                Handles.DrawWireCube(entity.LocalPosition, entity.Size);
                var worldPos = transform.TransformPoint(entity.LocalPosition);

                if (showPositionInSceneView)
                {
                    Handles.Label(entity.LocalPosition, $"{worldPos}");
                }
            }

            Handles.matrix = originalMatrix;
            Handles.color = originalColor;
        }
#endif

        protected void OnDestroy()
        {
            spawnPointsBuffer = null;
        }

        private Dictionary<CategoriesEnum[], Queue<SpawnPoint>> GetEmptySpawnPoints(EntitySpawnerSetting setting)
        {
            var spawnPointsDictionary =
                new Dictionary<CategoriesEnum[], Queue<SpawnPoint>>();

            var group = setting.SpawnPointGroup;
            group.GetSpawnPoints(spawnPointsBuffer, isShuffleSpawnPoint);

            spawnPointsDictionary.Add(setting.Categories.ToArray(), new Queue<SpawnPoint>(spawnPointsBuffer));

            spawnPointsBuffer.Clear();
            return spawnPointsDictionary;
        }

        private void PopulateEntitiesAtSpawnPoints(
            List<IEntity> entites,
            Dictionary<CategoriesEnum[], Queue<SpawnPoint>> spawnPoints)
        {
            foreach (var entity in entites)
            {
                SpawnPoint spawnPoint = null;
                if (entity is PlayerEntityViewModel player && player.IsMe)
                {
                    spawnPoint = GetMySpawnPoint(entity.Categories);
                }
                else
                {
                    var queue = spawnPoints.Where(s => s.Key.Intersect(entity.Categories).Any()).Select(s => s.Value).First();
                    if (queue != null && queue.Count > 0)
                    {
                        spawnPoint = queue.Dequeue();
                    }
                }

                if (spawnPoint != null)
                {
                    AddEntity(entity);
                    spawnPoint.SetOccupier(entity);
                    spawnPoint.SetOccupierLocalPosition(entityLocalTransform);
                }
            }
        }

        private SpawnPoint GetMySpawnPoint(CategoriesEnum[] categories)
        {
            var matched = settings.FirstOrDefault(
                s =>
                    s.SpawnPointForMe != null && s.Categories.Intersect(categories).Any() &&
                    s.SpawnPointForMe.Occupier == null);

            return matched?.SpawnPointForMe;
        }

        private void ArrangePreviousAndNextSpawnPointGroup(SpawnPointGroup currentGroup)
        {
            var previousIndex = (currentGroupIndex - 1 + settings.Count) % settings.Count;
            var nextIndex = (currentGroupIndex + 1) % settings.Count;
            var previousGroup = settings[previousIndex].SpawnPointGroup;
            var nextGroup = settings[nextIndex].SpawnPointGroup;
            var previousGroupTransform = previousGroup.transform;
            previousGroupTransform.position = currentGroup.LeftWorldPosition -
                                              (previousGroup.RightWorldPosition -
                                               previousGroupTransform.position);
            var nextGroupTransform = nextGroup.transform;
            nextGroupTransform.position = currentGroup.RightWorldPosition +
                nextGroupTransform.position - nextGroup.LeftWorldPosition;
        }

        private void AddEntity(IEntity data)
        {
            entities.Add(data);

            var view = CreateEntityView(data);
            if (view != null)
            {
                entityViews.Add(view);
            }

            OnEntitiesChanged?.Invoke(entities?.AsReadOnly());
        }

        [ContextMenu(nameof(ShowSpawnPointRenderer))]
        private void ShowSpawnPointRenderer()
        {
            foreach (var setting in settings)
            {
                setting.SpawnPointGroup.GetSpawnPoints(spawnPointsBuffer);

                foreach (var spawnPoint in spawnPointsBuffer)
                {
                    spawnPoint.EnableRenderer = true;
                }

                spawnPointsBuffer.Clear();
            }
        }

        [ContextMenu(nameof(HideSpawnPointRenderer))]
        private void HideSpawnPointRenderer()
        {
            foreach (var setting in settings)
            {
                setting.SpawnPointGroup.GetSpawnPoints(spawnPointsBuffer);

                foreach (var spawnPoint in spawnPointsBuffer)
                {
                    spawnPoint.EnableRenderer = false;
                }

                spawnPointsBuffer.Clear();
            }
        }

        private GameObject CreateEntityView(IEntity entity)
        {
            if (entity is PlayerEntityViewModel playerEntityViewModel)
            {
                var view = Instantiate(
                    playerEntityPrefab,
                    entity.LocalPosition,
                    transform.rotation,
                    entityParentTransform);
                view.Initialize(playerEntityViewModel);
                view.name = playerEntityViewModel.Nickname;
                return view.gameObject;
            }
            else
            {
                // TODO: Add other entity types
                return null;
            }
        }
    }
}