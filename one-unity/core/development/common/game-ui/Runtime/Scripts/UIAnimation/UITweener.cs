using System;
using UnityEngine;
using UnityEngine.Events;

namespace TPFive.Game.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup))]
    public class UITweener : MonoBehaviour
    {
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private bool isPlaying;
        private Action onStartAction;
        private Action onEndAction;

        [SerializeField]
        private Anim anim;

        [Space(5)]
        [SerializeField]
        private UnityEvent onTweenStart;

        [SerializeField]
        private UnityEvent onTweenEnd;

        public bool IsPlaying => isPlaying;

        /// <summary>
        /// Internal variable that holds the start RectTransform.anchoredPosition3D.
        /// </summary>
        private Vector3 StartPosition { get; set; }

        /// <summary>
        /// Internal variable that holds the start RectTransform.localEulerAngles
        /// </summary>
        private Vector3 StartRotation { get; set; }

        /// <summary>
        /// Internal variable that holds the start RectTransform.localScale
        /// </summary>
        private Vector3 StartScale { get; set; }

        /// <summary>
        /// Internal variable that holds the start alpha. It does that by checking if a CanvasGroup component is attached (holding the alpha value) or it just rememebers 1 (as in 100% visibility)
        /// </summary>
        private float StartAlpha { get; set; }

        public void Play()
        {
            Play(null, null);
        }

        public void Play(Action onStart, Action onEnd, bool forceStop = false)
        {
            ResetRectTransform();
            onStartAction = onStart;
            onEndAction = onEnd;
            if (forceStop)
            {
                Stop();
            }

            UIAnimator.Play(
                rectTransform,
                anim,
                StartPosition,
                StartRotation,
                StartScale,
                StartAlpha,
                onStart: OnPlayStart,
                onComplete: OnPlayComplete);
        }

        public void Stop()
        {
            UIAnimator.Stop(rectTransform);
        }

        /// <summary>
        /// Resets the UI's RectTransfrom to the start values.
        /// </summary>
        public void ResetRectTransform()
        {
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = StartPosition;
                rectTransform.localRotation = Quaternion.Euler(StartRotation);
                rectTransform.localScale = StartScale;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = StartAlpha;
            }
        }

        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            StartPosition = rectTransform.anchoredPosition3D;
            StartRotation = rectTransform.localEulerAngles;
            StartScale = rectTransform.localScale;
            StartAlpha = canvasGroup.alpha;
            isPlaying = false;
        }

        protected void OnDestroy()
        {
            UIAnimator.Stop(rectTransform);
        }

        private void OnPlayStart()
        {
            isPlaying = true;
            onTweenStart?.Invoke();
            onStartAction?.Invoke();
        }

        private void OnPlayComplete()
        {
            isPlaying = false;
            onTweenEnd?.Invoke();
            onEndAction?.Invoke();
        }
    }
}
