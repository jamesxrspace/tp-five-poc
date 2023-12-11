namespace TPFive.Game.Account
{
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    internal class IosPortingLayer : IPortingLayer
    {
        private readonly ILogger logger;

        public IosPortingLayer(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<EditorPortingLayer>();
        }

        public override void Login(string clientId, string domain, string redirectUri, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(Login));
        }

        public override void Logout(string clientId, string domain, string redirectUri, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(Logout));
        }

        public override void ReadInfo(string key, IAuthCallback<string> callback)
        {
            logger.LogInformation(nameof(ReadInfo));
        }

        public override void SaveInfo(string key, string value, IAuthCallback<bool> callback)
        {
            logger.LogInformation(nameof(SaveInfo));
        }

        public override void DeleteInfo(string key, IAuthCallback<bool> callback)
        {
            logger.LogInformation(nameof(Login));
        }
    }
}
