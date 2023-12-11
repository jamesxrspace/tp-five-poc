using System.IO;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    /// <summary>
    /// Upload a folder to remote location using command line tool. The folder refereed here is addressable folder.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class UploadFolder
    {
        private static readonly string ExeParentPath = Path.Combine(
            Define.CreatorToolPath, "TPFive.Creator.Console");

        internal static System.Action Handle(string id, string parentPath)
        {
            return async () =>
            {
                var version = "latest";
                var platform = EditorUserBuildSettings.activeBuildTarget.ToString();

                var addressablePath = Path.Combine(
                    Application.dataPath, "..", "ServerData", $"{id}", $"{platform}");

                var commandLineArguments = $@"upload-folder --id ""{id}"" --version ""{version}"" --platform ""{platform}"" --folder-path ""{addressablePath}"""
                    .Replace("\n", " ");

                await Utility.HandleUpload(Logger, commandLineArguments);

                AssetDatabase.Refresh();
            };
        }
    }
}
