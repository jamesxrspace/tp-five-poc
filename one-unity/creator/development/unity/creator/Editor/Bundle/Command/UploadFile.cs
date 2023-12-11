using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Creator.Editor;

    /// <summary>
    /// Upload a file to remote location using command line tool. The file refereed here is unitypackage.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class UploadFile
    {
        internal static System.Action Handle(string id, string parentPath)
        {
            return async () =>
            {
                var packagePath = Path.Combine(Application.dataPath, "..", $"{id}.unitypackage");

                var paths = new List<string>
                {
                    parentPath
                };

                Logger.LogDebug("{Method} - Export package to {packagePath}", nameof(Handle), packagePath);
                AssetDatabase.ExportPackage(paths.ToArray(), packagePath, ExportPackageOptions.Recurse);

                var commandLineArguments = $@"upload-file --id ""{id}"" --file-path ""{packagePath}"""
                    .Replace("\n", " ");

                await Utility.HandleUpload(Logger, commandLineArguments);

                Logger.LogDebug("{Method} - Delete package at path {packagePath}", nameof(Handle), packagePath);
                File.Delete(packagePath);

                AssetDatabase.Refresh();
            };
        }

    }
}
