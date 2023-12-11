namespace TPFive.Game.Audio
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        void PlaySound(string name);
    }

    public interface IServiceProvider :
        TPFive.Game.IServiceProvider
    {
        void PlaySound(string name);
    }
}
