using CandyCoded.env;

namespace TPFive.Game.ApplicationConfiguration
{
    public class AppConfigService : IAppConfigService
    {
        private const string BuildEnvKey = "BUILD_ENV";
        private const string RegionKey = "REGION";
        private const string AuthingDomainKey = "AUTHING_DOMAIN";
        private const string AuthingClientIdKey = "AUTHING_CLIENT_ID";
        private const string ServerDomainKey = "SERVER_DOMAIN";

        private IReadOnlyAppInfo appInfo;

        public IReadOnlyAppInfo GetAppInfo()
        {
            return appInfo ??= LoadAppInfo();
        }

        private IReadOnlyAppInfo LoadAppInfo()
        {
            return new AppInfo
            {
                BuildEnv = env.variables[BuildEnvKey],
                Region = env.variables[RegionKey],
                AuthingDomain = env.variables[AuthingDomainKey],
                AuthingClientId = env.variables[AuthingClientIdKey],
                GameServer = new GameServerInfo
                {
                    BaseUri = env.variables[ServerDomainKey],
                },
            };
        }
    }
}