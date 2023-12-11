using System.IO;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    /// <summary>
    /// Build addressable.
    /// </summary>
    /// <remarks>
    /// Currently inside Unity 2022.3 the use of build report provided by Unity may create issue preventing
    /// the build process going.
    /// </remarks>
    [EditorUseLogging]
    public sealed partial class BuildAddressable
    {
        private static readonly string BuildScriptPath =
            Path.Combine("Assets", "AddressableAssetsData", "DataBuilders", "BuildScriptPackedMode.asset");

        public static void Handle()
        {
            // Get the current settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var dataBuilder= AssetDatabase.LoadAssetAtPath<ScriptableObject>(BuildScriptPath) as IDataBuilder;
            var index = settings.DataBuilders.IndexOf((ScriptableObject)dataBuilder);
            if (index > 0)
            {
                settings.ActivePlayerDataBuilderIndex = index;
            }

            AddressableAssetSettings.CleanPlayerContent(
                AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);

            AddressableAssetSettings
                .BuildPlayerContent(out var result);

            Logger.LogDebug(
                "{Method} - result: {result}",
                nameof(Handle),
                result);
        }
    }
}
