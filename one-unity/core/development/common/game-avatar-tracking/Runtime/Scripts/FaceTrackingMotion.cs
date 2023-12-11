using Animancer;
using TPFive.Game.Mocap;
using TPFive.SCG.DisposePattern.Abstractions;
using UniRx;
using UnityEngine;

namespace TPFive.Game.Avatar.Tracking
{
    [Dispose]
    public partial class FaceTrackingMotion
    {
        private readonly AnimancerLayer layer;
        private readonly SkinnedMeshRenderer renderer;
        private readonly IFaceBlendShapeProvider provider;
        private readonly FaceTrackingState faceTrackingState;
        private readonly ARKitBlendShapeLocation[] locations;
        private readonly ARKitBlendShapeAccessor blendShapeAccessor;

        private float fadeDuration;
        private bool isEnable;
        private System.IDisposable updateSubscription;

        public FaceTrackingMotion(
            AnimancerLayer layer,
            SkinnedMeshRenderer renderer,
            IFaceBlendShapeProvider provider,
            float fadeDuration = 0.25f)
        {
            this.layer = layer ?? throw new System.ArgumentNullException(nameof(layer));

            if (renderer.Equals(null))
            {
                throw new System.ArgumentNullException(nameof(SkinnedMeshRenderer));
            }

            this.renderer = renderer;
            this.provider = provider ?? throw new System.ArgumentNullException(nameof(provider));
            locations = (ARKitBlendShapeLocation[])System.Enum.GetValues(typeof(ARKitBlendShapeLocation));
            blendShapeAccessor = new ARKitBlendShapeAccessor(renderer);
            this.fadeDuration = fadeDuration;
            faceTrackingState = new FaceTrackingState();
            this.layer.GetOrCreateState(faceTrackingState);
        }

        public bool IsEnable
        {
            get => isEnable;
            set
            {
                if (value == isEnable)
                {
                    return;
                }

                isEnable = value;
                OnStatusChanged();
            }
        }

        private void OnStatusChanged()
        {
            if (isEnable)
            {
                layer.Play(faceTrackingState, fadeDuration);
                updateSubscription?.Dispose();
                updateSubscription = Observable.EveryLateUpdate()
                    .Subscribe(Update);

                return;
            }

            layer.StartFade(0, fadeDuration);
            Observable.Timer(System.TimeSpan.FromSeconds(fadeDuration))
                .Subscribe(CancelUpdateIfNeed);
        }

        private void Update(long count)
        {
            if (renderer == null)
            {
                Debug.LogWarning($"The {nameof(SkinnedMeshRenderer)} is null in {nameof(FaceTrackingMotion)}");
                return;
            }

            var weight = layer.Weight;

            foreach (var location in locations)
            {
                var originalValue = blendShapeAccessor.GetBlendShapeWeight(location);
                var sourceValue = provider.GetBlendShapeValue(location);
                sourceValue = Mathf.Lerp(originalValue, sourceValue, weight);
                blendShapeAccessor.SetBlendShapeWeight(location, sourceValue);
            }
        }

        private void CancelUpdateIfNeed(long count)
        {
            if (isEnable || updateSubscription == null)
            {
                return;
            }

            updateSubscription?.Dispose();
            updateSubscription = null;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                updateSubscription?.Dispose();
            }

            _disposed = true;
        }
    }
}