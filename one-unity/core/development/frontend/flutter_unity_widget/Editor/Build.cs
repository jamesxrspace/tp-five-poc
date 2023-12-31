﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;

namespace FlutterUnityIntegration.Editor
{
    public class Build : EditorWindow
    {
        private static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static readonly string APKPath = Path.Combine(ProjectPath, "Builds/" + Application.productName + ".apk");

        private static readonly string AndroidExportPath = Path.GetFullPath(Path.Combine(ProjectPath, "exposed_flutter_unity_widget/android/unityLibrary"));
        private static readonly string WindowsExportPath = Path.GetFullPath(Path.Combine(ProjectPath, "exposed_flutter_unity_widget/windows/unityLibrary/data"));
        private static readonly string IOSExportPath = Path.GetFullPath(Path.Combine(ProjectPath, "exposed_flutter_unity_widget/ios/UnityLibrary"));
        private static readonly string WebExportPath = Path.GetFullPath(Path.Combine(ProjectPath, "exposed_flutter_unity_widget/web/UnityLibrary"));
        private static readonly string IOSExportPluginPath = Path.GetFullPath(Path.Combine(ProjectPath, "exposed_flutter_unity_widget/ios_xcode/UnityLibrary"));

        private bool _pluginMode = false;
        private static string _persistentKey = "flutter-unity-widget-pluginMode";

        //#region GUI Member Methods
        [MenuItem("Flutter/Export Android (Debug) %&n", false, 101)]
        public static void DoBuildAndroidLibraryDebug()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            DoBuildAndroid(Path.Combine(APKPath, "unityLibrary"), false, false);

            // Copy over resources from the launcher module that are used by the library
            Copy(Path.Combine(APKPath + "/launcher/src/main/res"), Path.Combine(AndroidExportPath, "src/main/res"));
        }

        [MenuItem("Flutter/Export Android (Release) %&m", false, 102)]
        public static void DoBuildAndroidLibraryRelease()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            DoBuildAndroid(Path.Combine(APKPath, "unityLibrary"), false, true);

