using DG.Tweening;
using UnityEngine;

namespace TPFive.Room
{
    public sealed class PlayerHudView : MonoBehaviour
    {
        [SerializeField]
        private Camera relativeCamera;
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private CameraFacingBillboard billboard;
        [SerializeField]
        private TMPro.TextMeshProUGUI label;
        [SerializeField]
        private float visiblityThreshold = 10f;

        [SerializeField]
        private DOTweenAnimation fadeInAnimation;

        [SerializeField]
        private DOTweenAnimation fadeOutAnimation;

        private float squaredVisiblityThreshold;
        private bool visiblity = true;

        public void UpdateLable(string text)
        {
            if (label != null)
            {
                label.text = text;
            }
        }

        public void FadeIn()
        {
            if (fadeInAnimation == null)
            {
                return;
            }

            fadeInAnimation.RecreateTweenAndPlay();
        }

        public void FadeOut()
        {
            if (fadeOutAnimation == null)
            {
                OnFadeOutComplete();
                return;
            }

            fadeOutAnimation.RecreateTweenAndPlay();
        }

        private void Awake()
        {
            if (relativeCamera == null)
            {
                relativeCamera = Camera.main;
            }

            if (relativeCamera != null && canvas != null && canvas.worldCamera == null)
            {
                canvas.worldCamera = relativeCamera;
            }

            if (billboard != null && relativeCamera != null)
            {
                billboard.SetRelativeCamera(relativeCamera);
                billboard.LimitRotateOnY(Game.GameApp.IsVRHeadset);
            }

            if (fadeOutAnimation != null)
            {
                fadeOutAnimation.onComplete.AddListener(OnFadeOutComplete);
            }

            squaredVisiblityThreshold = visiblityThreshold * visiblityThreshold;
        }

        private void LateUpdate()
        {
            if (relativeCamera == null)
            {
                return;
            }

            UpdateVisiblity(this.transform, relativeCamera.transform);
        }

        private void OnFadeOutComplete()
        {
            if (billboard != null)
            {
                billboard.enabled = false;
            }
        }

        private void UpdateVisiblity(Transform uiTransform, Transform cameraTranform)
        {
            Vector3 offset = uiTransform.position - cameraTranform.position;
            float squaredDistance = offset.sqrMagnitude;

            if (squaredDistance > squaredVisiblityThreshold)
            {
                if (visiblity)
                {
                    visiblity = false;
                    FadeOut();
                }
            }
            else
            {
                if (!visiblity)
                {
                    visiblity = true;
                    FadeIn();
                }

                if (billboard != null)
                {
                    billboard.enabled = true;
                }
            }
        }
    }
}
