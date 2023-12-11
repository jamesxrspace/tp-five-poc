using Microsoft.Extensions.Logging;

namespace TPFive.Extended.I2Localization
{
    using GameLocalization = TPFive.Game.Localization;

    public sealed partial class ServiceProvider :
        GameLocalization.IServiceProvider
    {
        //
        public void ChangeLanguage(string language)
        {
            Logger.LogDebug(
                "{Method} - {Language}",
                nameof(ChangeLanguage),
                language);
        }

        public string GetTerm(string termId)
        {
            Logger.LogDebug(
                "{Method} - {TermId}",
                nameof(GetTerm),
                termId);

            return default;
        }
    }
}
