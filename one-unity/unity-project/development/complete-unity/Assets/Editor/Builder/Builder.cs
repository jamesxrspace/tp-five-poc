using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;

namespace TPFive.Build.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    [EditorUseLogging]
    public partial class Builder
    {
        private static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static readonly string APKPath = Path.Combine(ProjectPath, $"Oculus-Builds/{Application.productName}.apk");

        [MenuItem("TPFive/Build/Oculus", priority = 1)]
        public static void BuildOculus()
        {
            BuilOculus();
        }

        [MenuItem("TPFive/Build/Oculus (DryRun)", priority = 200)]
        public static void BuildOculusDryRun()
        {
            BuilOculus(true);
        }

        private static void BuilOculus(bool dryRun = false)
        {
            Debug.Log($"BuildOculus - {nameof(dryRun)}:{dryRun}");

            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            // Gather values from project
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray();

            // Oculus has some performance issue. Leave only lowest quality setting for now.
            System.Exception err;
            for (int i = 0; i < QualitySettings.count; ++i)
            {
                if (QualitySettings.names[i] == "Oculus")
                {
                    if (QualitySettings.TryIncludePlatformAt("Android", i, out err))
                    {
                        QualitySettings.SetQualityLevel(i);
                    }
                }
                else
                {
                    QualitySettings.TryExcludePlatformAt("Android", i, out err);
                }

                if (err != null)
                {
                    throw err;
                }
            }

#if USING_XR_MANAGEMENT
            EnableToInitializeXROnStartup(BuildTargetGroup.Android, true);
#endif

            // Define BuildPlayer Options
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = APKPath,
                target = BuildTarget.Android,
                options = BuildOptions.None,
                extraScriptingDefines = new string[] { "OCULUS_VR" },
                subtarget = (int)StandaloneBuildSubtarget.Player
            };
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);
            PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.Android, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed);
            TPFive.BuildSupport.Editor.AppBuilder.ConfigureLogStackTraceSettings(
                TPFive.BuildSupport.Editor.AppBuilder.LogStackTraceSettings);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EnforceOculusAndroidSettings();

            if (!dryRun)
            {
                BuildAddressables();

                // Perform build
                BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

                if (buildReport.summary.result != BuildResult.Succeeded)
                    throw new UnityEditor.Build.BuildFailedException("Build failed");
            }

            Debug.Log("-- Android VR Build: SUCCESSFUL --");
        }

        private static void EnforceOculusAndroidSettings()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                return;

            if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.LandscapeLeft)
            {
                Debug.Log("OVREngineConfigurationUpdater: Setting orientation to Landscape Left");
                // Default screen orientation must be set to landscape left.
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            }

#pragma warning disable 618
            if (!PlayerSettings.virtualRealitySupported)
#pragma warning restore 618
            {
                // NOTE: This value should not affect the main window surface
                // when Built-in VR support is enabled.

                // NOTE: On Adreno Lollipop, it is an error to have antiAliasing set on the
                // main window surface with front buffer rendering enabled. The view will
                // render black.
                // On Adreno KitKat, some tiling control modes will cause the view to render
                // black.
                if (QualitySettings.antiAliasing != 0 && QualitySettings.antiAliasing != 1)
                {
                    Debug.Log("OVREngineConfigurationUpdater: Disabling antiAliasing");
                    QualitySettings.antiAliasing = 1;
                }
            }

            if (QualitySettings.vSyncCount != 0)
            {
                Debug.Log("OVREngineConfigurationUpdater: Setting vsyncCount to 0");
                // We sync in the TimeWarp, so we don't want unity syncing elsewhere.
                QualitySettings.vSyncCount = 0;
            }
        }


        private static void BuildAddressables()
        {
            AddressableAssetSettings.CleanPlayerContent(
                AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);

            BuildCache.PurgeCache(true);

            AddressableAssetProfileSettings profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
            var profileId = profileSettings.GetProfileId("Default");
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;
            AddressableAssetSettings.BuildPlayerContent();
        }

#if USING_XR_MANAGEMENT
        private static void EnableToInitializeXROnStartup(BuildTargetGroup targetGroup, bool enabled)
        {
            Debug.Log($"{(enabled ? "Enable" : "Disable")} \"Initialize XR On Startup\" for {targetGroup}");

            UnityEngine.XR.XRSettings.enabled = enabled;
            var settings = UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(targetGroup);
            if (settings != null)
            {
                settings.InitManagerOnStart = enabled;
            }
        }
#endif
    }
}
