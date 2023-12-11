using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.Creator.Editor;

    public class BundleHandlingUtility
    {
        public static (bool, AddressableImportSettings) CreateAddressableImportSettings(string bundleFolderPath)
        {
            var ais = ScriptableObject.CreateInstance<AddressableImportSettings>();
            var aisPath = Path.Combine(bundleFolderPath, "AddressableImportSettings.asset").Replace("\\", "/");
            var aagt = TPFive.Creator.Editor.Utility.GetUnityAssetOfType<AddressableAssetGroupTemplate>();
            var (index, aisParentPath) = TPFive.Creator.Editor.Utility.GetAdjustedPathWithIndex(aisPath);
            if (index < 0)
            {
                return (false, default);
            }

            ais.allowGroupCreation = true;
            ais.rules = Utility.GetAddressableImportRules(
                aisParentPath,
                index,
                aagt).ToList();

            AssetDatabase.CreateAsset(ais, aisPath);

            return (true, ais);
        }

        public static void CreateLayerMapping(string mappingLayerParentPath)
        {
            var mld = ScriptableObject.CreateInstance<MappingLayerData>();
            mld.mappingList = new List<Mapping>()
            {
                // Using layer 21 as the first layer for content project. This needs to be
                // discussed as it affects not just content project.
                new Mapping { key = 21, name = "Layer 1" },
                new Mapping { key = 22, name = "Layer 2" },
                new Mapping { key = 23, name = "Layer 3" },
                new Mapping { key = 24, name = "Layer 4" },
                new Mapping { key = 25, name = "Layer 5" },
                new Mapping { key = 26, name = "Layer 6" },
                new Mapping { key = 27, name = "Layer 7" },
                new Mapping { key = 28, name = "Layer 8" }
            };
            var mldPath = Path.Combine(mappingLayerParentPath, "Mapping Layer Data.asset");
            AssetDatabase.CreateAsset(mld, mldPath);
        }

        public static void CreateBundleDetailData(
            string bundleId,
            string bundleFolderPath,
            string thumbnailPath,
            string title,
            string description,
            BundleKind bundleKind,
            IReadOnlyList<UnityEditor.GUID> sceneGuids)
        {
            var dd = ScriptableObject.CreateInstance<BundleDetailData>();
            dd.id = bundleId;
            dd.title = title;
            dd.bundleUsage = BundleUsage.DesignUse;
            dd.bundleKind = bundleKind;
            dd.storageKind = StorageKind.Remote;
            dd.outcomeFormat = OutcomeFormat.Addressable;
            dd.description = description;
            // Will remove this soon to adopt the use of asset reference.
            dd.thumbnail = AssetDatabase.LoadAssetAtPath<Texture>(thumbnailPath);
            dd.scenes = new List<AssetReference>();
            var ddPath = Path.Combine(bundleFolderPath, $"{bundleId}.asset");
            foreach (var sceneGuid in sceneGuids)
            {
                dd.scenes.Add(new AssetReference(sceneGuid.ToString()));
            }

            // Add thumbnail into asset reference list.
            var thumbnailGuid = AssetDatabase.GUIDFromAssetPath(thumbnailPath);
            dd.thumbnails = new List<AssetReferenceTexture>();
            dd.thumbnails.Add(new AssetReferenceTexture(thumbnailGuid.ToString()));

            AssetDatabase.CreateAsset(dd, ddPath);
        }

        public static (bool, string, string) CreateFolders(
            Microsoft.Extensions.Logging.ILogger logger,
            string bundleFolderName,
            string bundleSubFolderName)
        {
            var (bundleFolderGuid, bundleFolderPath) = Utility.CreateFolder(Define.ContentPath, bundleFolderName);
            if (string.IsNullOrEmpty(bundleFolderGuid))
            {
                logger.LogWarning(
                    "{Method} - Can not create {bundleFolderName}",
                    nameof(CreateFolders),
                    bundleFolderName);

                return (false, default, default);
            }

            var (particleFolderGuid, bundleSubFolderPath) = Utility.CreateFolder(bundleFolderPath, bundleSubFolderName);
            if (string.IsNullOrEmpty(particleFolderGuid))
            {
                logger.LogWarning(
                    "{Method} - Can not create {bundleSubFolderName}",
                    nameof(CreateFolders),
                    bundleSubFolderName);

                return (false, bundleFolderPath, default);
            }

            // These are optional folders, if these can not be created, just log a warning
            var (prefabFolderGuid, prefabFolderPath) = Utility.CreateFolder(bundleSubFolderPath, "Prefabs");
            if (string.IsNullOrEmpty(prefabFolderGuid))
            {
                logger.LogWarning(
                    "{Method} - Can not create {prefabFolderName}",
                    nameof(CreateFolders),
                    "Prefabs");
            }

            var (textureFolderGuid, textureFolderPath) = Utility.CreateFolder(bundleSubFolderPath, "Textures");
            if (string.IsNullOrEmpty(textureFolderGuid))
            {
                logger.LogWarning(
                    "{Method} - Can not create {textureFolderName}",
                    nameof(CreateFolders),
                    "Textures");
            }

            var (materialFolderGuid, materialFolderPath) = Utility.CreateFolder(bundleSubFolderPath, "Materials");
            if (string.IsNullOrEmpty(textureFolderGuid))
            {
                logger.LogWarning(
                    "{Method} - Can not create {materialFolderName}",
                    nameof(CreateFolders),
                    "Materials");
            }

            return (true, bundleFolderPath, bundleSubFolderPath);
        }
    }
}
