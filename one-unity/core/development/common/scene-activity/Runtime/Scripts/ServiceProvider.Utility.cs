using System.Linq;
using Cysharp.Threading.Tasks;
using TPFive.Game.Logging;
using UnityEngine;

namespace TPFive.Extended.SceneActivity
{
    public partial class ServiceProvider
    {
        private async UniTask<string> ReadBundleIdFromScriptObject(string sceneKey)
        {
            var scriptableObject = await _resourceService.LoadAssetAsync<ScriptableObject>(sceneKey);

            if (scriptableObject is TPFive.Creator.BundleDetailData sdd && sdd.bundleKind == TPFive.Creator.BundleKind.Level)
            {
                var assetReferenceScene = sdd.scenes.FirstOrDefault();

                if (assetReferenceScene != null)
                {
                    return assetReferenceScene.RuntimeKey.ToString();
                }
            }

            return string.Empty;
        }

        private void SendTimeUsage(
            string phase,
            object title,
            string loadingOrUnloading,
            float time)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(LoadUnloadSceneAsync)} - {phase} scene {title} {loadingOrUnloading} at {time}");

            Logger.LogEditorDebug(
                "{Method} - {Beginning} scene {Title} {LoadingOrUnloading} at {StartTime}",
                nameof(LoadUnloadSceneAsync),
                phase,
                title,
                loadingOrUnloading,
                time);
        }
    }
}
