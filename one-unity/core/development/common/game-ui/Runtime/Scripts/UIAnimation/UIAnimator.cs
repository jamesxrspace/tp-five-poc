using System;
using DG.Tweening;
using UnityEngine;

namespace TPFive.Game.UI
{
    public class UIAnimator
    {
        /// <summary>
        /// Default duration set to an animation.
        /// </summary>
        public const float DefaultDuration = 0.3f;

        /// <summary>
        /// Default start delay set to an animation.
        /// </summary>
        public const float DefaultStartDelay = 0f;

        /// <summary>
        /// Default ease set to an animations.
        /// </summary>
        public const Ease DefaultEase = Ease.Linear;

        /// <summary>
        /// Type of ease an animation, loop or punch should use.
        /// </summary>
        public enum EaseType
        {
            /// <summary>
            /// Easing function.
            /// </summary>
            Ease = 0,

            /// <summary>
            /// Animation Curve.
            /// </summary>
            AnimationCurve = 1,
        }

        /// <summary>
        /// Play animation.
        /// </summary>
        /// <param name="target">UI RectTransform.</param>
        /// <param name="animation">animation settings.</param>
        /// <param name="startPosition">animation start position of target.</param>
        /// <param name="startRotation">animation start rotation of target.</param>
        /// <param name="startScale">animation start scale of target.</param>
        /// <param name="startAlpha">animation start alpha of target.</param>
        /// <param name="instant">instant to destination or not.</param>
        /// <param name="onStart">on animation start callback.</param>
        /// <param name="onComplete">on animation end callback.</param>
        public static void Play(
            RectTransform target,
            Anim animation,
            Vector3 startPosition,
            Vector3 startRotation,
            Vector3 startScale,
            float startAlpha,
            bool instant = false,
            Action onStart = null,
            Action onComplete = null)
        {
            if (target == null)
            {
                Debug.LogError($"[{nameof(UIAnimator)}] {nameof(Play)}(): The target is NULL.");
                return;
            }

            if (animation == null)
            {
                Debug.LogError($"[{nameof(UIAnimator)}] {nameof(Play)}(): The animation is NULL.");
                return;
            }

            if (!animation.Enabled)
            {
                return;
            }

            if (instant)
            {
                onStart?.Invoke();
                PlayInstant(target, animation, startPosition, startRotation, startScale, startAlpha);
                onComplete?.Invoke();
                return;
            }

            // Bind callback
            Sequence animSequence = DOTween.Sequence()
                .SetId(target)
                .OnStart(() => onStart?.Invoke())
                .OnComplete(() => onComplete?.Invoke());

            // Add tweener
            if (TryPrepareMoveTweener(target, animation, ref startPosition, out var moveTweener))
            {
                animSequence.Join(moveTweener);
            }

            if (TryPrepareRotateTweener(target, animation, ref startRotation, out var rotateTweener))
            {
                animSequence.Join(rotateTweener);
            }

            if (TryPrepareScaleTweener(target, animation, ref startScale, out var scaleTweener))
            {
                animSequence.Join(scaleTweener);
            }

            if (TryPrepareFadeTweener(target, animation, ref startAlpha, out var fadeTweener))
            {
                animSequence.Join(fadeTweener);
            }

            // Play
            animSequence.Play();
        }

        /// <summary>
        /// Stop animation.
        /// </summary>
        /// <param name="target">UI RectTransform.</param>
        public static void Stop(RectTransform target)
        {
            if (target != null)
            {
                DOTween.Kill(target);
            }
        }

        /// <summary>
        /// Resets the given target (RectTransform) to the given start parameters (position, rotation, scale and alpha).
        /// </summary>
        /// <param name="target">UI RectTransform.</param>
        /// <param name="animation">animation setting.</param>
        /// <param name="startPosition">start position of target.</param>
        /// <param name="startRotation">animation start rotation of target.</param>
        /// <param name="startScale">animation start scale of target.</param>
        /// <param name="startAlpha">animation start alpha of target.</param>
        public static void Reset(
            RectTransform target,
            Anim animation,
            Vector3 startPosition,
            Vector3 startRotation,
            Vector3 startScale,
            float startAlpha)
        {
            if (target == null)
            {
                Debug.LogError($"[{nameof(UIAnimator)}] {nameof(Reset)}(): The target is NULL.");
                return;
            }

            if (animation == null)
            {
                Debug.LogError($"[{nameof(UIAnimator)}] {nameof(Reset)}(): The animation is NULL.");
                return;
            }

            target.anchoredPosition3D = startPosition;
            target.localRotation = Quaternion.Euler(startRotation);
            target.localScale = startScale;

            if (target.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = startAlpha;
            }
        }

