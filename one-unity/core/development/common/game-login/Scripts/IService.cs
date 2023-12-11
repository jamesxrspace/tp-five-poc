using Cysharp.Threading.Tasks;

namespace TPFive.Game.Login
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask<LoginResult> LoginByWebView();

        UniTask<LoginResult> LoginByPassword(string username, string password);

        UniTask<LoginResult> LoginByDeviceCode(System.IProgress<DeviceCodeInfo> progress);

        UniTask<LogoutResult> Logout();

        string GetAccessToken();

        UniTask<bool> RefreshAccessToken();

        void SetAccessToken(string token);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        UniTask<LoginResult> LoginByWebView();

        UniTask<LoginResult> LoginByPassword(string username, string password);

        UniTask<LoginResult> LoginByDeviceCode(System.IProgress<DeviceCodeInfo> progress);

        UniTask<LogoutResult> Logout();

        string GetAccessToken();

        UniTask<bool> RefreshAccessToken();
    }
}
