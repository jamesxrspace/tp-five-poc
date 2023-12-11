using VContainer.Unity;

namespace TPFive.Game.Assist
{
    public sealed partial class Service
    {
        public void SetLatestLifetimeScope(LifetimeScope lifetimeScope)
        {
            var serviceProvider = GetServiceProvider(QuantumConsoleServiceProvider);

            serviceProvider.SetLatestLifetimeScope(lifetimeScope);
        }
    }
}
