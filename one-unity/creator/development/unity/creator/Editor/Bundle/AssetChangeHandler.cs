using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TPFive.Creator.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    /// <summary>
    /// This will be called whenever any asset is changed under monitored folder.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class AssetChangeHandler : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            Logger.LogDebug(
                "{Method}",
                nameof(OnPostprocessAllAssets));

            var movedAssetPath = "";
            var path = "";
            foreach (var movedAsset in movedAssets)
            {
                var (result, p) = PassCondition(movedAsset);
                if (result)
                {
                    movedAssetPath = movedAsset;
                    path = p;
                    break;
                }
            }

            if (string.IsNullOrEmpty(movedAssetPath))
            {
                return;
            }

            Logger.LogDebug(
                "{Method} - movedAssetPath: {movedAssetPath}",
                nameof(OnPostprocessAllAssets),
                path);
            var aisPath = Path.Combine(path, "AddressableImportSettings.asset").Replace("\\", "/");
            var ais = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(aisPath);

            var addressableAssetGroupTemplate = Utility.GetUnityAssetOfType<AddressableAssetGroupTemplate>();

            var (pathIndex, beginPath) = Utility.GetAdjustedPathWithIndex(aisPath);
            if (pathIndex >= 0)
            {
                beginPath = beginPath.Replace("\\", "/");
                ais.rules = TPFive.Creator.Bundle.Command.Editor.Utility.GetAddressableImportRules(
                    beginPath,
                    pathIndex,
                    addressableAssetGroupTemplate).ToList();
            }

            EditorUtility.SetDirty(ais);
            AssetDatabase.SaveAssets();
        }

        private static (bool, string) PassCondition(string s)
        {
            var pattern = $"{Define.GuidPattern}.asset";
            var regex = new Regex(pattern);

            if (regex.IsMatch(s))
            {
                var loadedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(s);
                if (loadedAsset is BundleDetailData _)
                {
                    var parentFolder = Path.GetDirectoryName(s);
                    return (true, parentFolder);
                }
            }

            return (false, string.Empty);
        }
    }
}
