using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    /// <summary>
    /// Add profile to Addressables named "Remote"
    /// </summary>
    [EditorUseLogging]
    public sealed partial class AddProfile
    {
        /// <summary>
        /// While setting up profile, create one variable named "SceneId" with the build path and load path
        /// set to predefined format.
        /// </summary>
        [MenuItem("TPFive/Creator/Add Profile")]
        internal static void Handle()
        {
            // Get the current settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var defaultProfileId = settings.profileSettings.GetProfileId("Default");
            var remoteProfileId = settings.profileSettings.AddProfile("Remote", defaultProfileId);

            settings.activeProfileId = remoteProfileId;

            settings.profileSettings.CreateValue("SceneId", "");

            var buildPath = Path
                .Combine("ServerData", "[SceneId]", "[BuildTarget]")
                .Replace("\\", "/");

            var baseUrl = "«PrefixPath»";
            var intermediatePath = "«IntermediatePath»";
            var sceneId = "[SceneId]";
            var buildTarget = "[BuildTarget]";
            var loadPath = Path
                .Combine(baseUrl, sceneId, intermediatePath, buildTarget)
                .Replace("\\", "/");

            settings.profileSettings.SetValue(remoteProfileId, "Remote.BuildPath", buildPath);
            settings.profileSettings.SetValue(remoteProfileId, "Remote.LoadPath", loadPath);

            settings.RemoteCatalogBuildPath = new ProfileValueReference();
        }
    }
}
