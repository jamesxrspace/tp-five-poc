using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace TPFive.Game.Editor
{
    public class Utility
    {
        public static IReadOnlyList<T> GetUnityAssetCollectionOfType<T>()
                where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            var results =
                    guids
                        .Select(guid =>
                        {
                            var path = AssetDatabase.GUIDToAssetPath(guid);
                            var result = AssetDatabase.LoadAssetAtPath<T>(path);

                            return result;
                        })
                        .Where(x => x != null);

            return results.ToList();
        }

        public static T GetUnityAssetOfType<T>()
                where T : UnityEngine.Object
        {
            var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name}").FirstOrDefault();
            if (guid == null)
            {
                return default;
            }

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var result = AssetDatabase.LoadAssetAtPath<T>(path);

            return result;
        }

        /// <summary>
        /// Get platform and return according name of the folder.
        /// </summary>
        /// <returns>
        /// Platform that matches the OS Unity Editor is running on.
        /// </returns>
        public static string GetPlatform()
        {
            var platform = string.Empty;
#if UNITY_EDITOR_WIN
            platform = "win-x64";
#elif UNITY_EDITOR_OSX
            platform = "osx-x64";
#elif UNITY_EDITOR_LINUX
            platform = "linux-x64";
#endif
            return platform;
        }
    }
}