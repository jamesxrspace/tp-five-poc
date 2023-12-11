using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace TPFive.Game.Record.Entry
{
    public class UIStateManager
    {
        private readonly ILogger log;

        private Dictionary<UINameTag, UIState> cacheUIStateDict = new Dictionary<UINameTag, UIState>();
        private Dictionary<UINameTag, UIState> defaultUIStateDict = new Dictionary<UINameTag, UIState>();

        private string cacheJson;
        private string defaultJson;

        private bool isDirty = false;
        private bool isDefaultDirty = false;

        public UIStateManager(UIStateSettings uiStateSettings, ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<UIStateManager>();

            uiStateSettings.FlutterUIStateList.ForEach(state =>
            {
                var copy = new UIState()
                {
                    IsVisible = state.State.IsVisible,
                    IsHighlight = state.State.IsHighlight,
                    IsInteractable = state.State.IsInteractable,
                };

                defaultUIStateDict.TryAdd(state.Id, state.State);
                AddUIState(state.Id, copy);
            });
        }

        public Dictionary<UINameTag, UIState> CacheUIStateDict => cacheUIStateDict;

        // TODO: Send json to flutter for updating flutter UI display
        public string CacheJson
        {
            get
            {
                UpdateUIStatesToJson();
                return cacheJson;
            }
        }

        public string DefaultJson
        {
            get
            {
                if (!isDefaultDirty)
                {
                    defaultJson = JsonConvert.SerializeObject(defaultUIStateDict);
                    isDefaultDirty = true;
                }

                return defaultJson;
            }
        }

        public void AddUIState(UINameTag id, UIState state)
        {
            if (cacheUIStateDict.TryAdd(id, state))
            {
                log.LogInformation($"AddUIState : {id} add succeed");
                isDirty = true;
            }
            else
            {
                log.LogWarning($"AddUIState : {id} is already in dictonary");
            }
        }

        public void UpdateUIState(UINameTag id, UIState state)
        {
            if (cacheUIStateDict.TryGetValue(id, out var oldState))
            {
                oldState.Update(state);
                log.LogInformation($"UpdateUIState : {id} update succeed");
                isDirty = true;
            }
            else
            {
                log.LogWarning($"UpdateUIState : {id} is not found");
            }
        }

        public void ResetCacheUIState()
        {
            foreach (var kvp in cacheUIStateDict)
            {
                if (defaultUIStateDict.TryGetValue(kvp.Key, out var state))
                {
                    kvp.Value.Update(state);
                }
            }

            isDirty = true;
        }

        public void RemoveUIState(UINameTag id)
        {
            if (cacheUIStateDict.Remove(id))
            {
                log.LogWarning($"RemoveUIState : {id} remove succeed");
                isDirty = true;
            }
            else
            {
                log.LogWarning($"RemoveUIState : {id} is not found");
            }
        }

        public void UpdateUIStatesToJson()
        {
            if (!isDirty)
            {
                return;
            }

            cacheJson = JsonConvert.SerializeObject(cacheUIStateDict);
            log.LogInformation($"Cache Json : {cacheJson}");
            isDirty = false;
        }

        public UIState GetUIState(UINameTag id)
        {
            cacheUIStateDict.TryGetValue(id, out var oldState);
            return oldState;
        }
    }
}
