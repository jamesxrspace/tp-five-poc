using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEditor;

namespace TPFive.Extended.Addressable.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Extended.Addressable.Editor;

    /// <summary>
    /// Upload a file to remote location using command line tool. The file refereed here is unitypackage.
    /// </summary>
    [EditorUseLogging]
    public sealed partial class UpdateContentOverview
    {
        internal static async Task Handle(ContentOverviewData contentOverviewData)
        {
            var downloadPath = Path.GetFullPath(Define.FetcherDownloadPath);

            var commandLineArguments = $@"download-overview --folder-path ""{downloadPath}"""
                .Replace("\n", " ");

            await Utility.HandleDownload(
                Logger,
                commandLineArguments,
                (progress) =>
                {
                    Logger.LogDebug("Empty progress handler");
                });

            var jsonPath = Path.Combine(downloadPath, "content-overview.json");
            var json = await File.ReadAllTextAsync(jsonPath);
            var jsonContentOverviewData = TPFive.Fetcher.Generated.ContentOverviewData.FromJson(json);

            contentOverviewData.UnitypackageList.Clear();
            foreach (var up in jsonContentOverviewData.Unitypackages)
            {
                contentOverviewData.UnitypackageList.Add(new FileContent
                {
                    Id = up.Id,
                    ToBeImported = false,
                });
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
