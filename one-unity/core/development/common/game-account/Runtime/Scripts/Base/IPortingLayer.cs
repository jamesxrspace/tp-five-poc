namespace TPFive.Game.Account
{
    public abstract class IPortingLayer
    {
        public abstract void Login(string clientId, string domain, string redirectUri, IAuthCallback<string> callback);

        public abstract void Logout(string clientId, string domain, string redirectUri, IAuthCallback<string> callback);

        public abstract void SaveInfo(string key, string value, IAuthCallback<bool> callback);

        public abstract void ReadInfo(string key, IAuthCallback<string> callback);

        public abstract void DeleteInfo(string key, IAuthCallback<bool> callback);
    }
}
