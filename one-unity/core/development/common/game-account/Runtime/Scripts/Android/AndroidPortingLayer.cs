namespace TPFive.Game.Account
{
    using Microsoft.Extensions.Logging;
    using UnityEngine;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    internal class AndroidPortingLayer : IPortingLayer
    {
        private readonly AndroidJavaObject accountObject;
        private readonly ILogger logger;

        public AndroidPortingLayer(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<EditorPortingLayer>();
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var androidContext = activity.Call<AndroidJavaObject>("getApplicationContext");
            accountObject = new AndroidJavaObject("com.xrspace.xrauth.XrAuth", androidContext);
        }

        public override void Login(string clientId, string domain, string redirectUri, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(Login));
            accountObject.Call("login", clientId, domain, redirectUri, new AndroidCallback<string>(callback));
        }

        public override void Logout(string clientId, string domain, string redirectUri, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(Logout));
            accountObject.Call("logout", new AndroidCallback<string>(callback));
        }

        public override void ReadInfo(string key, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(ReadInfo));
            accountObject.Call("readInfo", key, new AndroidCallback<string>(callback));
        }

        public override void SaveInfo(string key, string value, IAuthCallback<bool> callback)
        {
            logger.LogInformation(nameof(SaveInfo));
            accountObject.Call("saveInfo", key, value, new AndroidCallback<bool>(callback));
        }

        public override void DeleteInfo(string key, IAuthCallback<bool> callback)
        {
            logger.LogInformation(nameof(DeleteInfo));
            accountObject.Call("deleteInfo", key, new AndroidCallback<bool>(callback));
        }
    }
}