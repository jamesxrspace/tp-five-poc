using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEditor;
using UnityEngine;
using Path = System.IO.Path;

namespace TPFive.Creator.Editor
{
    public class Utility
    {
        // Get all assets of type T in project
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
        /// This is used to get the path and index for addressable import settings rule.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static (int, string) GetAdjustedPathWithIndex(string path)
        {
            const int deductedCount = 2;
            const int invalidIndex = -1;

            // Split the path using '/' as separator as no matter what platform is used, the addressable path
            // is always using '/' as separator.
            var pathSections = path
                .Split('/')
                .ToList();
            var usedCount = (pathSections.Count > deductedCount) ? pathSections.Count - deductedCount : invalidIndex;
            if (usedCount <= invalidIndex)
            {
                return (invalidIndex, string.Empty);
            }
            
            // Skip the last one as the last one is asset file itself.
            var combinedPath = pathSections
                .SkipLast(1)
                .Aggregate(string.Empty, Path.Combine)
                .Replace("\\", "/")
                .TrimStart('/');
            
            return (usedCount, combinedPath);
        }
    }
}
