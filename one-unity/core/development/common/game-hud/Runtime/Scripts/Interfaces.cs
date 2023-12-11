namespace TPFive.Game.Hud
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        void ShowHud(GeneralContext generalContext);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        void ShowHud(GeneralContext generalContext);
    }
}