        private static void PlayInstant(
            RectTransform target,
            Anim animation,
            Vector3 startPosition,
            Vector3 startRotation,
            Vector3 startScale,
            float startAlpha)
        {
            if (target == null
                || animation == null
                || !animation.Enabled)
            {
                return;
            }

            switch (animation.Type)
            {
                case Anim.AnimationType.In:
                    UpdateValueIfEnabled(
                        target,
                        animation,
                        () => startPosition,
                        () => Quaternion.Euler(startRotation),
                        () => startScale,
                        canvasGroup =>
                        {
                            canvasGroup.alpha = startAlpha;
                            canvasGroup.blocksRaycasts = true;
                        });
                    break;
                case Anim.AnimationType.Out:
                    UpdateValueIfEnabled(
                        target,
                        animation,
                        () => GetTargetPosition(target, startPosition, animation),
                        () => Quaternion.Euler(animation.Rotate.Rotation),
                        () => animation.Scale.Value,
                        canvasGroup =>
                        {
                            canvasGroup.alpha = 0;
                            canvasGroup.blocksRaycasts = false;
                        });
                    break;
                case Anim.AnimationType.State:
                    UpdateValueIfEnabled(
                        target,
                        animation,
                        () => startPosition + animation.Move.CustomPosition,
                        () => Quaternion.Euler(startRotation + animation.Rotate.Rotation),
                        () => startScale + animation.Scale.Value,
                        canvasGroup => canvasGroup.alpha = animation.Fade.Alpha);
                    break;
            }
        }

        private static void UpdateValueIfEnabled(
            RectTransform target,
            Anim animation,
            Func<Vector3> getPosition,
            Func<Quaternion> getRotation,
            Func<Vector3> getScale,
            Action<CanvasGroup> onFadeEnabled)
        {
            if (animation.Move.Enabled)
            {
                target.anchoredPosition3D = getPosition();
            }

            if (animation.Rotate.Enabled)
            {
                target.localRotation = getRotation();
            }

            if (animation.Scale.Enabled)
            {
                target.localScale = getScale();
            }

            if (animation.Fade.Enabled &&
                target.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                onFadeEnabled(canvasGroup);
            }
        }

        private static bool TryPrepareMoveTweener(RectTransform target, Anim animation, ref Vector3 startPosition, out Tweener tweener)
        {
            tweener = null;

            if (target == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareMoveTweener)}(): The target is NULL.");
                return false;
            }

