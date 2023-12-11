using Microsoft.Extensions.Logging;
using QFSW.QC;
using VContainer.Unity;

namespace TPFive.Extended.QuantumConsole
{
    using TPFive.Game.Logging;

    using GameAssist = TPFive.Game.Assist;

    public sealed partial class ServiceProvider :
        GameAssist.IServiceProvider
    {
        private LifetimeScope _lifetimeScope;

        public void SetLatestLifetimeScope(LifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        private void SetupQuantumConsole()
        {
            Logger.LogDebug(
                "{Method}",
                nameof(SetupQuantumConsole));

            QuantumRegistry.RegisterObject<ServiceProvider>(this);
        }

        private void CleanupQuantumConsole()
        {
            Logger.LogDebug(
                "{Method}",
                nameof(CleanupQuantumConsole));

            QuantumRegistry.DeregisterObject<ServiceProvider>(this);
        }

        // This method shows how to use Quantum Console without using static methods.
        [Command("load-home", MonoTargetType.Registry)]
        private void LoadHome()
        {
            Logger.LogDebug(
                "{Method}",
                nameof(LoadHome));

            // As GetSceneContextFromTop is removed, just skip check and some log.
            _pubBackToHome.Publish(new Game.Messages.BackToHome
            {
            });
        }
    }
}
