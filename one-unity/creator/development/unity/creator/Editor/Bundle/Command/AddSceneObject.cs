using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEditor;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    using CreatorCrossEditorBridge = TPFive.Creator.Cross.Editor.Bridge;

    /// <summary>
    /// Add new level to the project. This created a predefined folder structure and a scene.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class AddSceneObject
    {
        [MenuItem("TPFive/Creator/Add Scene Object")]
        internal static void Handle()
        {
            var bundleId = System.Guid.NewGuid().ToString();
            var bundleFolderName = $"Bundle - {bundleId}";

            // Create scene object folder
            var (success, bundleFolderPath, sceneObjectFolderPath) =
                BundleHandlingUtility.CreateFolders(
                    Logger,
                    bundleFolderName,
                    "Scene Object");

            if (!success)
            {
                Logger.LogWarning(
                    "{Method} Can not create folders",
                    nameof(Handle));

                return;
            }

            // Create AddressableImportSettings
            var (aisSuccess, ais) = BundleHandlingUtility.CreateAddressableImportSettings(bundleFolderPath);
            if (!aisSuccess)
            {
                Logger.LogWarning(
                    "{Method} Can not create AddressableImportSettings",
                    nameof(Handle));

                return;
            }

            var thumbnailPath = Path.Combine(sceneObjectFolderPath, Define.ThumbnailIconName);
            var defaultThumbnailPath = Define.BundleSceneObjectIconPath;
            var createThumbnailResult = AssetDatabase.CopyAsset(defaultThumbnailPath, thumbnailPath);
            if (!createThumbnailResult)
            {
                Logger.LogError(
                    "{Method} Can not create thumbnail",
                    nameof(Handle));

                return;
            }

            // Create Mapping Layer Data
            BundleHandlingUtility.CreateLayerMapping(sceneObjectFolderPath);

            // Create Bundle Detail Data
            BundleHandlingUtility.CreateBundleDetailData(
                bundleId,
                bundleFolderPath,
                thumbnailPath,
                "New Scene Object",
                "Newly created scene object",
                BundleKind.SceneObject,
                new List<UnityEditor.GUID>());

            AssetDatabase.SaveAssets();
        }
    }
}
