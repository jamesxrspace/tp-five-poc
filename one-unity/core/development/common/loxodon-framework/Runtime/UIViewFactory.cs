using System;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Views;
using VContainer;
using VContainer.Unity;

namespace TPFive.Extended.LoxodonFramework
{
    /// <summary>
    /// Load a view or window and inject dependecies.
    /// </summary>
    public class UIViewFactory : IUIViewFactory
    {
        private readonly IUIViewLocator viewLocator;
        private readonly IObjectResolver container;

        public UIViewFactory(IUIViewLocator viewLocator, IObjectResolver container)
        {
            this.viewLocator = viewLocator;
            this.container = container;
        }

        public async UniTask<T> GetViewAsync<T>(string name)
            where T : IView
        {
            var result = await viewLocator.LoadViewAsync<T>(name).ToUniTask();
            if (result is IView view)
            {
                container.InjectGameObject(view.Transform.gameObject);
            }

            return result;
        }

        public async UniTask<T> GetViewAsync<T>(string name, IProgress<float> progress)
            where T : IView
        {
            var result = await viewLocator.LoadViewAsync<T>(name).ToUniTask(progress);
            if (result is IView view)
            {
                container.InjectGameObject(view.Transform.gameObject);
            }

            return result;
        }

        public async UniTask<T> GetWindowAsync<T>(string name)
            where T : IWindow
        {
            var result = await viewLocator.LoadWindowAsync<T>(name).ToUniTask();
            if (result is IView view)
            {
                container.InjectGameObject(view.Transform.gameObject);
            }

            return result;
        }

        public async UniTask<T> GetWindowAsync<T>(string name, IProgress<float> progress)
            where T : IWindow
        {
            var result = await viewLocator.LoadWindowAsync<T>(name).ToUniTask(progress);
            if (result is IView view)
            {
                container.InjectGameObject(view.Transform.gameObject);
            }

            return result;
        }

        public async UniTask<T> GetWindowAsync<T>(IWindowManager manager, string name)
            where T : IWindow
        {
            var result = await viewLocator.LoadWindowAsync<T>(manager, name).ToUniTask();
            if (result is IView view)
            {
                container.InjectGameObject(view.Transform.gameObject);
            }

            return result;
        }

        public async UniTask<T> GetWindowAsync<T>(IWindowManager manager, string name, IProgress<float> progress)
            where T : IWindow
        {
            var result = await viewLocator.LoadWindowAsync<T>(manager, name).ToUniTask(progress);
            if (result is IView view)
            {
                container.InjectGameObject(view.Transform.gameObject);
            }

            return result;
        }
    }
}
