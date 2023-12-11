namespace TPFive.Game.ApplicationConfiguration
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public class AppInfo : IReadOnlyAppInfo
    {
        public string BuildEnv { get; set; }

        public string Region { get; set; }

        public string AuthingDomain { get; set; }

        public string AuthingClientId { get; set; }

        public GameServerInfo GameServer { get; set; }

        IReadOnlyGameServerInfo IReadOnlyAppInfo.GameServer => GameServer;
    }
}