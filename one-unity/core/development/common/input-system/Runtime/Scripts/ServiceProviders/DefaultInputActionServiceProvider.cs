using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine.InputSystem;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.InputSystem
{
    public class DefaultInputActionServiceProvider : TPFive.Game.Input.IServiceProvider
    {
        private readonly ILogger log;
        private readonly HashSet<InputActionAsset> inputActionAssets = new HashSet<InputActionAsset>();

        public DefaultInputActionServiceProvider(ILoggerFactory loggerFactory)
        {
            this.log = loggerFactory.CreateLogger<DefaultInputActionServiceProvider>();
        }

        public void RegisterInputActionAsset(InputActionAsset inputActionAsset)
        {
            if (inputActionAsset == null)
            {
                log.LogError("{Method}: input action asset is null", nameof(RegisterInputActionAsset));
                return;
            }

            if (inputActionAssets.Add(inputActionAsset))
            {
                log.LogInformation("{Method}: register input action asset. asset name: {InputActionAsset}", nameof(RegisterInputActionAsset), inputActionAsset.name);
            }
            else
            {
                log.LogWarning("{Method}: input action asset already registered. asset name: {InputActionAsset}", nameof(RegisterInputActionAsset), inputActionAsset.name);
            }
        }

        public void UnregisterInputActionAsset(InputActionAsset inputActionAsset)
        {
            if (inputActionAsset == null)
            {
                log.LogError("{Method}: input action asset is null", nameof(UnregisterInputActionAsset));
                return;
            }

            if (inputActionAssets.Remove(inputActionAsset))
            {
                log.LogInformation("{Method}: unregister input action asset. asset name: {InputActionAsset}", nameof(UnregisterInputActionAsset), inputActionAsset.name);
            }
            else
            {
                log.LogWarning("{Method}: input action asset not registered. asset name: {InputActionAsset}", nameof(UnregisterInputActionAsset), inputActionAsset.name);
            }
        }

        public bool TryGetInputAction(string actionNameOrId, out InputAction inputAction)
        {
            inputAction = null;

            foreach (var inputActionAsset in inputActionAssets)
            {
                if (inputActionAsset == null)
                {
                    continue;
                }

                inputAction = inputActionAsset.FindAction(actionNameOrId);
                if (inputAction != null)
                {
                    log.LogDebug(
                        "{Method}: found input action. action name or id: {ActionNameOrId}",
                        nameof(TryGetInputAction),
                        actionNameOrId);
                    return true;
                }
            }

            log.LogWarning(
                "{Method}: not found input action. action name or id: {ActionNameOrId}",
                nameof(TryGetInputAction),
                actionNameOrId);

            return false;
        }
    }
}
