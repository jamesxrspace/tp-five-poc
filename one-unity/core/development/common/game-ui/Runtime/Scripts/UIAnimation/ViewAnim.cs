using System;
using Loxodon.Framework.Views.Animations;
using TPFive.Game.Extensions;
using UnityEngine;

namespace TPFive.Game.UI
{
    /// <summary>
    /// Integrate <see cref="TPFive.Game.UI.Anim"/> with <see cref="Loxodon.Framework.Views.Animations.IAnimation"/>.
    /// </summary>
    [Serializable]
    public class ViewAnim : Anim, IAnimation
    {
#pragma warning disable SA1401
        [NonSerialized]
        public RectTransform Target;

        [NonSerialized]
        public Vector3 StartPosition;

        [NonSerialized]
        public Vector3 StartRotation;

        [NonSerialized]
        public Vector3 StartScale;

        [NonSerialized]
        public float StartAlpha;
#pragma warning restore SA1401

        private event Action OnStartCallback;

        private event Action OnEndCallback;

        public static void CacheRectTransform(RectTransform rectTransform, params ViewAnim[] viewAnims)
        {
            Vector3 startPosition = rectTransform.anchoredPosition3D;
            Vector3 startRotation = rectTransform.localEulerAngles;
            Vector3 startScale = rectTransform.localScale;
            float startAlpha = GetOrAddCanvasGroup(rectTransform).alpha;

            foreach (var viewAnim in viewAnims)
            {
                viewAnim.Target = rectTransform;
                viewAnim.StartPosition = startPosition;
                viewAnim.StartRotation = startRotation;
                viewAnim.StartScale = startScale;
                viewAnim.StartAlpha = startAlpha;
            }
        }

        public static void RestoreRectTransform(RectTransform rectTransform, ViewAnim viewAnim)
        {
            rectTransform.anchoredPosition3D = viewAnim.StartPosition;
            rectTransform.localEulerAngles = viewAnim.StartRotation;
            rectTransform.localScale = viewAnim.StartScale;
            GetOrAddCanvasGroup(rectTransform).alpha = viewAnim.StartAlpha;
        }

        public IAnimation OnStart(Action onStart)
        {
            OnStartCallback += onStart;

            return this;
        }

        public IAnimation OnEnd(Action onEnd)
        {
            OnEndCallback += onEnd;

            return this;
        }

        public IAnimation Play()
        {
            return Play(false);
        }

        public IAnimation Play(bool instant)
        {
            if (Enabled)
            {
                UIAnimator.Stop(Target);
                UIAnimator.Play(
                    Target,
                    this,
                    StartPosition,
                    StartRotation,
                    StartScale,
                    StartAlpha,
                    instant,
                    OnStart,
                    OnEnd);
            }
            else
            {
                OnStart();
                OnEnd();
            }

            return this;
        }

        private static CanvasGroup GetOrAddCanvasGroup(RectTransform rectTransform)
        {
            return rectTransform.gameObject.GetOrAddComponent<CanvasGroup>();
        }

        private void OnStart()
        {
            try
            {
                OnStartCallback?.Invoke();
                OnStartCallback = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(ViewAnim)}] OnStart(): got exception: {e}");
            }
        }

        private void OnEnd()
        {
            try
            {
                OnEndCallback?.Invoke();
                OnEndCallback = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(ViewAnim)}] OnEnd(): got exception: {e}");
            }
        }
    }
}
