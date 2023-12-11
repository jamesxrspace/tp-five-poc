using System;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Analytics;
using Microsoft.Extensions.Logging;

namespace TPFive.Extended.GoogleAnalytics
{
    using TPFive.Game.Logging;
    using GameAnalytics = TPFive.Game.Analytics;

    public sealed partial class ServiceProvider :
        GameAnalytics.IServiceProvider
    {
        public void SetUser(string userID)
        {
            if (!isInitialized)
            {
                Logger.LogError("Attempted to call SetUser without resolving Firebase dependencies.");
                return;
            }

            FirebaseAnalytics.SetUserId(userID);
        }

        public void ScreenView(string screenName, string screenClass)
        {
            if (!isInitialized)
            {
                Logger.LogError("Attempted to call ScreenView without resolving Firebase dependencies.");
                return;
            }

            var parameters = new Parameter[2]
            {
                new Parameter(FirebaseAnalytics.ParameterScreenName, screenName),
                new Parameter(FirebaseAnalytics.ParameterScreenClass, screenClass),
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, parameters);
        }
    }
}
