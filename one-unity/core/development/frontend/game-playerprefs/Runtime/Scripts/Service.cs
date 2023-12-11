namespace TPFive.Game.PlayerPrefs
{
    using System.Collections.Generic;
    using MessagePipe;
    using TPFive.Model;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using UniRx;
    using VContainer;

    public class PrefsVariance
    {
        public string key;
        public Value? Value;
    }

    [Dispose]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
        private readonly Dictionary<string, Prefs> cachedPrefs = new ();
        private readonly ISubscriber<FlutterMessage> subFlutterMessage;
        private readonly IPublisher<PrefsVariance> pubPrefsVariance;

        [Inject]
        public Service(ISubscriber<FlutterMessage> subFlutterMessage, IPublisher<PrefsVariance> pubPrefsVariance)
        {
            this.subFlutterMessage = subFlutterMessage;
            this.pubPrefsVariance = pubPrefsVariance;

            this.subFlutterMessage.Subscribe(OnPrefsChanged, arg => arg.Type == FlutterMessageTypeEnum.Prefs).AddTo(compositeDisposable);
        }

        public Prefs GetPrefsByKey(string key)
        {
            return cachedPrefs.TryGetValue(key, out var prefs) ? prefs : default;
        }

        private void OnPrefsChanged(FlutterMessage flutterMessage)
        {
            var prefsData = PrefsData.FromJson(flutterMessage.Data);
            foreach (var data in prefsData.Prefs)
            {
                var key = data.Key;
                if (!data.Value.Value.HasValue)
                {
                    cachedPrefs.Remove(key);
                    continue;
                }

                cachedPrefs[key] = data.Value;
                pubPrefsVariance.Publish(new PrefsVariance()
                {
                    key = key,
                    Value = data.Value.Value,
                });
            }
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                compositeDisposable?.Dispose();
            }

            _disposed = true;
        }
    }
}
