using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPFive.Home.Entry.SocialLobby
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerEntityViewBase playerEntityPrefab;
        [SerializeField]
        private List<GameObject> entityViews = new List<GameObject>();
        [SerializeField]
        private bool showPositionInSceneView = true;

        [Header("Editor Debug Settings")]
        [SerializeField]
        private Color entityVisibleColor = Color.green;
        [SerializeField]
        private Color entityInvisibleColor = Color.gray;
        private List<IEntity> entities = new List<IEntity>();

        public event Action<IReadOnlyCollection<IEntity>> OnEntitiesChanged;

        public void AddEntities(List<IEntity> data)
        {
            if (entities == null)
            {
                this.entities = new List<IEntity>(data);
            }
            else
            {
                entities.AddRange(data);
            }

            foreach (var entity in data)
            {
                // add view
                var view = CreateEntityView(entity);
                if (view != null)
                {
                    entityViews.Add(view);
                }
            }

            OnEntitiesChanged?.Invoke(entities?.AsReadOnly());
        }

        public void ClearEntityViews()
        {
            var childCount = transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            entityViews.Clear();
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

        private GameObject CreateEntityView(IEntity entity)
        {
            if (entity is PlayerEntityViewModel playerEntityViewModel)
            {
                var view = Instantiate(playerEntityPrefab, entity.LocalPosition, transform.rotation, transform);
                view.Initialize(playerEntityViewModel);
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