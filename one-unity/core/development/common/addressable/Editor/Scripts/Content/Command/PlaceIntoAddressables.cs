using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Splat;
using UnityEditor;
using UnityEngine;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Addressable.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    /// <summary>
    /// Upload a file to remote location using command line tool. The file refereed here is unitypackage.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class PlaceIntoAddressables
    {
        internal static void Handle(IEnumerable<string> ids)
        {
            var sdds = TPFive.Game.Editor.Utility
                .GetUnityAssetCollectionOfType<TPFive.Creator.BundleDetailData>();

            // TODO: Extract SpaceDetailData to upper level
            var spaceDetailDatas =
                ids
                    .Select(id =>
                    {
                        var spaceDetailData = sdds.FirstOrDefault(x => x.id == id);

                        return spaceDetailData;
                    })
                    .Where(x => x != null)
                    .ToList();
            if (!spaceDetailDatas.Any())
            {
                Debug.LogWarning($"No SpaceDetailData found");
                return;
            }

            var addressableImportSettingsList = TPFive.Game.Editor.Utility.GetUnityAssetOfType<AddressableImportSettingsList>();

            // Cache existing settings
            var previousSettingList = new List<AddressableImportSettings>();
            previousSettingList.AddRange(addressableImportSettingsList.SettingList);
            addressableImportSettingsList.SettingList.Clear();
            addressableImportSettingsList.SettingList.AddRange(
                spaceDetailDatas
                    .Select(TPFive.Creator.Bundle.Command.Editor.Utility.GetAddressableImportSettingsBySiblingAsset)
                    .Where(x => x != null));

            // Reimport folder
            var parentPaths = spaceDetailDatas
                .Select(sdd => Path.GetDirectoryName(AssetDatabase.GetAssetPath(sdd)))
                .Distinct();
            AddressableImporter.FolderImporter.ReimportFolders(parentPaths, showConfirmDialog: false);

            // Restore back to original settings
            addressableImportSettingsList.SettingList.Clear();
            addressableImportSettingsList.SettingList.AddRange(previousSettingList);
        }
    }
}
