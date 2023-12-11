using System.Collections;
using Cysharp.Threading.Tasks;
using TPFive.Game.Logging;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TPFive.Extended.Addressable
{
    public sealed partial class ServiceProvider
    {
        private static string GetCatalogPath(string bundleId)
        {
            var platform = Game.Utility.GetBuildTargetByRuntimePlatform(Game.GameApp.RuntimePlatform);

#if UNITY_EDITOR_WIN
            platform = "StandaloneWindows64";
#elif UNITY_EDITOR_OSX
            platform = "StandaloneOSX";
#endif

            return $"«PrefixPath»/{bundleId}/«IntermediatePath»/{platform}/catalog_latest.json";
        }

        // TODO: Remove to general utility later.
        private static float ConvertToMB(long bytes)
        {
            return bytes / 1024f / 1024f;
        }

        private async UniTask ShowDownloadStatus<T>(object key, AsyncOperationHandle<T> handle)
        {
            while (!handle.IsDone)
            {
                var downloadStatus = handle.GetDownloadStatus();
                Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                    _pubPostUnityMessage,
                    $"{nameof(LoadAssetAsync)} - Asset {key} is downloaded status {handle.Status}: DownloadBytes: {ConvertToMB(downloadStatus.DownloadedBytes)}MB TotalBytes: {ConvertToMB(downloadStatus.TotalBytes)}MB");

                Logger.LogEditorDebug(
                    "{Method} - Asset {Key} is downloaded status {Status}: DownloadBytes: {DownloadBytes}MB TotalBytes: {TotalBytes}MB",
                    nameof(LoadAssetAsync),
                    key,
                    handle.Status,
                    ConvertToMB(downloadStatus.DownloadedBytes),
                    ConvertToMB(downloadStatus.TotalBytes));

                await UniTask.Yield();
            }
        }
    }
}
