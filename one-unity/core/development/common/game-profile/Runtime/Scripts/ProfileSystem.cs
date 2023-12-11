using System;
using UnityEngine;
using VContainer;
using MessagePipe;
using TPFive.Game.PlayerPrefs;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using PlayerPrefsService = TPFive.Game.PlayerPrefs.IService;

namespace TPFive.Game.Profile
{
    public class ProfileSystem : MonoBehaviour
    {
        private const string PlayerPrefsKey = "Profiler";
        private AppEntrySettings appEntrySettings;
        private PlayerPrefsService playerPrefsService;
        private IDisposable prefsVarianceSub;
        private GameObject statsMonitor;

        [Inject]
        public void InjectDpendency(
            AppEntrySettings appEntrySettings,
            PlayerPrefsService playerPrefsService,
            ISubscriber<PrefsVariance> prefsVarianceSubscriber)
        {
            this.appEntrySettings = appEntrySettings;
            this.playerPrefsService = playerPrefsService;
            this.prefsVarianceSub = prefsVarianceSubscriber.Subscribe(
                (msg) => ToggleStatsMonitor(ResolvePlayerPrefsToggle(PlayerPrefsKey)),
                (msg) => msg.key == PlayerPrefsKey);
        }

        protected void Start()
        {
            ResolveStatsMonitorToggle();
        }

        protected void OnDestroy()
        {
            prefsVarianceSub?.Dispose();
            prefsVarianceSub = null;
        }

        private void ResolveStatsMonitorToggle()
        {
            ToggleStatsMonitor(Application.isEditor
                || !GameApp.IsFlutter
                || ResolvePlayerPrefsToggle(PlayerPrefsKey));
        }

        private bool ResolvePlayerPrefsToggle(string playerPrefsKey)
        {
            var prefs = playerPrefsService.GetPrefsByKey(playerPrefsKey);
            return prefs?.Value?.Bool ?? false;
        }

        private void ToggleStatsMonitor(bool show)
        {
            if (statsMonitor == null)
            {
                statsMonitor = appEntrySettings.profileSystemSetting.StatsMonitorPrefab.InstantiateAsync(Vector3.zero, Quaternion.identity).WaitForCompletion();
            }

            statsMonitor.SetActive(show);
        }
    }
}
