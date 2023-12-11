using VContainer.Unity;

namespace TPFive.Game.Assist
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        void SetLatestLifetimeScope(LifetimeScope lifetimeScope);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        void SetLatestLifetimeScope(LifetimeScope lifetimeScope);
    }
}
