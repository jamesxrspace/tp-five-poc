using System.IO;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Utils
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        /// This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject you want create.</typeparam>
        public static void CreateAsset<T>()
            where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), string.Empty);
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, $"New {typeof(T).Name}.asset"));

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
