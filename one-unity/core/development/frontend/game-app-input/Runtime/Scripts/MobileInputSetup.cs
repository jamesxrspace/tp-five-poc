using System;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Extended.InputDeviceProvider.OnScreen;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IOnScreenControlService = TPFive.Extended.InputDeviceProvider.OnScreen.IService;

namespace TPFive.Game.App.Input.Mobile
{
    public sealed class MobileInputSetup : MonoBehaviour
    {
        [Header("Stick controllers")]
        [SerializeField]
        private AssetReferenceGameObject moveStickControllerPrefabRef;

        [SerializeField]
        private AssetReferenceGameObject rotateStickControllerPrefabRef;

        private ILogger log;
        private IOnScreenControlService onScreenControlService;
        private IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;
        private GameObject moveStickControllerGo;
        private GameObject rotateStickControllerGo;

        [Inject]
        private void Construct(
            ILoggerFactory loggerFactory,
            IOnScreenControlService onScreenControlService,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage)
        {
            this.log = loggerFactory.CreateLogger<MobileInputSetup>();
            this.onScreenControlService = onScreenControlService;
            this._pubPostUnityMessage = pubPostUnityMessage;

            InitMobileInput().Forget();
        }

        private void OnDestroy()
        {
            if (onScreenControlService != null)
            {
                onScreenControlService.MoveStickController.Value = null;
                onScreenControlService.RotateStickController.Value = null;
            }

            ReleaseAddressableInstance(moveStickControllerPrefabRef, moveStickControllerGo);
            ReleaseAddressableInstance(rotateStickControllerPrefabRef, rotateStickControllerGo);
        }

        private async UniTaskVoid InitMobileInput()
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(MobileInputSetup)} - Begin {nameof(InitMobileInput)}");

            log.LogDebug("{Method}: Initial mobile input begin", nameof(InitMobileInput));

            (moveStickControllerGo, rotateStickControllerGo) = await UniTask.WhenAll(
                InstantiatePrefabAsync(moveStickControllerPrefabRef),
                InstantiatePrefabAsync(rotateStickControllerPrefabRef));

            var moveStickController = moveStickControllerGo.GetComponent<OnScreenStickController>();
            onScreenControlService.MoveStickController.Value = moveStickController;

            var rotateStickController = rotateStickControllerGo.GetComponent<OnScreenStickController>();
            onScreenControlService.RotateStickController.Value = rotateStickController;

            log.LogDebug("{Method}: Initial mobile input finish", nameof(InitMobileInput));

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(MobileInputSetup)} - Finish {nameof(InitMobileInput)}");
        }

        private async UniTask<GameObject> InstantiatePrefabAsync(AssetReferenceGameObject prefabRef)
        {
            object runtimeKey = prefabRef?.RuntimeKey;

            log.LogDebug(
                "{Method}: Instantiate {RuntimeKey} begin",
                nameof(InstantiatePrefabAsync),
                runtimeKey);

            if (prefabRef == null)
            {
                throw new ArgumentNullException(
                    nameof(prefabRef),
                    "Prefab reference is null.");
            }

            if (!prefabRef.RuntimeKeyIsValid())
            {
                throw new ArgumentException(
                    "Prefab reference runtime key is invalid. Please check the addressable prefab reference.");
            }

            var loadTask = prefabRef.InstantiateAsync();
            await loadTask;

            log.LogDebug("{Method}: Instantiate {RuntimeKey} end. Status: {Status}",
                nameof(InstantiatePrefabAsync),
                runtimeKey,
                loadTask.Status);

            return loadTask.Result;
        }

        private void ReleaseAddressableInstance(AssetReferenceGameObject prefabRef, GameObject instanceGo)
        {
            if (prefabRef == null ||
                !prefabRef.IsValid() ||
                instanceGo == null)
            {
                return;
            }

            prefabRef.ReleaseInstance(instanceGo);

            log.LogDebug(
                "{Method}: Release ({RuntimeKey})",
                nameof(ReleaseAddressableInstance),
                prefabRef.RuntimeKey);
        }
    }
}
