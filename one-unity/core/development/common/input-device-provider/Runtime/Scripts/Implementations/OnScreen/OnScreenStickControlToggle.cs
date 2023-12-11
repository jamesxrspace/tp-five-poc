using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UniRx;
using UnityEngine;
using VContainer;
using IOnScreenControlService = TPFive.Extended.InputDeviceProvider.OnScreen.IService;

namespace TPFive.Extended.InputDeviceProvider.OnScreen
{
    /// <summary>
    /// Toggle the active state of the on-screen stick controller.
    /// Let the user can decide the active state of the stick controller in the scene by inspector.
    /// </summary>
    public sealed class OnScreenStickControlToggle : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private StickControllerActiveState[] activeStates;

        private IOnScreenControlService onScreenControlService;
        private CompositeDisposable disposables = new CompositeDisposable();

        private IDictionary<StickControllerType, bool> activeStateDict = new Dictionary<StickControllerType, bool>();

        private enum StickControllerType
        {
            MoveStick,
            RotateStick,
        }

        [Inject]
        public void Construct(IOnScreenControlService onScreenControlService)
        {
            this.onScreenControlService = onScreenControlService;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            activeStateDict.Clear();

            if (activeStates == null)
            {
                return;
            }

            foreach (var activeState in activeStates)
            {
                activeStateDict.Add(activeState.TargetType, activeState.IsActive);
            }
        }

        private void Start()
        {
            if (activeStates == null)
            {
                return;
            }

            foreach (var targetType in activeStateDict.Keys)
            {
                switch (targetType)
                {
                    case StickControllerType.MoveStick:
                        onScreenControlService.MoveStickController
                            .Subscribe(OnMoveStickControllerChanged)
                            .AddTo(disposables);
                        break;
                    case StickControllerType.RotateStick:
                        onScreenControlService.RotateStickController
                            .Subscribe(OnRotateStickControllerChanged)
                            .AddTo(disposables);
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            if (disposables == null)
            {
                return;
            }

            disposables.Dispose();
            disposables = null;
        }

        private void OnMoveStickControllerChanged(IOnScreenStickController controller)
        {
            if (controller == null ||
                !activeStateDict.TryGetValue(StickControllerType.MoveStick, out bool isActive))
            {
                return;
            }

            // Assign the active state to the controller.
            controller.IsActive.Value = isActive;
        }

        private void OnRotateStickControllerChanged(IOnScreenStickController controller)
        {
            if (controller == null ||
                !activeStateDict.TryGetValue(StickControllerType.RotateStick, out bool isActive))
            {
                return;
            }

            // Assign the active state to the controller.
            controller.IsActive.Value = isActive;
        }

        [System.Serializable]
        private struct StickControllerActiveState
        {
            public StickControllerType TargetType;

            public bool IsActive;
        }
    }
}
