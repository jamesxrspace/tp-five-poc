using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEditor;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Addressable.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    /// <summary>
    /// Upload a file to remote location using command line tool. The file refereed here is unitypackage.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class DownloadFiles
    {
        internal static async Task Handle(
            IReadOnlyList<string> ids,
            string parentPath,
            System.Action<float> progressCallback = null)
        {
            Logger.LogDebug("{Method} - ids: {ids}", nameof(Handle), ids);

            var idArgs = ids.Aggregate(string.Empty, (acc, next) => $@"{acc} --id ""{next}"" --save-to-path ""{parentPath}""");
            var commandLineArguments = $@"download-file {idArgs}"
                    .Replace("\n", " ");
            Logger.LogDebug(commandLineArguments);

            await Utility.HandleDownload(Logger, commandLineArguments, progressCallback);

            foreach (var id in ids)
            {
                var packagePath = Path.Combine(parentPath, $"{id}.unitypackage");

                AssetDatabase.ImportPackage(packagePath, false);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
