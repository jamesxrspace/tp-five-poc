namespace TPFive.Game.Analytics
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        void SetUser(string userID);
        void ScreenView(string screenName, string screenClass);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        void SetUser(string userID);
        void ScreenView(string screenName, string screenClass);
    }
}