            // Copy over resources from the launcher module that are used by the library
            Copy(Path.Combine(APKPath + "/launcher/src/main/res"), Path.Combine(AndroidExportPath, "src/main/res"));
        }

        [MenuItem("Flutter/Export Android Plugin %&p", false, 103)]
        public static void DoBuildAndroidPlugin()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            DoBuildAndroid(Path.Combine(APKPath, "unityLibrary"), true, true);

            // Copy over resources from the launcher module that are used by the library
            Copy(Path.Combine(APKPath + "/launcher/src/main/res"), Path.Combine(AndroidExportPath, "src/main/res"));
        }

        [MenuItem("Flutter/Export IOS (Debug) %&i", false, 201)]
        public static void DoBuildIOSDebug()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            BuildIOS(IOSExportPath, false);
        }

        [MenuItem("Flutter/Export IOS (Release) %&i", false, 202)]
        public static void DoBuildIOSRelease()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            BuildIOS(IOSExportPath, true);
        }

        [MenuItem("Flutter/Export IOS Plugin %&o", false, 203)]
        public static void DoBuildIOSPlugin()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            BuildIOS(IOSExportPluginPath, true);

            // Automate so manual steps
            SetupIOSProjectForPlugin();

            // Build Archive
            // BuildUnityFrameworkArchive();

        }

        [MenuItem("Flutter/Export Web GL %&w", false, 301)]
        public static void DoBuildWebGL()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            BuildWebGL(WebExportPath);
        }

        [MenuItem("Flutter/Export Windows %&d", false, 401)]
        public static void DoBuildWindowsOS()
        {
            TPFive.BuildSupport.Editor.AppBuilder.RemoveAssistScene();

            BuildWindowsOS(WindowsExportPath);
        }

        [MenuItem("Flutter/Settings %&S", false, 501)]
        public static void PluginSettings()
        {
            EditorWindow.GetWindow(typeof(Build));
        }

        private void OnGUI()
        {
            GUILayout.Label("Flutter Unity Widget Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            _pluginMode = EditorGUILayout.Toggle("Plugin Mode", _pluginMode);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(_persistentKey, _pluginMode);
            }
        }

        private void OnEnable()
        {
            _pluginMode = EditorPrefs.GetBool(_persistentKey, false);
        }
        //#endregion


        //#region Build Member Methods

        private static void BuildWindowsOS(String path)
        {
            // Switch to Android standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            if (Directory.Exists(WindowsExportPath))
                Directory.Delete(WindowsExportPath, true);

            TPFive.BuildSupport.Editor.AppBuilder.ConfigureLogStackTraceSettings(
                TPFive.BuildSupport.Editor.AppBuilder.LogStackTraceSettings);

            var playerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                target = BuildTarget.StandaloneWindows64,
                locationPathName = path,
                options = BuildOptions.AllowDebugging
            };

            // Switch to Android standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

            // build addressable
            ExportAddressables();
            var report = BuildPipeline.BuildPlayer(playerOptions);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Build failed");

            Debug.Log("-- Windows Build: SUCCESSFUL --");
        }

        private static void BuildWebGL(String path)
        {
            // Switch to Android standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            if (Directory.Exists(WebExportPath))
                Directory.Delete(WebExportPath, true);

            // EditorUserBuildSettings. = true;

            TPFive.BuildSupport.Editor.AppBuilder.ConfigureLogStackTraceSettings(
                TPFive.BuildSupport.Editor.AppBuilder.LogStackTraceSettings);

            var playerOptions = new BuildPlayerOptions();
            playerOptions.scenes = GetEnabledScenes();
            playerOptions.target = BuildTarget.WebGL;
            playerOptions.locationPathName = path;

            // Switch to Android standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            // build addressable
            ExportAddressables();
            var report = BuildPipeline.BuildPlayer(playerOptions);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Build failed");

            // Copy(path, WebExportPath);
            ModifyWebGLExport();

            Debug.Log("-- WebGL Build: SUCCESSFUL --");
        }

        private static void DoBuildAndroid(String buildPath, bool isPlugin, bool isReleaseBuild)
        {
            // Switch to Android standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            if (Directory.Exists(APKPath))
                Directory.Delete(APKPath, true);

            if (Directory.Exists(AndroidExportPath))
                Directory.Delete(AndroidExportPath, true);

            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            var playerOptions = new BuildPlayerOptions();
            playerOptions.scenes = GetEnabledScenes();
            playerOptions.target = BuildTarget.Android;
            playerOptions.locationPathName = APKPath;
            playerOptions.extraScriptingDefines = new string[] { "FLUTTER" };
            if (!isReleaseBuild)
            {
                // remove this line if you don't use a debugger and you want to speed up the flutter build
                // Add `AllowDebugging` option makes app crash. Please don't add it.
                playerOptions.options = BuildOptions.Development;
            }
            #if UNITY_2022_1_OR_NEWER
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, isReleaseBuild ? Il2CppCompilerConfiguration.Release : Il2CppCompilerConfiguration.Debug);
                // Refer to https://github.com/homuler/MediaPipeUnityPlugin/issues/898
                // Need to set this to OptimizeSpeed to prevent code being stripped.
                PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.Android, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed);
            #elif UNITY_2021_2_OR_NEWER
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, isReleaseBuild ? Il2CppCompilerConfiguration.Release : Il2CppCompilerConfiguration.Debug);
                EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
            #endif

            TPFive.BuildSupport.Editor.AppBuilder.ConfigureLogStackTraceSettings(
                TPFive.BuildSupport.Editor.AppBuilder.LogStackTraceSettings);

#if USING_XR_MANAGEMENT
            EnableToInitializeXROnStartup(BuildTargetGroup.Android, false);
