using System.IO;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Editor
{
    public static class ImportUnitypackages
    {
        [InitializeOnLoadMethod]
        public static void OnLoad()
        {
            {
                var path = "Packages/com.unity.assetstore.editor-console-pro/Samples~/Editor Console Pro.unitypackage";
                var destPath = Path.Combine(Application.dataPath, "Editor Console Pro");
                if (File.Exists(path) && !Directory.Exists(destPath))
                {
                    UnityEditor.AssetDatabase.ImportPackage(path, false);
                }
            }

            {
                var path = "Packages/com.unity.assetstore.quantum-console/Samples~/Quantum Console.unitypackage";
                var destPath = Path.Combine(Application.dataPath, "Plugins", "Quantum Console");
                if (File.Exists(path) && !Directory.Exists(destPath))
                {
                    UnityEditor.AssetDatabase.ImportPackage(path, false);
                }
            }

            {
                var path = "Packages/com.unity.assetstore.ultimate-editor-enhancer/Samples~/Ultimate Editor Enhancer.unitypackage";
                var destPath = Path.Combine(Application.dataPath, "Plugins", "Infinity Code");
                if (File.Exists(path) && !Directory.Exists(destPath))
                {
                    UnityEditor.AssetDatabase.ImportPackage(path, false);
                }
            }
        }

        [MenuItem("TPFive/Import Plugin/Easy Character Movement 2")]
        public static void ImportEasyCharacterMovement()
        {
            var contentUnitypackagePath = "Assets/Easy Character Movement 2/Easy Character Movement 2 - Input System (aka New input).unitypackage";
            var absolutePath = Path.Combine(
                Application.dataPath,
                "Easy Character Movement 2",
                "Easy Character Movement 2 - Input System (aka New input).unitypackage");

            if (File.Exists(absolutePath))
            {
                UnityEditor.AssetDatabase.ImportPackage(contentUnitypackagePath, false);
            }
        }
    }
}
