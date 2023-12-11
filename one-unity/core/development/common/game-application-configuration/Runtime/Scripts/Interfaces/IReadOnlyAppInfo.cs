namespace TPFive.Game.ApplicationConfiguration
{
    /// <summary>
    /// Application information. (ReadOnly).
    /// </summary>
    public interface IReadOnlyAppInfo
    {
        string BuildEnv { get; }

        string Region { get; }

        string AuthingDomain { get; }

        string AuthingClientId { get; }

        IReadOnlyGameServerInfo GameServer { get; }
    }
}
