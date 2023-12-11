using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    // TODO: Need to extract the logic later for further use.
    [EditorUseLogging]
    public sealed partial class CreateAddressable
    {
        internal static void Handle(BundleDetailData bundleDetailData)
        {
            // Get the current settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var profileId = settings.profileSettings.GetProfileId("Remote");
            settings.profileSettings.SetValue(profileId, "SceneId", bundleDetailData.id);
            Logger.LogDebug($"SceneId: {bundleDetailData.id}");

            settings.activeProfileId = profileId;

            var buildPath = Path
                .Combine("ServerData", "[SceneId]", "[BuildTarget]")
                .Replace("\\", "/");
            var baseUrl = "«PrefixPath»";
            var intermediatePath = "«IntermediatePath»";
            var sceneId = "[SceneId]";
            var loadPath = Path
                .Combine(baseUrl, sceneId, intermediatePath, "[BuildTarget]")
                .Replace("\\", "/");

            Logger.LogDebug("settings.groups.Count: {Count}", settings.groups.Count);
            foreach (var group in settings.groups)
            {
                foreach (var schema in group.Schemas)
                {
                    Logger.LogDebug("schema.name: {name}", schema.name);
                }

                Logger.LogDebug("buildPath: {buildPath}\nloadPath: {loadPath}", buildPath, loadPath);


                var bundledAssetGroupSchema =
                    group.GetSchema<BundledAssetGroupSchema>();

                settings.profileSettings.SetValue(profileId, "Remote.BuildPath", buildPath);
                settings.profileSettings.SetValue(profileId, "Remote.LoadPath", loadPath);

                var remoteBuildPath = settings.profileSettings.GetValueByName(profileId, "Remote.BuildPath");
                var remoteLoadPath = settings.profileSettings.GetValueByName(profileId, "Remote.LoadPath");

                Logger.LogDebug($"remoteBuildPath: {remoteBuildPath}");
                Logger.LogDebug($"remoteLoadPath: {remoteLoadPath}");

                var variableNames = settings.profileSettings.GetVariableNames();
                variableNames.ForEach(x =>
                {
                    Logger.LogDebug("name x: {x}", x);
                });

                var variableIds = settings.profileSettings.GetAllVariableIds();
                variableIds.ToList().ForEach(x =>
                {
                    Logger.LogDebug("id x: {x}", x);
                });

                bundledAssetGroupSchema.BuildPath.SetVariableByName(settings, "Remote.BuildPath");
                bundledAssetGroupSchema.LoadPath.SetVariableByName(settings, "Remote.LoadPath");
            }
        }
    }
}
