using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.Bundle.Command.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    [EditorUseLogging]
    public sealed partial class UploadToUgcService
    {
        internal static System.Action Handle(BundleDetailData bundleDetailData)
        {
            return async () =>
            {
                var bundleKind = System.Enum.GetName(typeof(BundleKind), bundleDetailData.bundleKind);
                var generatedBdd = new Bundle.Editor.Generated.BundlDetaileData
                {
                    Id = bundleDetailData.id,
                    Title = bundleDetailData.title,
                    Description = bundleDetailData.description,
                    TitleTermId = bundleDetailData.titleTermId,
                    DescriptionTermId = bundleDetailData.descriptionTermId,
                    BundleKind = bundleKind,
                    StorageKind = System.Enum.GetName(typeof(StorageKind), bundleDetailData.storageKind),
                    OutputFormat = System.Enum.GetName(typeof(OutcomeFormat), bundleDetailData.outcomeFormat),
                    UgcId = bundleDetailData.ugcId
                };

                var jsonString = Bundle.Editor.Generated.Serialize.ToJson(generatedBdd);

                var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                var base64JsonString = System.Convert.ToBase64String(jsonBytes);

                var thumbnailPath = AssetDatabase.GetAssetPath(bundleDetailData.thumbnail);
                thumbnailPath = Path.GetFullPath(thumbnailPath);

                var bundleDetailDataPath = AssetDatabase.GetAssetPath(bundleDetailData);
                var bundleDetailDataFullPath = Path.GetFullPath(bundleDetailDataPath);

                Logger.LogDebug("{Method} - thumbnailPath: {thumbnailPath}", nameof(Handle), thumbnailPath);

                var projectId = CloudProjectSettings.projectId;
                // Thinking to use environment settings from service but not yet found the way to get it.
                // Fixed to development for now.
                var environmentName = "development";

                var commandLineArguments = $@"
upload-content
    --project-id ""{projectId}""
    --env-name ""{environmentName}""
    --asset-path ""{bundleDetailDataFullPath}""
    --json ""{base64JsonString}""
    --thumbnail ""{thumbnailPath}""
    --tag ""{bundleKind}""
";

                var pattern = @"[\r\n]";
                commandLineArguments = Regex.Replace(commandLineArguments, pattern, " ");

                await UploadUgcUtility.HandleUpload(Logger, commandLineArguments);
            };
        }
    }
}
