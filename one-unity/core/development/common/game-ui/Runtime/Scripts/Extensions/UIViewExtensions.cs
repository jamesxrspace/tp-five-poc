using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Views;

namespace TPFive.Game.UI
{
    public static class UIViewExtensions
    {
        public static IAsyncResult ShowView(this IUIView view, bool ignoreAnimation = false)
        {
            AsyncResult result = new AsyncResult(true);

            if (!view.Visibility)
            {
                view.Visibility = true;
            }

            if (ignoreAnimation || view.EnterAnimation == null)
            {
                result.SetResult();
                return result;
            }

            view.EnterAnimation
                .OnStart(() => view.Visibility = true)
                .OnEnd(() => result.SetResult())
                .Play();

            return result;
        }

        public static IAsyncResult HideView(this IUIView view, bool ignoreAnimation = false)
        {
            AsyncResult result = new AsyncResult(true);

            if (!view.Visibility)
            {
                result.SetResult();
                return result;
            }

            if (ignoreAnimation || view.ExitAnimation == null)
            {
                view.Visibility = false;
                result.SetResult();
                return result;
            }

            view.ExitAnimation
                .OnEnd(() =>
                {
                    view.Visibility = false;
                    result.SetResult();
                })
                .Play();

            return result;
        }
    }
}
