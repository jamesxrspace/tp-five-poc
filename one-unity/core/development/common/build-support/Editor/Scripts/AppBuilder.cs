using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace TPFive.BuildSupport.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    [EditorUseLogging]
    public partial class AppBuilder
    {
        private const string AssistEntry = "AssistEntry";
        private static readonly string GamePath = Path.Combine("Assets", "_", "1 - Game");
        private static readonly string AppEntrySettingsPath = Path.Combine(GamePath, "Scene - App", "Entry", "Data Assets", "Settings - Entry.asset");
        private static readonly string AssistFolderPath = Path.Combine(GamePath, "Scene - Assist");

        private static AddAndRemoveRequest _addAndRemoveRequest;
        private static bool _atRemovePackageStage = false;

        [MenuItem("TPFive/Build/Remove Assist Scene", priority = 100)]
        public static void RemoveAssistScene()
        {
            try
            {
                // Remove addressables
                var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
                var groupToRemoveList = addressableAssetSettings.groups
                    .Where(group => group.Name.Contains("Scene - Assist")).ToList();
                foreach (var group in groupToRemoveList)
                {
                    addressableAssetSettings.RemoveGroup(group);
                }

                // Remove scene
                FileUtil.DeleteFileOrDirectory(AssistFolderPath);

                // Remove settings
                var appEntrySettings =
                    AssetDatabase.LoadAssetAtPath<TPFive.Game.App.Entry.Settings>(AppEntrySettingsPath);
                if (appEntrySettings != null)
                {
                    var assistEntryItems =
                        appEntrySettings.sceneLists
                            .SelectMany(x => x.SceneList.SceneProperties.Select(x => x))
                            .Where(x => x.title.Equals(AssistEntry));
                    var sceneProperties = assistEntryItems.ToList();
                    if (sceneProperties.Any())
                    {
                        sceneProperties.ToList().ForEach(x => x.load = false);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }

                // Remove packages
                // TODO: Add config if packages grow to certain number.
                _atRemovePackageStage = true;

                EditorApplication.update += PackageRemovalProgress;
                EditorApplication.LockReloadAssemblies();

                _addAndRemoveRequest = Client.AddAndRemove(
                    null,
                    new string[]
                    {
                        "io.xrspace.tpfive.creator.asset",
                        "io.xrspace.tpfive.game.assist.entry",
                        "io.xrspace.tpfive.game.assist",
                        "io.xrspace.tpfive.extended.quantum-console",
                    });
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }
            finally
            {
                if (_atRemovePackageStage)
                {
                    ResetPackageProgress();
                }
            }
        }

        private static void PackageRemovalProgress()
        {
            if (_addAndRemoveRequest is { IsCompleted: true })
            {
                switch (_addAndRemoveRequest.Status)
                {
                    case StatusCode.Failure:
                        Logger.LogError(
                            "{Method} Remove error: {Message}",
                            nameof(PackageRemovalProgress),
                            _addAndRemoveRequest.Error.message);
                        break;
                    case StatusCode.InProgress:
                        break;
                    case StatusCode.Success:
                        Logger.LogDebug(
                            "{Method} Remove success",
                            nameof(PackageRemovalProgress));

                        ResetPackageProgress();

                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }
        }

        private static void ResetPackageProgress()
        {
            EditorApplication.update -= PackageRemovalProgress;
            EditorApplication.UnlockReloadAssemblies();
            _atRemovePackageStage = true;
        }
    }
}
