using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;
using Microsoft.Extensions.Logging;
using UnityEngine;
using TPFive.Game.Messages;

namespace TPFive.Extended.Doozy
{
    //
    using TPFive.Game.Logging;

    using GameHud = TPFive.Game.Hud;

    public partial class ServiceProvider :
        GameHud.IServiceProvider
    {
        // Need to find out when to pass these variables from some where else.
        public List<GameObject> receiveGOs;
        // public Settings settings;

        private readonly List<SignalBinding> _signalBindings = new List<SignalBinding>();

        private void SetupDoozySignal()
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupDoozySignal));

            _signalBindings.Clear();

            var adjusted =
                _settings.signalBindingDataList.Select(x =>
                {
                    return new SignalBinding
                    {
                        SignalBindingData = new SignalBindingData
                        {
                            streamCategory = x.streamCategory,
                            streamName = x.streamName
                        }
                    };
                });

            _signalBindings.AddRange(adjusted);

            // TODO: Extract this part as it is deep nested.
            _signalBindings.ForEach(x =>
            {
                x.SignalStream = SignalStream.Get(x.SignalBindingData.streamCategory, x.SignalBindingData.streamName);
                x.SignalReceiver = new SignalReceiver();
                x.SignalReceiver.SetOnSignalCallback(signal =>
                {
                    if (signal.hasValue)
                    {
                        {
                            var v = 0;
                            var result = signal.TryGetValue<int>(out v);
                            if (result)
                            {
                                Logger.LogEditorDebug(
                                    "{Method} - {Value}",
                                    nameof(SetupDoozySignal),
                                        v);
                                // CustomEvent.Trigger(rgo, $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName} - int", new object[] { v });
                                _pubHudMessage.Publish(new HudMessage
                                {
                                    // StringParams = new List<string> { $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName}", "int" },
                                    StringParams = new List<string> { x.SignalBindingData.streamCategory, x.SignalBindingData.streamName, "int" },
                                    IntParams = new List<int> { v },
                                    // GameObjectParams = new List<GameObject> { rgo }
                                });
                            }
                        }
                        {
                            var v = 0f;
                            var result = signal.TryGetValue<float>(out v);
                            if (result)
                            {
                                Logger.LogEditorDebug(
                                    "{Method} - {Value}",
                                    nameof(SetupDoozySignal),
                                    v);

                                // CustomEvent.Trigger(rgo, $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName} - float", new object[] { v });
                                _pubHudMessage.Publish(new HudMessage
                                {
                                    // StringParams = new List<string> { $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName}", "float" },
                                    StringParams = new List<string> { x.SignalBindingData.streamCategory, x.SignalBindingData.streamName, "float" },
                                    FloatParams = new List<float> { v },
                                    // GameObjectParams = new List<GameObject> { rgo }
                                });
                            }
                        }
                        {
                            var v = string.Empty;
                            var result = signal.TryGetValue<string>(out v);
                            if (result)
                            {
                                Logger.LogEditorDebug(
                                    "{Method} - {Value}",
                                    nameof(SetupDoozySignal),
                                    v);

                                // CustomEvent.Trigger(rgo, $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName} - string", new object[] { v });
                                _pubHudMessage.Publish(new HudMessage
                                {
                                    // StringParams = new List<string> { $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName}", "string", v },
                                    StringParams = new List<string> { x.SignalBindingData.streamCategory, x.SignalBindingData.streamName, "string", v },

                                    // GameObjectParams = new List<GameObject> { rgo }
                                });
                            }
                        }
                    }
                    else
                    {
                        // CustomEvent.Trigger(rgo, $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName}", new object[] { });
                        _pubHudMessage.Publish(new HudMessage
                        {
                            // StringParams = new List<string> { $"{x.SignalBindingData.streamCategory} - {x.SignalBindingData.streamName}" },
                            StringParams = new List<string> { x.SignalBindingData.streamCategory, x.SignalBindingData.streamName },

                            // GameObjectParams = new List<GameObject> { rgo }
                        });
                    }
                });
            });
        }

        private void CleanupDoozySignal()
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(CleanupDoozySignal));

            _signalBindings.ForEach(x =>
            {
                x.SignalReceiver.Disconnect();
                x.SignalReceiver.ClearOnSignalCallback();
            });
        }

        private void RegisterDoozySignal()
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(RegisterDoozySignal));

            _signalBindings.ForEach(x =>
            {
                x.SignalStream.ConnectReceiver(x.SignalReceiver);
            });
        }

        private void UnregisterDoozySignal()
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(UnregisterDoozySignal));

            _signalBindings.ForEach(x =>
            {
                x.SignalStream.DisconnectAllReceivers();
            });

            _signalBindings.Clear();
        }

        //
        public void ShowHud(GameHud.GeneralContext generalContext)
        {
            Logger.LogDebug(
                "{Method} - {GeneralContext}",
                nameof(ShowHud),
                generalContext);

            // _pubHudMessage.Publish(new HudMessage
            // {
            //
            // });

            var signalStream = SignalStream.Get("XSPO", generalContext.Name);
            signalStream.SendSignal(generalContext.Name);
        }
    }
}
