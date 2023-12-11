namespace TPFive.Game.Account
{
    public interface IService
    {
        public bool IsSignedIn();

        public string GetAccessToken();

        public void TryGetValidToken(IAuthCallback<string> listener);

        public void SignInUserByWebview(IAuthCallback<string> listener);

        public void SignInUserByUsername(string username, string password, IAuthCallback<string> listener);

        public void RenewToken(IAuthCallback<string> listener);

        public void SignOutUser(IAuthCallback<string> listener);

        public void SignInDevice(System.Action<DeviceCodeFormat> action, IAuthCallback<string> listener);

        public bool IsGuestExist();

        public bool IsGuestLoggedIn();

        public void CreateGuestAccount(string nickname, IAuthCallback<string> listener);

        public void SignInGuest(IAuthCallback<string> listener);

        public void SetAccessToken(string token);
    }
}