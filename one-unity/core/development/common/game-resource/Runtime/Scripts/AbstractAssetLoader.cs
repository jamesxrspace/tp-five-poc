using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Resource
{
    public abstract class AbstractAssetLoader : MonoBehaviour
    {
        private string bundleID;

        public string BundleID
        {
            get => bundleID;
            set
            {
                if (string.Equals(bundleID, value, System.StringComparison.Ordinal))
                {
                    return;
                }

                var previosBundleID = bundleID;
                bundleID = value;
                OnBundleChanged(previosBundleID, bundleID);
            }
        }

        protected abstract UniTask Load(string bundleID, CancellationToken token);

        protected abstract UniTask Unload(string bundleId, CancellationToken token);

        private void OnBundleChanged(string previosBundleID, string currentBundleID)
        {
            if (!string.IsNullOrEmpty(previosBundleID))
            {
                Unload(previosBundleID, destroyCancellationToken)
                    .SuppressCancellationThrow();
            }

            if (!string.IsNullOrEmpty(currentBundleID))
            {
                Load(currentBundleID, destroyCancellationToken)
                    .SuppressCancellationThrow();
            }
        }
    }
}
