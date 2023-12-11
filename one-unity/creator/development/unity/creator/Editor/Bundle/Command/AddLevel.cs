using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    using CreatorCrossEditorBridge = TPFive.Creator.Cross.Editor.Bridge;

    /// <summary>
    /// Add new level to the project. This created a predefined folder structure and a scene.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class AddLevel
    {
        [MenuItem("TPFive/Creator/Add Level")]
        internal static void Handle()
        {
            var bundleId = System.Guid.NewGuid().ToString();
            var bundleFolderName = $"Bundle - {bundleId}";

            // Create level folder
            var (success, bundleFolderPath, levelFolderPath, entryFolderPath, sceneFolderPath) =
                CreateFolders(bundleFolderName);

            if (!success)
            {
                Logger.LogWarning(
                    "{Method} Can not create folders",
                    nameof(Handle));

                return;
            }

            // Create scene and save
            var (_, sceneFilePath) = CreateSceneAndSave(sceneFolderPath, entryFolderPath, Define.LevelName, bundleId);
            var sceneGuid = AssetDatabase.GUIDFromAssetPath(sceneFilePath);

            // Create AddressableImportSettings
            var (aisSuccess, ais) = BundleHandlingUtility.CreateAddressableImportSettings(bundleFolderPath);
            if (!aisSuccess)
            {
                Logger.LogWarning(
                    "{Method} Can not create AddressableImportSettings",
                    nameof(Handle));

                return;
            }

            var thumbnailPath = Path.Combine(levelFolderPath, Define.ThumbnailIconName);
            var defaultThumbnailPath = Define.BundleLevelIconPath;
            var createThumbnailResult = AssetDatabase.CopyAsset(defaultThumbnailPath, thumbnailPath);;
            if (!createThumbnailResult)
            {
                Logger.LogError(
                    "{Method} Can not create thumbnail",
                    nameof(Handle));

                return;
            }

            // Create Mapping Layer Data
            BundleHandlingUtility.CreateLayerMapping(levelFolderPath);

            // Create Bundle Detail Data
            BundleHandlingUtility.CreateBundleDetailData(
                bundleId,
                bundleFolderPath,
                thumbnailPath,
                "New Level",
                "Newly created level",
                BundleKind.Level,
                new List<UnityEditor.GUID> { sceneGuid });

            AssetDatabase.SaveAssets();
        }

        private static (bool, string, string, string, string) CreateFolders(string bundleFolderName)
        {
            // All the folders have to be created, if any is not created, return false
            var (bundleFolderGuid, bundleFolderPath) = Utility.CreateFolder(Define.ContentPath, bundleFolderName);
            if (string.IsNullOrEmpty(bundleFolderGuid))
            {
                Logger.LogWarning(
                    "{Method} - Can not create {bundleFolderName}",
                    nameof(CreateFolders),
                    bundleFolderName);

                return (false, default, default, default, default);
            }

            var (levelFolderGuid, levelFolderPath) = Utility.CreateFolder(bundleFolderPath, "Level");
            if (string.IsNullOrEmpty(levelFolderGuid))
            {
                Logger.LogWarning(
                    "{Method} - Can not create {levelFolderName}",
                    nameof(CreateFolders),
                    "Level");

                return (false, bundleFolderPath, default, default, default);
            }

            var (entryFolderGuid, entryFolderPath) = Utility.CreateFolder(levelFolderPath, "Entry");
            if (string.IsNullOrEmpty(entryFolderGuid))
            {
                Logger.LogWarning(
                    "{Method} - Can not create {entryFolderName}",
                    nameof(CreateFolders),
                    "Entry");

                return (false, bundleFolderPath, levelFolderPath, default, default);
            }

            var (sceneFolderGuid, sceneFolderPath) = Utility.CreateFolder(entryFolderPath, "Scenes");
            if (string.IsNullOrEmpty(sceneFolderGuid))
            {
                Logger.LogWarning(
                    "{Method} - Can not create {sceneFolderName}",
                    nameof(CreateFolders),
                    "Scenes");

                return (false, bundleFolderPath, levelFolderPath, entryFolderPath, default);
            }

            return (true, bundleFolderPath, levelFolderPath, entryFolderPath, sceneFolderPath);
        }

        private static (Scene, string) CreateSceneAndSave(string sceneFolderPath, string entryFolderPath, string sceneName, string bundleId)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var sceneFilePath = Path.Combine(sceneFolderPath, sceneName);

            // Delegate to editor bridge to create scene objects
            CreatorCrossEditorBridge.SceneCreation?.Invoke(scene, entryFolderPath, bundleId);

            EditorSceneManager.SaveScene(scene, sceneFilePath);

            return (scene, sceneFilePath);
        }
    }
}
