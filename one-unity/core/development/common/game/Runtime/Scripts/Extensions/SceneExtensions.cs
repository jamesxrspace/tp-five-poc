using System;
using System.Collections.Generic;
using DynamicData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Game.Extensions
{
    public static class SceneExtensions
    {
        /// <summary>
        /// Gets the first component of the specified type in the scene.
        /// </summary>
        /// <param name="scene">The scene to search in.</param>
        /// <param name="type">The type of component to find.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <returns>The first component of the specified type found in the scene, or null if not found.</returns>
        public static Component GetComponentInScene(this Scene scene, Type type, bool includeInactive = false)
        {
            Component component = null;
            foreach (var rootObj in scene.GetRootGameObjects())
            {
                component = rootObj.GetComponentInChildren(type, includeInactive);
                if (component != null)
                {
                    break;
                }
            }

            return component;
        }

        /// <summary>
        /// Gets the first component of type T found in the specified scene.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="scene">The scene to search in.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects in the search.</param>
        /// <returns>The first component of type T found in the scene, or null if not found.</returns>
        public static T GetComponentInScene<T>(this Scene scene, bool includeInactive = false)
            where T : Component
        {
            T component = null;

            foreach (var rootObj in scene.GetRootGameObjects())
            {
                component = rootObj.GetComponentInChildren<T>(includeInactive);
                if (component != null)
                {
                    break;
                }
            }

            return component;
        }

        /// <summary>
        /// Gets all components of a specified type in a scene.
        /// </summary>
        /// <param name="scene">The scene to search in.</param>
        /// <param name="type">The type of components to search for.</param>
        /// <param name="results">A list to store the found components.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects in the search.</param>
        /// <returns>The total number of components found in the scene.</returns>
        public static int GetComponentsInScene(this Scene scene, Type type, List<Component> results, bool includeInactive = false)
        {
            int count = 0;
            foreach (var rootObj in scene.GetRootGameObjects())
            {
                var components = rootObj.GetComponentsInChildren(type, includeInactive);
                count += components.Length;
                results.AddRange(components);
            }

            return count;
        }

        /// <summary>
        /// Gets all components of type T in the scene.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="scene">The scene to search for.</param>
        /// <param name="results">A list to use for the returned results.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <returns>The total number of type T found in the scene.</returns>
        public static int GetComponentsInScene<T>(this Scene scene, List<T> results, bool includeInactive = false)
            where T : Component
        {
            int count = 0;
            foreach (var rootObj in scene.GetRootGameObjects())
            {
                var components = rootObj.GetComponentsInChildren<T>(includeInactive);
                count += components.Length;
                results.AddRange(components);
            }

            return count;
        }

        /// <summary>
        /// Gets all the components of a specified type in the scene.
        /// </summary>
        /// <param name="scene">The scene to search for.</param>
        /// <param name="type">The type of components to search for.</param>
        /// <param name="includeInactive">Determines whether to include inactive components in the search. Default is false.</param>
        /// <returns>T[] An array of all found components matching the specified type.</returns>
        public static Component[] GetComponentsInScene(this Scene scene, Type type, bool includeInactive = false)
        {
            var result = new List<Component>();
            GetComponentsInScene(scene, type, result, includeInactive);
            return result.ToArray();
        }

        /// <summary>
        /// Gets all components of type T in the scene.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="scene">The scene to search for.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <returns>T[] An array of all found components matching the specified type T.</returns>
        public static T[] GetComponentsInScene<T>(this Scene scene, bool includeInactive = false)
            where T : Component
        {
            var result = new List<T>();
            GetComponentsInScene(scene, result, includeInactive);
            return result.ToArray();
        }
    }
}