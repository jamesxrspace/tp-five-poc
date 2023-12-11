using System;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Views;

namespace TPFive.Extended.LoxodonFramework
{
    public interface IUIViewFactory
    {
        UniTask<T> GetViewAsync<T>(string name)
            where T : IView;

        UniTask<T> GetViewAsync<T>(string name, IProgress<float> progress)
            where T : IView;

        UniTask<T> GetWindowAsync<T>(string name)
            where T : IWindow;

        UniTask<T> GetWindowAsync<T>(string name, IProgress<float> progress)
            where T : IWindow;

        UniTask<T> GetWindowAsync<T>(IWindowManager manager, string name)
            where T : IWindow;

        UniTask<T> GetWindowAsync<T>(IWindowManager manager, string name, IProgress<float> progress)
            where T : IWindow;
    }
}
