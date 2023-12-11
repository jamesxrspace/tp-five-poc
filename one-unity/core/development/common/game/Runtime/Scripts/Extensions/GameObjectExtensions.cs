using System.Collections.Generic;
using TPFive.Game.Text;
using UnityEngine;

namespace TPFive.Game.Extensions
{
    public static class GameObjectExtensions
    {
        private static readonly int DefaultCapacity = 30;
        private static readonly List<string> GameObjectNames = new List<string>(DefaultCapacity);

        /// <summary>
        /// Extension method for the GameObject class that retrieves the full hierarchical path of the GameObject.
        /// The path is constructed by concatenating the names of the GameObject and all its parent GameObjects, separated by '/'.
        /// </summary>
        /// <param name="gameObject">The GameObject for which to retrieve the path.</param>
        /// <returns>
        /// A string representing the full hierarchical path of the GameObject, including the names of
        /// the GameObject and all its parent GameObjects.
        /// The path is constructed by concatenating the names, separated by '/'.
        /// </returns>
        public static string GetGameObjectPath(this GameObject gameObject)
        {
            var name = gameObject.name;
            int length = name.Length;
            GameObjectNames.Add(name);

            GameObject go = gameObject;
            while (go.transform.parent != null)
            {
                var parent = go.transform.parent.gameObject;
                name = parent.name;
                length = length + name.Length + 1;
                GameObjectNames.Add(name);

                go = parent;
            }

            var sb = StringBuilderCache.Acquire(length);
            for (int i = GameObjectNames.Count - 1; i >= 0; --i)
            {
                sb.Append(GameObjectNames[i]);
                if (i > 0)
                {
                    sb.Append('/');
                }
            }

            var path = StringBuilderCache.GetStringAndRelease(sb);
            GameObjectNames.Clear();
            return path;
        }

        /// <summary>
        /// Extension method for the GameObject class that retrieves a Component of the specified type attached to the GameObject.
        /// If the Component is not found, it adds a new instance of the Component to the GameObject and returns it.
        /// </summary>
        /// <typeparam name="T">The type of Component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to search for the Component on.</param>
        /// <returns>
        /// The Component of the specified type attached to the GameObject if it exists; otherwise, a new instance of
        /// the Component is added to the GameObject and returned.
        /// </returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            if (!gameObject.TryGetComponent<T>(out var component))
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        /// <summary>
        /// Extension method for the GameObject class that retrieves a Component of the specified type attached to the GameObject.
        /// If the Component is not found, it adds a new instance of the Component to the GameObject and returns it.
        /// </summary>
        /// <param name="gameObject">The GameObject to search for the Component on.</param>
        /// <param name="type">The type of Component to get or add.</param>
        /// <returns>
        /// The Component of the specified type attached to the GameObject if it exists; otherwise,
        /// a new instance of the Component is added to the GameObject and returned.
        /// </returns>
        public static Component GetOrAddComponent(this GameObject gameObject, System.Type type)
        {
            if (!gameObject.TryGetComponent(type, out Component component))
            {
                component = gameObject.AddComponent(type);
            }

            return component;
        }

        /// <summary>
        /// Checks if the specified Component type is attached to the GameObject.
        /// </summary>
        /// <typeparam name="T">The type of Component to check for.</typeparam>
        /// <param name="gameObject">The GameObject to check for the Component on.</param>
        /// <returns>
        /// True if a Component of the specified type is attached to the GameObject; otherwise, false.
        /// </returns>
        public static bool HasComponent<T>(this GameObject gameObject)
            where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }
    }
}