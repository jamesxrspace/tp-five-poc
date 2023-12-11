using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.UI
{
    /// <summary>
    /// This class is used to assign the animation(<see cref="TPFive.Game.UI.ViewAnim"/>) to the <see cref="Loxodon.Framework.Views.IUIView"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIViewAnimation : MonoBehaviour
    {
        [SerializeField]
        private ViewAnim enterAnimation;

        [SerializeField]
        private ViewAnim exitAnimation;

        private IUIView view;
        private RectTransform rectTransform;

        protected IUIView View
        {
            get
            {
                if (view == null)
                {
                    view = GetComponent<IUIView>();
                }

                return view;
            }
        }

        protected RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null && !TryGetComponent(out rectTransform))
                {
                    rectTransform = gameObject.AddComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        public void CacheRectTransform()
        {
            ViewAnim.CacheRectTransform(RectTransform, enterAnimation, exitAnimation);
        }

        public void RestoreRectTransform()
        {
            ViewAnim.RestoreRectTransform(RectTransform, enterAnimation);
        }

        protected virtual void Awake()
        {
            CacheRectTransform();

            AssignAnimationToView();
        }

        protected virtual void AssignAnimationToView()
        {
            if (View == null)
            {
                return;
            }

            View.EnterAnimation = enterAnimation;
            View.ExitAnimation = exitAnimation;
        }
    }
}
