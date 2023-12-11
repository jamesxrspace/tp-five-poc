using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace TPFive.Extended.VContainer
{
    /// <summary>
    /// Functions like <see cref="LifetimeScope"/> auto inject GameObjects,
    /// but it isn't required to be a <see cref="LifetimeScope"/>,
    /// also not necessary in the same scene with <see cref="LifetimeScope"/>.
    /// </summary>
    public class GameObjectInjector : MonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="LifetimeScope"/> that will be used to inject GameObjects.
        /// </summary>
        /// <remarks>
        /// Don't be confused about the <see cref="ParentReference"/> type name,
        /// we want to serialize the <see cref="LifetimeScope"/>
        /// and VContainer already has a <see cref="ParentReference"/> can do that.
        /// </remarks>
        [SerializeField]
        private ParentReference lifetimeScopeReference;

        /// <summary>
        /// auto inject GameObjects when Awake.
        /// </summary>
        [SerializeField]
        private bool autoInject = true;

        /// <summary>
        /// Which GameObjects that will be injected.
        /// </summary>
        [SerializeField]
        private List<GameObject> autoInjectGameObjects;

        private LifetimeScope runtimeLifetimeScope;

        public bool AutoInject => autoInject;

        public void InjectAll()
        {
            if (runtimeLifetimeScope == null)
            {
                Debug.LogError($"[{nameof(GameObjectInjector)}]{nameof(InjectAll)}(): Runtime LifetimeScope not found.");
                return;
            }

            var container = runtimeLifetimeScope.Container;
            if (container == null)
            {
                Debug.LogError($"[{nameof(GameObjectInjector)}]{nameof(InjectAll)}(): Runtime LifetimeScope container not ready yet.");
                return;
            }

            if (autoInjectGameObjects == null)
            {
                return;
            }

            foreach (var target in autoInjectGameObjects)
            {
                if (target == null)
                {
                    continue;
                }

                container.InjectGameObject(target);
            }
        }

        protected virtual void Awake()
        {
            runtimeLifetimeScope = GetRuntimeLifetimeScope();
            if (autoInject)
            {
                InjectAll();
            }
        }

        private LifetimeScope GetRuntimeLifetimeScope()
        {
            if (lifetimeScopeReference.Object != null)
            {
                return lifetimeScopeReference.Object;
            }

            if (lifetimeScopeReference.Type == null)
            {
                return null;
            }

            var found = (LifetimeScope)FindObjectOfType(lifetimeScopeReference.Type);
            if (found == null || found.Container == null)
            {
                throw new VContainerParentTypeReferenceNotFound(
                    lifetimeScopeReference.Type,
                    $"{name} could not found lifetime scope reference of type : {lifetimeScopeReference.Type}");
            }

            return found;
        }
    }
}