#endif

            // Switch to Android standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            // build addressable
            ExportAddressables();
            var report = BuildPipeline.BuildPlayer(playerOptions);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Build failed");

            Copy(buildPath, AndroidExportPath);

            // Modify build.gradle
            ModifyAndroidGradle(isPlugin);

            // [TFI-63] Skip modify the build.gradle
            // if(isPlugin)
            // {
            //     SetupAndroidProjectForPlugin();
            // } else
            // {
            //     SetupAndroidProject();
            // }

            if (isReleaseBuild) {
                Debug.Log($"-- Android Release Build: SUCCESSFUL --");
            } else
            {
                Debug.Log($"-- Android Debug Build: SUCCESSFUL --");
            }
        }

        private static void ModifyWebGLExport()
        {
            // Modify index.html
            var indexFile = Path.Combine(WebExportPath, "index.html");
            var indexHtmlText = File.ReadAllText(indexFile);

            indexHtmlText = indexHtmlText.Replace("<script>", @"
    <script>
        var mainUnityInstance;

        window['handleUnityMessage'] = function (params) {
        window.parent.postMessage({
            name: 'onUnityMessage',
            data: params,
            }, '*');
        };

        window['handleUnitySceneLoaded'] = function (name, buildIndex, isLoaded, isValid) {
        window.parent.postMessage({
            name: 'onUnitySceneLoaded',
            data: {
                'name': name,
                'buildIndex': buildIndex,
                'isLoaded': isLoaded == 1,
                'isValid': isValid == 1,
            }
            }, '*');
        };

        window.parent.addEventListener('unityFlutterBiding', function (args) {
            const obj = JSON.parse(args.data);
            mainUnityInstance.SendMessage(obj.gameObject, obj.methodName, obj.message);
        });

        window.parent.addEventListener('unityFlutterBidingFnCal', function (args) {
            mainUnityInstance.SendMessage('GameManager', 'HandleWebFnCall', args);
        });
        ");

            indexHtmlText = indexHtmlText.Replace("canvas.style.width = \"960px\";", "canvas.style.width = \"100%\";");
			indexHtmlText = indexHtmlText.Replace("canvas.style.height = \"600px\";", "canvas.style.height = \"100%\";");

			indexHtmlText = indexHtmlText.Replace("}).then((unityInstance) => {", @"
         }).then((unityInstance) => {
           window.parent.postMessage('unityReady', '*');
           mainUnityInstance = unityInstance;
         ");
			File.WriteAllText(indexFile, indexHtmlText);

			/// Modidy style.css
			var cssFile = Path.Combine($"{WebExportPath}/TemplateData", "style.css");
			var fullScreenCss = File.ReadAllText(cssFile);
			fullScreenCss = @"
body { padding: 0; margin: 0; overflow: hidden; }
#unity-container { position: absolute }
#unity-container.unity-desktop { width: 100%; height: 100% }
#unity-container.unity-mobile { width: 100%; height: 100% }
#unity-canvas { background: #231F20 }
.unity-mobile #unity-canvas { width: 100%; height: 100% }
#unity-loading-bar { position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); display: none }
#unity-logo { width: 154px; height: 130px; background: url('unity-logo-dark.png') no-repeat center }
#unity-progress-bar-empty { width: 141px; height: 18px; margin-top: 10px; background: url('progress-bar-empty-dark.png') no-repeat center }
#unity-progress-bar-full { width: 0%; height: 18px; margin-top: 10px; background: url('progress-bar-full-dark.png') no-repeat center }
#unity-footer { display: none }
.unity-mobile #unity-footer { display: none }
#unity-webgl-logo { float:left; width: 204px; height: 38px; background: url('webgl-logo.png') no-repeat center }
#unity-build-title { float: right; margin-right: 10px; line-height: 38px; font-family: arial; font-size: 18px }
#unity-fullscreen-button { float: right; width: 38px; height: 38px; background: url('fullscreen-button.png') no-repeat center }
#unity-mobile-warning { position: absolute; left: 50%; top: 5%; transform: translate(-50%); background: white; padding: 10px; display: none }
            ";
			File.WriteAllText(cssFile, fullScreenCss);
        }

        private static void ModifyAndroidGradle(bool isPlugin)
        {
            // Modify build.gradle
            var buildFile = Path.Combine(AndroidExportPath, "build.gradle");
            var buildText = File.ReadAllText(buildFile);
            buildText = buildText.Replace("com.android.application", "com.android.library");
            buildText = buildText.Replace("bundle {", "splits {");
            buildText = buildText.Replace("enableSplit = false", "enable false");
            buildText = buildText.Replace("enableSplit = true", "enable true");
            buildText = buildText.Replace(
                "implementation fileTree(dir: 'libs', include: ['*.jar'])",
                "implementation(name: 'unity-classes', ext:'jar')\n    implementation fileTree(dir: 'libs', include: ['*.jar'], exclude: ['guava*.jar', 'unity-classes.jar'])");
            buildText = buildText.Replace(" + unityStreamingAssets.tokenize(', ')", "");

            if(isPlugin)
            {
                buildText = Regex.Replace(buildText, @"implementation\(name: 'androidx.* ext:'aar'\)", "\n");
            }

            buildText = Regex.Replace(buildText, @"\n.*applicationId '.+'.*\n", "\n");
            File.WriteAllText(buildFile, buildText);

            // Modify AndroidManifest.xml
            var manifestFile = Path.Combine(AndroidExportPath, "src/main/AndroidManifest.xml");
            var manifestText = File.ReadAllText(manifestFile);
            manifestText = Regex.Replace(manifestText, @"<application .*>", "<application>");
            var regex = new Regex(@"<activity.*>(\s|\S)+?</activity>", RegexOptions.Multiline);
            manifestText = regex.Replace(manifestText, "");
            File.WriteAllText(manifestFile, manifestText);

            // Modify proguard-unity.txt
            var proguardFile = Path.Combine(AndroidExportPath, "proguard-unity.txt");
            var proguardText = File.ReadAllText(proguardFile);
            var keptClasses = new string[] {
                "-keep class com.xraph.plugin.** { *; }",
                "-keep class com.unity3d.plugin.* { *; }",
                "-keep class com.xrspace.xrauth.** { *; }",
                "-keep class io.agora.** { *; }",
                "-keep class api.natsuite.natcorder.** { *; }",
                "-keep class com.besthttp.** { *; }",
                "-keep class com.renderheads.AVPro.Video.** { *; }",
                "-keep class com.google.android.exoplr2avp.** { *; }",
                "-keep class com.twobigears.audio360.** { *; }",
            };
            proguardText = proguardText.Replace("-ignorewarnings", $"{string.Join('\n', keptClasses)}\n-ignorewarnings");
            File.WriteAllText(proguardFile, proguardText);

            // Modify res/values/styles.xml
            // Prevent app crash upon loading. Reference: https://github.com/juicycleff/flutter-unity-view-widget/issues/516
            var stylesFile = Path.Combine(AndroidExportPath, "src/main/res/values/styles.xml");
            var stylesText = File.ReadAllText(stylesFile);
            stylesText = Regex.Replace(stylesText, @"<resources>", "<resources>\n<string name=\"game_view_content_description\">Game view</string>");
            File.WriteAllText(stylesFile, stylesText);
        }

        private static void BuildIOS(String path, bool isReleaseBuild)
        {
            // Switch to ios standalone build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            #if (UNITY_2021_1_OR_NEWER)
                EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Release;
            #else
                EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;
            #endif

            #if UNITY_2022_1_OR_NEWER
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.iOS, isReleaseBuild ? Il2CppCompilerConfiguration.Release : Il2CppCompilerConfiguration.Debug);
                PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.iOS, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed);
            #elif UNITY_2021_2_OR_NEWER
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.iOS, isReleaseBuild ? Il2CppCompilerConfiguration.Release : Il2CppCompilerConfiguration.Debug);
                EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
            #endif

            TPFive.BuildSupport.Editor.AppBuilder.ConfigureLogStackTraceSettings(
                TPFive.BuildSupport.Editor.AppBuilder.LogStackTraceSettings);

