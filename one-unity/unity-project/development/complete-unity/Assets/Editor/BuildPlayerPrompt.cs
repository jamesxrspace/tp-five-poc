using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

namespace TPFive.Editor
{
    class BuildPlayerPrompt
    {
        private static bool IsBatchMode => ContainsArgument("-batchmode");
        private const int BuildPlayerContent = 0;
        private const int CancellBuild = 1;

#pragma warning disable IDE0051 // Remove unused private members
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (IsBatchMode)
            {
                return;
            }

            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
        }
#pragma warning restore IDE0051 // Remove unused private members

        private static void BuildPlayerHandler(BuildPlayerOptions options)
        {
            if (IsBatchMode)
            {
                return;
            }

            EnsureEssentialFiles();
            PromptBuildContentsOrBuildPlayer(options);
        }

        private static void EnsureEssentialFiles()
        {
            try
            {
                var setting = BuildPlayerPromptSettingManager.FindSetting(EditorUserBuildSettings.activeBuildTarget);
                if (setting != null &&
                    setting.EssentialFiles != null &&
                    setting.EssentialFiles.Count > 0)
                {
                    var projectDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../")).Replace('\\', '/');
                    setting.EssentialFiles.ForEach(x =>
                    {
                        var files = new Matcher().AddInclude(x).GetResultsInFullPath(projectDir);
                        if (!files.Any())
                        {
                            throw new BuildFailedException($"Missing {x}");
                        }
                    });
                }
            }
            catch (BuildFailedException e)
            {
                EditorUtility.DisplayDialog("Build Failed", e.Message, "OK");
                throw;
            }
        }

        private static void PromptBuildContentsOrBuildPlayer(BuildPlayerOptions options)
        {
            var answer = EditorUtility.DisplayDialogComplex(
                nameof(BuildPlayerPrompt),
                "Do you want to build addressable content before starting to build?",
                "Yes",
                "Cancel build",
                "No");

            if (answer == BuildPlayerContent)
            {
                var dataBuilder = AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder;
                AddressableAssetSettings.CleanPlayerContent(dataBuilder);
                BuildCache.PurgeCache(true);
                BuildScript.buildCompleted += result =>
                {
                    BuildScript.buildCompleted = null;
                    if (string.IsNullOrEmpty(result.Error))
                    {
                        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
                    }
                };

                AddressableAssetSettings.BuildPlayerContent();
            }
            else if (answer == CancellBuild)
            {
                throw new BuildFailedException("Cancelled by the user");
            }
            else
            {
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
            }
        }

        private static bool ContainsArgument(string name)
        {
            var args = new HashSet<string>(Environment.GetCommandLineArgs(), StringComparer.OrdinalIgnoreCase);
            return args.Contains(name);
        }
    }
}