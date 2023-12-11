using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    /// <summary>
    /// Place specific scene folder into Addressable as a group. To make the build later without the predefined
    /// default group. This removes all other group inside Addressable.
    /// </summary>
    /// <remarks>
    /// Remove all other group but keep only the specified one is unique to the build inside a project that has
    /// to create a catalog for each specified scene.
    /// </remarks>
    [EditorUseLogging]
    public sealed partial class SetupAddressableContent
    {
        public static System.Action Handle(BundleDetailData bundleDetailData)
        {
            return () =>
            {
                // Get the current settings
                var settings = AddressableAssetSettingsDefaultObject.Settings;

                {
                    var groupToRemoveList = new List<AddressableAssetGroup>();
                    foreach (var group in settings.groups)
                    {
                        groupToRemoveList.Add(group);
                    }

                    Logger.LogDebug("groupToRemoveList count: {Count}", groupToRemoveList.Count);
                    foreach (var t in groupToRemoveList)
                    {
                        settings.RemoveGroup(t);
                    }

                    AssetDatabase.Refresh();
                }

                // Get the path containing bundle detail data
                var path = AssetDatabase.GetAssetPath(bundleDetailData);
                var parentPath = Path.GetDirectoryName(path);

                Logger.LogDebug("path: {path} parentPath: {parentPath}", path, parentPath);

                var addressableImportSettingsList = TPFive.Creator.Editor.Utility.GetUnityAssetOfType<AddressableImportSettingsList>();

                addressableImportSettingsList.SettingList.Clear();
                var ais = Utility.GetAddressableImportSettingsBySiblingAsset(bundleDetailData);
                if (ais == null)
                {
                    return;
                }
                addressableImportSettingsList.SettingList.Add(ais);

                // Reimport folder
                AddressableImporter.FolderImporter.ReimportFolders(new[] { parentPath }, showConfirmDialog: false);

                // Remove addressable group
                {
                    var groupToRemoveList = new List<AddressableAssetGroup>();
                    foreach (var group in settings.groups)
                    {
                        if (group.Name.Contains("Default Local Group"))
                        {
                            groupToRemoveList.Add(group);
                        }
                    }

                    Logger.LogDebug("groupToRemoveList count: {Count}", groupToRemoveList.Count);
                    foreach (var group in groupToRemoveList)
                    {
                        settings.RemoveGroup(group);
                    }
                }

                var profileId = settings.profileSettings.GetProfileId("Remote");

                foreach (var group in settings.groups)
                {
                    var schema = group.GetSchema<BundledAssetGroupSchema>();
                    schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZMA;
                    var buildPathName = schema.BuildPath.GetName(settings);
                    var loadPathName = schema.LoadPath.GetName(settings);

                    schema.BuildPath.SetVariableByName(settings, "Remote.BuildPath");
                    schema.LoadPath.SetVariableByName(settings, "Remote.LoadPath");
                }

                settings.profileSettings.SetValue(profileId, "SceneId", bundleDetailData.id);

                settings.BuildRemoteCatalog = true;
                settings.OverridePlayerVersion = "latest";
                var profileValueReferenceBuild = new ProfileValueReference();
                profileValueReferenceBuild.SetVariableByName(settings, "Remote.BuildPath");
                var profileValueReferenceLoad = new ProfileValueReference();
                profileValueReferenceLoad.SetVariableByName(settings, "Remote.LoadPath");
                settings.RemoteCatalogBuildPath = profileValueReferenceBuild;
                settings.RemoteCatalogLoadPath = profileValueReferenceLoad;
            };
        }
    }
}
