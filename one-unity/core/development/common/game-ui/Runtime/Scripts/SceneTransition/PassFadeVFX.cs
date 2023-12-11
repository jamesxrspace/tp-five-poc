using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace TPFive.Game.UI
{
    /// <summary>
    /// This is a Black Sphere to block the screen in VR.
    /// </summary>
    public class PassFadeVFX : MonoBehaviour, ISceneTransition
    {
        private const string MainCameraTag = "MainCamera";

        [SerializeField]
        private DOTweenAnimation fadeInAnimation;

        [SerializeField]
        private DOTweenAnimation fadeOutAnimation;

        [SerializeField]
        private MeshRenderer fadeMeshRenderer;

        private GameObject _mainCamera;

        public void Prepare()
        {
            // Recenter the PassFadeVFX to the camera position.
            transform.position = GetCameraPosition();
        }

        [ContextMenu(nameof(FadeIn))]
        public void FadeIn()
        {
            EnableMeshRenderer();

            if (fadeInAnimation != null)
            {
                fadeInAnimation.RecreateTweenAndPlay();
            }
        }

        [ContextMenu(nameof(FadeOut))]
        public void FadeOut()
        {
            EnableMeshRenderer();

            if (fadeOutAnimation != null)
            {
                fadeOutAnimation.RecreateTweenAndPlay();
            }
        }

        protected void Start()
        {
            Assert.IsNotNull(fadeInAnimation, "Missing FadeInAnimation!");
            Assert.IsNotNull(fadeOutAnimation, "Missing FadeOutAnimation!");

            // This is to allow the PassFadeVFX to be hidden when not in used, to avoid wasting draw calls.
            fadeOutAnimation.onComplete.AddListener(DisableMeshRenderer);
        }

        protected void OnDestroy()
        {
            if (fadeOutAnimation != null)
            {
                fadeOutAnimation.onComplete.RemoveAllListeners();
            }
        }

        private void EnableMeshRenderer()
        {
            if (fadeMeshRenderer != null)
            {
                fadeMeshRenderer.enabled = true;
            }
        }

        private void DisableMeshRenderer()
        {
            if (fadeMeshRenderer != null)
            {
                fadeMeshRenderer.enabled = false;
            }
        }

        private Vector3 GetCameraPosition()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindWithTag(MainCameraTag);
            }

            return _mainCamera != null ? _mainCamera.transform.position : Vector3.zero;
        }
    }
}