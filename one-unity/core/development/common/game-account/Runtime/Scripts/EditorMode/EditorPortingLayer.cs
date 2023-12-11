namespace TPFive.Game.Account
{
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class EditorPortingLayer : IPortingLayer
    {
        private readonly IKeyStore keyStore = new FileKeyStore();
        private readonly ILogger logger;

        public EditorPortingLayer(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<EditorPortingLayer>();
        }

        public override void Login(string clientId, string domain, string redirectUri, IAuthCallback<string> callback)
        {
            logger.LogWarning($"{nameof(Login)} by webview function not support in editor mode");
            callback?.OnFailure(AuthError.FunctionNotSupported.Code, "not for Editor Mode.");
        }

        public override void Logout(string clientId, string domain, string redirectUri, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(Logout));
            callback.OnSuccess("logout success");
        }

        public override void ReadInfo(string key, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(ReadInfo));
            string info = keyStore.ReadInfo(key);
            if (string.IsNullOrEmpty(info))
            {
                callback?.OnFailure(AuthError.ReadDataFailed.Code, "file not found or data is empty");
                return;
            }

            callback.OnSuccess(info);
        }

        public override void SaveInfo(string key, string value, IAuthCallback<bool> callback)
        {
            logger.LogInformation(nameof(SaveInfo));
            bool saveResult = keyStore.SaveInfo(key, value);
            if (!saveResult)
            {
                callback?.OnFailure(AuthError.SaveDataFailed.Code, AuthError.SaveDataFailed.Message);
                return;
            }

            callback.OnSuccess(saveResult);
        }

        public override void DeleteInfo(string key, IAuthCallback<bool> callback)
        {
            logger.LogInformation(nameof(DeleteInfo));
            bool deleteResult = keyStore.DeleteInfo(key);
            if (!deleteResult)
            {
                callback?.OnFailure(AuthError.DeleteDataFailed.Code, AuthError.DeleteDataFailed.Message);
                return;
            }

            callback.OnSuccess(deleteResult);
        }
    }
}