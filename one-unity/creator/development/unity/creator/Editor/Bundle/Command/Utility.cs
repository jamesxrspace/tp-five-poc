using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.Creator.Editor;

    public static class Utility
    {
        internal static async Task HandleUpload(
            ILogger logger,
            string commandLineArguments)
        {
            var platform = Utility.GetPlatform();
            if (string.IsNullOrEmpty(platform))
            {
                logger.LogError("Platform not supported.");
                return;
            }

            var combinedExePath = Path.Combine(
                Define.CreatorExeParentPath,
                Utility.GetPlatform(),
                Define.ExeName);
            var absoluteExePath = Path.GetFullPath(combinedExePath);

            logger.LogDebug(
                "{Method} - absoluteExePath: {absoluteExePath}",
                nameof(HandleUpload), absoluteExePath);

            // Start external process
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = absoluteExePath;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            logger.LogDebug("{Method} - startInfo: {startInfo}", nameof(HandleUpload), startInfo);

            // Keep the pipe usage for now even though the use in Unity is not making any difference.
            using var pipeRead = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

            startInfo.Arguments = commandLineArguments;

            var process = System.Diagnostics.Process.Start(startInfo);

            logger.LogDebug("{Method} - process: {process}", nameof(HandleUpload), process);
            if (process is null) return;

            logger.LogDebug("Start another process {fileName} {arguments}",startInfo.FileName, startInfo.Arguments);

            pipeRead.DisposeLocalCopyOfClientHandle();

            using var sr = new StreamReader(pipeRead);

            try
            {
                var consoleTask = Task.Run(() =>
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        logger.LogDebug(line);
                    }
                });

                var pipeTask = Task.Run(() =>
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        logger.LogDebug(line);
                    }
                });

                await Task.WhenAll(consoleTask, pipeTask);
            }
            catch (System.Exception e)
            {
                logger.LogError($"Exception: {e}");
            }
            finally
            {
                process.WaitForExit();
                var exitCode = process.ExitCode;

                process.Close();

                sr.Close();

                logger.LogDebug($"ExitCode: {exitCode}", exitCode);
            }
        }

        /// <summary>
        /// Get platform and return according name of the folder.
        /// </summary>
        /// <returns>
        /// The name matching the folder name. Empty string if not found.
        /// </returns>
        public static string GetPlatform()
        {
            var platform = string.Empty;
#if UNITY_EDITOR_WIN
            platform = "win-x64";
#elif UNITY_EDITOR_OSX
            platform = "osx-x64";
#elif UNITY_EDITOR_LINUX
            platform = "linux-x64";
#endif
            return platform;
        }

        public static AddressableImportSettings GetAddressableImportSettingsBySiblingAsset(BundleDetailData bundleDetailData)
        {
            var sddPath = AssetDatabase.GetAssetPath(bundleDetailData);
            var parentPath = Path.GetDirectoryName(sddPath);
            var aisPath = Path.Combine(parentPath, "AddressableImportSettings.asset");
            var ais = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(aisPath);
            
            return ais;
        }

        public static (string, string) CreateFolder(string parentFolderPath, string folderName)
        {
            var folderGuid = AssetDatabase.CreateFolder(parentFolderPath, folderName);
            if (string.IsNullOrEmpty(folderGuid))
            {
                return (string.Empty, string.Empty);
            }

            var folderPath = AssetDatabase.GUIDToAssetPath(folderGuid);

            return (folderGuid, folderPath);
        }
        
        public static IEnumerable<AddressableImportRule> GetAddressableImportRules(
            string aisParentPath,
            int index,
            AddressableAssetGroupTemplate addressableAssetGroupTemplate)
        {
            var groupName = $@"${{PATH[{index}]}}";
            
            var rules = new List<AddressableImportRule>
            {
                CreateAddressableImportRule(
                    $"{aisParentPath}/{Define.AddressableRuleCategoryPattern}",
                    addressableAssetGroupTemplate,
                    groupName,
                    "${category}/${asset}"),
                CreateAddressableImportRule(
                    $"{aisParentPath}/{Define.AddressableRuleGuidPattern}",
                    addressableAssetGroupTemplate,
                    groupName,
                    "${asset}"),
            };

            return rules;
        }

        private static AddressableImportRule CreateAddressableImportRule(
            string path,
            AddressableAssetGroupTemplate addressableAssetGroupTemplate,
            string groupName,
            string addressReplacement)
        {
            return new AddressableImportRule
            {
                // The path has to be using "/" no matter what OS is used.
                path = path,
                matchType = AddressableImportRuleMatchType.Regex,
                groupTemplate = addressableAssetGroupTemplate,
                groupName = groupName,
                addressReplacement = addressReplacement
            };
        }
    }
}
