using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    [EditorUseLogging]
    public sealed partial class AddressablePlacement
    {
        [MenuItem("TPFive/Creator/Addressable Placement")]
        internal static void Handle()
        {
            var bundleDetailDatas = TPFive.Creator.Editor.Utility.GetUnityAssetCollectionOfType<BundleDetailData>();

            var aisList = new List<(string, AddressableImportSettings)>();
            foreach (var sdd in bundleDetailDatas)
            {
                var assetPath = AssetDatabase.GetAssetPath(sdd);
                var path = assetPath.Replace($"/{sdd.name}.asset", "");

                var ais = Utility.GetAddressableImportSettingsBySiblingAsset(sdd);
                if (ais == null)
                {
                    continue;
                }

                aisList.Add((path, ais));
            }

            var aisl = TPFive.Creator.Editor.Utility.GetUnityAssetOfType<AddressableImportSettingsList>();

            aisl.SettingList.Clear();
            var aisCollection = aisList.Select(x => x.Item2);
            aisl.SettingList.AddRange(aisCollection);

            // Reimport to make group create
            var parentPaths = aisList
                .Select(x => x.Item1)
                .ToArray();

            AddressableImporter.FolderImporter.ReimportFolders(
                parentPaths,
                showConfirmDialog: false);
        }
    }
}
