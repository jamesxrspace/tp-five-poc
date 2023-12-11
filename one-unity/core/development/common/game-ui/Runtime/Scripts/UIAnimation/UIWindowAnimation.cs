using Loxodon.Framework.Views;
using Loxodon.Framework.Views.Animations;
using UnityEngine;

namespace TPFive.Game.UI
{
    public class UIWindowAnimation : UIAnimation
    {
        [SerializeField]
        private UITweener[] tweeners;

        private IUIView view;

        private int totalTween;
        private bool isTweening;
        private int tweenEndCount;

        public UITweener[] UITweenerArrary => tweeners;

        public override IAnimation Play()
        {
            foreach (UITweener tweener in tweeners)
            {
                tweener.Play(this.OnTweenStart, this.OnTweenEnd);
            }

            return this;
        }

        protected void Awake()
        {
            if (tweeners == null)
            {
                return;
            }

            totalTween = tweeners.Length;
        }

        protected void OnEnable()
        {
            view = GetComponent<IUIView>();

            switch (AnimationType)
            {
                case AnimationType.EnterAnimation:
                    view.EnterAnimation = this;
                    break;
                case AnimationType.ExitAnimation:
                    view.ExitAnimation = this;
                    break;
                case AnimationType.ActivationAnimation:
                    {
                        if (view is IWindowView windowView)
                        {
                            windowView.ActivationAnimation = this;
                        }
                    }

                    break;
                case AnimationType.PassivationAnimation:
                    {
                        if (view is IWindowView windowView)
                        {
                            windowView.PassivationAnimation = this;
                        }
                    }

                    break;
            }
        }

        private void OnTweenStart()
        {
            if (isTweening)
            {
                return;
            }

            isTweening = true;
            tweenEndCount = 0;
            OnStart();
        }

        private void OnTweenEnd()
        {
            ++tweenEndCount;

            // All tween finished
            if (tweenEndCount == totalTween)
            {
                isTweening = false;
                OnEnd();
            }
        }
    }
}
