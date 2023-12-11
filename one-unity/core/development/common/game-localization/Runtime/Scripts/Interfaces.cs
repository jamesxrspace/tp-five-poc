namespace TPFive.Game.Localization
{
    public interface IService
    {
        // IServiceProvider NullServiceProvider { get; }

        void ChangeLanguage(string language);
        string GetTerm(string termId);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        void ChangeLanguage(string language);
        string GetTerm(string termId);
    }
}
