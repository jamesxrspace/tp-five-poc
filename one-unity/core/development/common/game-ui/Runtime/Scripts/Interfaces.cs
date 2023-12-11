using Cysharp.Threading.Tasks;
using Loxodon.Framework.Views;

namespace TPFive.Game.UI
{
    public interface IService
    {
        UniTask<WindowBase> ShowWindow(System.Type type);

        UniTask<WindowBase> ShowWindow(System.Type type, IBundle bundle);

        UniTask<WindowBase> ShowWindow(string prefabPath);

        UniTask<WindowBase> ShowWindow(string prefabPath, IBundle bundle);

        UniTask<T> ShowWindow<T>()
            where T : WindowBase;

        UniTask<T> ShowWindow<T>(IBundle bundle)
            where T : WindowBase;

        UniTask<T> ShowWindow<T>(string prefabPath)
            where T : WindowBase;

        UniTask<T> ShowWindow<T>(string prefabPath, IBundle bundle)
            where T : WindowBase;

        T FindWindow<T>()
            where T : WindowBase;

        T[] FindWindows<T>()
            where T : WindowBase;

        T FindWindowInRoot<T>(UIRootType type)
            where T : WindowBase;

        T[] FindWindowsInRoot<T>(UIRootType type)
            where T : WindowBase;
    }
}