            if (animation == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareMoveTweener)}(): The animation is NULL.");
                return false;
            }

            if (!animation.Move.Enabled)
            {
                return false;
            }

            // Prepare tweener
            Vector3 targetPosition = Vector3.zero;
            switch (animation.Type)
            {
                case Anim.AnimationType.In:
                    targetPosition = startPosition;
                    target.anchoredPosition3D = GetTargetPosition(target, startPosition, animation);
                    break;
                case Anim.AnimationType.Out:
                    targetPosition = GetTargetPosition(target, startPosition, animation);
                    target.anchoredPosition3D = startPosition;
                    break;
                case Anim.AnimationType.State:
                    targetPosition = startPosition + animation.Move.CustomPosition;
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareMoveTweener)}(): The animation type({animation.Type}) is invalid.");
                    break;
            }

            tweener = target.DOAnchorPos3D(targetPosition, animation.Move.Duration)
                .SetDelay(animation.Move.StartDelay)
                .SetUpdate(true);

            switch (animation.Move.EaseType)
            {
                case EaseType.Ease:
                    tweener.SetEase(animation.Move.Ease);
                    break;
                case EaseType.AnimationCurve:
                    tweener.SetEase(animation.Move.AnimationCurve);
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareMoveTweener)}(): The ease type({animation.Move.EaseType}) is invalid.");
                    break;
            }

            return true;
        }

        private static bool TryPrepareRotateTweener(RectTransform target, Anim animation, ref Vector3 startRotation, out Tweener tweener)
        {
            tweener = null;

            if (target == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareRotateTweener)}(): The target is NULL.");
                return false;
            }

            if (animation == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareRotateTweener)}(): The animation is NULL.");
                return false;
            }

            if (!animation.Rotate.Enabled)
            {
                return false;
            }

            // Prepare tweener
            Vector3 targetRotation = Vector3.zero;
            switch (animation.Type)
            {
                case Anim.AnimationType.In:
                    targetRotation = startRotation;
                    target.localRotation = Quaternion.Euler(animation.Rotate.Rotation);
                    break;
                case Anim.AnimationType.Out:
                    targetRotation = animation.Rotate.Rotation;
                    target.localRotation = Quaternion.Euler(startRotation);
                    break;
                case Anim.AnimationType.State:
                    targetRotation = startRotation + Quaternion.Euler(animation.Rotate.Rotation).eulerAngles;
                    target.localRotation = Quaternion.Euler(startRotation);
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareRotateTweener)}(): The animation type({animation.Type}) is invalid.");
                    break;
            }

            tweener = target.DOLocalRotate(targetRotation, animation.Rotate.Duration, RotateMode.FastBeyond360)
                .SetDelay(animation.Rotate.StartDelay)
                .SetUpdate(true);

            switch (animation.Rotate.EaseType)
            {
                case EaseType.Ease:
                    tweener.SetEase(animation.Rotate.Ease);
                    break;
                case EaseType.AnimationCurve:
                    tweener.SetEase(animation.Rotate.AnimationCurve);
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareRotateTweener)}(): The ease type({animation.Rotate.EaseType}) is invalid.");
                    break;
            }

            return true;
        }

        private static bool TryPrepareScaleTweener(RectTransform target, Anim animation, ref Vector3 startScale, out Tweener tweener)
        {
            tweener = null;

            if (target == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareScaleTweener)}(): The target is NULL.");
                return false;
            }

            if (animation == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareScaleTweener)}(): The animation is NULL.");
                return false;
            }

            if (!animation.Scale.Enabled)
            {
                return false;
            }

            // Prepare tweener
            Vector3 targetScale = Vector3.one;
            switch (animation.Type)
            {
                case Anim.AnimationType.In:
                    targetScale = startScale;
                    target.localScale = animation.Scale.Value;
                    break;
                case Anim.AnimationType.Out:
                    targetScale = animation.Scale.Value;
                    target.localScale = startScale;
                    break;
                case Anim.AnimationType.State:
                    targetScale = startScale + animation.Scale.Value;
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareScaleTweener)}(): The animation type({animation.Type}) is invalid.");
                    break;
            }

            tweener = target.DOScale(targetScale, animation.Scale.Duration)
                .SetDelay(animation.Scale.StartDelay)
                .SetUpdate(true);

            switch (animation.Scale.EaseType)
            {
                case EaseType.Ease:
                    tweener.SetEase(animation.Scale.Ease);
                    break;
                case EaseType.AnimationCurve:
                    tweener.SetEase(animation.Scale.AnimationCurve);
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareScaleTweener)}(): The ease type({animation.Scale.EaseType}) is invalid.");
                    break;
            }

            return true;
        }

        private static bool TryPrepareFadeTweener(RectTransform target, Anim animation, ref float startAlpha, out Tweener tweener)
        {
            tweener = null;

            if (target == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareFadeTweener)}(): The target is NULL.");
                return false;
            }

            if (animation == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(TryPrepareFadeTweener)}(): The animation is NULL.");
                return false;
            }

            if (!animation.Fade.Enabled)
            {
                return false;
            }

            if (!target.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = startAlpha;
            float targetAlpha = 0;

            switch (animation.Type)
            {
                case Anim.AnimationType.In:
                    targetAlpha = startAlpha;
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = true;
                    break;
                case Anim.AnimationType.Out:
                    targetAlpha = 0;
                    canvasGroup.alpha = startAlpha;
                    canvasGroup.blocksRaycasts = false;
                    break;
                case Anim.AnimationType.State:
                    targetAlpha = animation.Fade.Alpha;
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareFadeTweener)}(): The animation type({animation.Type}) is invalid.");
                    break;
            }

            tweener = canvasGroup.DOFade(targetAlpha, animation.Fade.Duration)
                .SetDelay(animation.Fade.StartDelay)
                .SetUpdate(true);

            switch (animation.Fade.EaseType)
            {
                case EaseType.Ease:
                    tweener.SetEase(animation.Fade.Ease);
                    break;
                case EaseType.AnimationCurve:
                    tweener.SetEase(animation.Fade.AnimationCurve);
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareFadeTweener)}(): The ease type({animation.Fade.EaseType}) is invalid.");
                    break;
            }

            return true;
        }

        /// <summary>
        /// Returns Move targetPosition taking into account the MoveDirection.
        /// </summary>
        /// <param name="target">Target RectTransform.</param>
        /// <param name="startPosition">The initial position of the target.</param>
        /// <param name="animation">The animation settings.</param>
        private static Vector3 GetTargetPosition(RectTransform target, Vector3 startPosition, Anim animation)
        {
            var canvas = target.GetComponentInParent<Canvas>();
            if (canvas == null || canvas.rootCanvas == null)
            {
                Debug.LogWarning(
                    $"[{nameof(UIAnimator)}] {nameof(GetTargetPosition)}(): Can't find the root canvas, so return custom position.");
                return animation.Move.CustomPosition;
            }

            var rootCanvas = canvas.rootCanvas;
            RectTransform rootCanvasRectTransform = rootCanvas.GetComponent<RectTransform>();

            if (animation.Move.Direction == Move.MoveDirection.CustomPosition)
            {
                return animation.Move.CustomPosition;
            }

            float offsetX = 0f;
            float offsetY = 0f;

            // X-Axis
            switch (animation.Move.Direction)
            {
                case Move.MoveDirection.Left:
                case Move.MoveDirection.TopLeft:
                case Move.MoveDirection.MiddleLeft:
                case Move.MoveDirection.BottomLeft:
                    offsetX = -((rootCanvasRectTransform.rect.width * target.anchorMin.x)
                                + (target.rect.width * target.localScale.x * (1f - target.pivot.x)));
                    break;
                case Move.MoveDirection.Right:
                case Move.MoveDirection.TopRight:
                case Move.MoveDirection.MiddleRight:
                case Move.MoveDirection.BottomRight:
                    offsetX = (rootCanvasRectTransform.rect.width * (1f - target.anchorMin.x))
                              + (target.rect.width * target.localScale.x * target.pivot.x);
                    break;
                case Move.MoveDirection.TopCenter:
                case Move.MoveDirection.MiddleCenter:
                case Move.MoveDirection.BottomCenter:
                    offsetX = 0f;
                    break;
                case Move.MoveDirection.Top:
                    offsetX = startPosition.x;
                    break;
                case Move.MoveDirection.Bottom:
                    offsetX = startPosition.x;
                    break;
                case Move.MoveDirection.CustomPosition:
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareFadeTweener)}(): The direction({animation.Move.Direction}) is invalid.");
                    break;
            }

            // Y-Axis
            switch (animation.Move.Direction)
            {
                case Move.MoveDirection.Left:
                    offsetY = startPosition.y;
                    break;
                case Move.MoveDirection.Right:
                    offsetY = startPosition.y;
                    break;
                case Move.MoveDirection.Top:
                case Move.MoveDirection.TopLeft:
                case Move.MoveDirection.TopRight:
                case Move.MoveDirection.TopCenter:
                    offsetY = (rootCanvasRectTransform.rect.height * (1f - target.anchorMin.y))
                              + (target.rect.height * target.localScale.y * target.pivot.y);
                    break;
                case Move.MoveDirection.MiddleLeft:
                case Move.MoveDirection.MiddleRight:
                case Move.MoveDirection.MiddleCenter:
                    offsetY = 0f;
                    break;
                case Move.MoveDirection.Bottom:
                case Move.MoveDirection.BottomLeft:
                case Move.MoveDirection.BottomRight:
                case Move.MoveDirection.BottomCenter:
                    offsetY = -((rootCanvasRectTransform.rect.height * target.anchorMin.y)
                                + (target.rect.height * target.localScale.y * (1f - target.pivot.y)));
                    break;
                case Move.MoveDirection.CustomPosition:
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(UIAnimator)}] {nameof(TryPrepareFadeTweener)}(): The direction({animation.Move.Direction}) is invalid.");
                    break;
            }

            return new Vector3(offsetX, offsetY, startPosition.z);
        }
    }
}