#if USING_XR_MANAGEMENT
            EnableToInitializeXROnStartup(BuildTargetGroup.iOS, false);
#endif

            var playerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                target = BuildTarget.iOS,
                locationPathName = path,
                extraScriptingDefines = new string[] { "FLUTTER" }
            };

            if (!isReleaseBuild)
            {
                playerOptions.options = BuildOptions.AllowDebugging | BuildOptions.Development;
            }

            // Switch to iOS.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            // build addressable
            ExportAddressables();

            var report = BuildPipeline.BuildPlayer(playerOptions);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Build failed");

            //trigger postbuild script manually
#if UNITY_IOS
            XcodePostBuild.PostBuild(BuildTarget.iOS, report.summary.outputPath);
#endif

            if (isReleaseBuild) {
                Debug.Log("-- iOS Release Build: SUCCESSFUL --");
            } else {
                Debug.Log("-- iOS Debug Build: SUCCESSFUL --");
            }
        }

        //#endregion


        //#region Other Member Methods
        private static void Copy(string source, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
                Directory.Delete(destinationPath, true);

            Directory.CreateDirectory(destinationPath);

            foreach (var dirPath in Directory.GetDirectories(source, "*",
                         SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, destinationPath));

            foreach (var newPath in Directory.GetFiles(source, "*.*",
                         SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, destinationPath), true);
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            return scenes;
        }

        private static void ExportAddressables() {
            Debug.Log("Start building player content (Addressables)");
            Debug.Log("BuildAddressablesProcessor.PreExport start");

            AddressableAssetSettings.CleanPlayerContent(
                AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            
            BuildCache.PurgeCache(true);

            AddressableAssetProfileSettings profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
            string profileId = profileSettings.GetProfileId("Default");
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;

            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("BuildAddressablesProcessor.PreExport done");
        }


        /// <summary>
        /// This method tries to autome the build setup required for Android
        /// </summary>
        private static void SetupAndroidProject()
        {
            var androidPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android"));
            var androidAppPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android/app"));
            var projBuildPath = Path.Combine(androidPath, "build.gradle");
            var appBuildPath = Path.Combine(androidAppPath, "build.gradle");
            var settingsPath = Path.Combine(androidPath, "settings.gradle");

            var projBuildScript = File.ReadAllText(projBuildPath);
            var settingsScript = File.ReadAllText(settingsPath);
            var appBuildScript = File.ReadAllText(appBuildPath);

            // Sets up the project build.gradle files correctly
            if (!Regex.IsMatch(projBuildScript, @"flatDir[^/]*[^}]*}"))
            {
                var regex = new Regex(@"allprojects \{[^\{]*\{", RegexOptions.Multiline);
                projBuildScript = regex.Replace(projBuildScript, @"
allprojects {
    repositories {
        flatDir {
            dirs ""${project(':unityLibrary').projectDir}/libs""
        }
");
                File.WriteAllText(projBuildPath, projBuildScript);
            }

            // Sets up the project settings.gradle files correctly
            if (!Regex.IsMatch(settingsScript, @"include "":unityLibrary"""))
            {
                settingsScript += @"

include "":unityLibrary""
project("":unityLibrary"").projectDir = file(""./unityLibrary"")
";
                File.WriteAllText(settingsPath, settingsScript);
            }


            // Sets up the project app build.gradle files correctly
            if (!Regex.IsMatch(appBuildScript, @"dependencies \{"))
            {
                appBuildScript += @"
dependencies {
    implementation project(':unityLibrary')
}
";
                File.WriteAllText(appBuildPath, appBuildScript);
            } else
            {
                if (!appBuildScript.Contains(@"implementation project(':unityLibrary')"))
                {
                    var regex = new Regex(@"dependencies \{", RegexOptions.Multiline);
                    appBuildScript = regex.Replace(appBuildScript, @"
dependencies {
    implementation project(':unityLibrary')
");
                    File.WriteAllText(appBuildPath, appBuildScript);
                }
            }
        }

        /// <summary>
        /// This method tries to autome the build setup required for Android
        /// </summary>
        private static void SetupAndroidProjectForPlugin()
        {
            var androidPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android"));
            var projBuildPath = Path.Combine(androidPath, "build.gradle");
            var settingsPath = Path.Combine(androidPath, "settings.gradle");

            var projBuildScript = File.ReadAllText(projBuildPath);
            var settingsScript = File.ReadAllText(settingsPath);

            // Sets up the project build.gradle files correctly
            if (Regex.IsMatch(projBuildScript, @"// BUILD_ADD_UNITY_LIBS"))
            {
                var regex = new Regex(@"// BUILD_ADD_UNITY_LIBS", RegexOptions.Multiline);
                projBuildScript = regex.Replace(projBuildScript, @"
        flatDir {
            dirs ""${project(':unityLibrary').projectDir}/libs""
        }
");
                File.WriteAllText(projBuildPath, projBuildScript);
            }

            // Sets up the project settings.gradle files correctly
            if (!Regex.IsMatch(settingsScript, @"include "":unityLibrary"""))
            {
                settingsScript += @"

include "":unityLibrary""
project("":unityLibrary"").projectDir = file(""./unityLibrary"")
";
                File.WriteAllText(settingsPath, settingsScript);
            }
        }

        private static void SetupIOSProjectForPlugin()
        {
            var iosRunnerPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../ios"));
            var pubsecFile = Path.Combine(iosRunnerPath, "flutter_unity_widget.podspec");
            var pubsecText = File.ReadAllText(pubsecFile);

            if (!Regex.IsMatch(pubsecText, @"\w\.xcconfig(?:[^}]*})+") && !Regex.IsMatch(pubsecText, @"tar -xvjf UnityFramework.tar.bz2"))
            {
                var regex = new Regex(@"\w\.xcconfig(?:[^}]*})+", RegexOptions.Multiline);
                pubsecText = regex.Replace(pubsecText, @"
	spec.xcconfig = {
        'FRAMEWORK_SEARCH_PATHS' => '""${PODS_ROOT}/../.symlinks/plugins/flutter_unity_widget/ios"" ""${PODS_ROOT}/../.symlinks/flutter/ios-release"" ""${PODS_CONFIGURATION_BUILD_DIR}""',
        'OTHER_LDFLAGS' => '$(inherited) -framework UnityFramework \${PODS_LIBRARIES}'
    }

    spec.vendored_frameworks = ""UnityFramework.framework""
			");
                File.WriteAllText(pubsecFile, pubsecText);
            }
        }

        // DO NOT USE (Contact before trying)
        private static async void BuildUnityFrameworkArchive()
        {
            var xcprojectExt = "/Unity-iPhone.xcodeproj";

            // check if we have a workspace or not
            if (Directory.Exists(IOSExportPluginPath + "/Unity-iPhone.xcworkspace")) {
                xcprojectExt = "/Unity-iPhone.xcworkspace";
            }

            const string framework = "UnityFramework";
            var xcprojectName = $"{IOSExportPluginPath}{xcprojectExt}";
            var schemeName = $"{framework}";
            var buildPath = IOSExportPluginPath + "/build";
            var frameworkNameWithExt = $"{framework}.framework";

            var iosRunnerPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../ios/"));
            const string iosArchiveDir = "Release-iphoneos-archive";
            var iosArchiveFrameworkPath = $"{buildPath}/{iosArchiveDir}/Products/Library/Frameworks/{frameworkNameWithExt}";
            var dysmNameWithExt = $"{frameworkNameWithExt}.dSYM";

            try
            {
                Debug.Log("### Cleaning up after old builds");
                await $" - rf {iosRunnerPath}{frameworkNameWithExt}".Bash("rm");
                await $" - rf {buildPath}".Bash("rm");

                Debug.Log("### BUILDING FOR iOS");
                Debug.Log("### Building for device (Archive)");

                await $"archive -workspace {xcprojectName} -scheme {schemeName} -sdk iphoneos -archivePath {buildPath}/Release-iphoneos.xcarchive ENABLE_BITCODE=NO |xcpretty".Bash("xcodebuild");

                Debug.Log("### Copying framework files");
                await $" -RL {iosArchiveFrameworkPath} {iosRunnerPath}/{frameworkNameWithExt}".Bash("cp");
                await $" -RL {iosArchiveFrameworkPath}/{dysmNameWithExt} {iosRunnerPath}/{dysmNameWithExt}".Bash("cp");
                Debug.Log("### DONE ARCHIVING");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }


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
        //#endregion
    }
}
