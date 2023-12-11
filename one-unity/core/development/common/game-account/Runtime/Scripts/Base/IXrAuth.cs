namespace TPFive.Game.Account
{
    using Cysharp.Threading.Tasks;

    public interface IXrAuth
    {
        void SignInUser(IAuthCallback<Credential> listener);

        void SignInUser(string username, string password, IAuthCallback<Credential> listener);

        void SignOutUser(IAuthCallback<string> listener);

        void RenewAccessToken(string refreshToken, IAuthCallback<Credential> listener);

        void ApiDeviceCodeSignIn(IAuthCallback<DeviceCodeFormat> verifyUriListener, IAuthCallback<Credential> authStateListener);

        void ApiStartDeviceAuth(IAuthCallback<DeviceCodeFormat> listener);

        void GetCurrentDeviceAuthState(string deviceCode, IAuthCallback<Credential> listener);

        void CreateGuestAccount(string nickname, IAuthCallback<Guest> listener);

        UniTask<AuthResult<string>> ReadDataAsync(string key);

        UniTask<AuthResult<bool>> SaveDataAsync(string key, string value);

        UniTask<AuthResult<bool>> DeleteDataAsync(string key);
    }
}