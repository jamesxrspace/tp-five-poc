using Microsoft.Extensions.Logging;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using TPFive.Extended.ScreenPointer;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.InputXREvent
{
    public sealed class XREventTransmitterMobile : MonoBehaviour
    {
        private readonly XRInputData _lastInputData = new XRInputData();

        [SerializeField]
        private Transform controller;
        [SerializeField]
        private ScreenPointerInteractor screenPointerInteractor;

        private PlatformInputSettings _platformInputSettings;
        private ILoggerFactory _loggerFactory;
        private ILogger<XREventTransmitterMobile> _logger;
        private IXRInputEventReceiver _inputEventReceiver;
        private float _pointDownTime;
        private float _clickedTime;
        private int _clickCount;

        private ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    var context = Loxodon.Framework.Contexts.Context.GetApplicationContext();
                    var service = context.GetService<Game.ReferenceLocator.IService>();
                    _loggerFactory = service.GetInstance<ILoggerFactory>();
                    if (_loggerFactory != null)
                    {
                        _logger = _loggerFactory.CreateLogger<XREventTransmitterMobile>();
                    }
                }

                return _logger;
            }
        }

        private float WaitingBufferTime => (_platformInputSettings == null) ? 0.25f : _platformInputSettings.WaitingBufferTime;

        private float ClickThreshold => (_platformInputSettings == null) ? 0.25f : _platformInputSettings.ClickThreshold;

        private float LongPressThreshold => (_platformInputSettings == null) ? 0.5f : _platformInputSettings.LongPressThreshold;

        [Inject]
        public void Construct(PlatformInputSettings platformInputSettings)
        {
            _platformInputSettings = platformInputSettings;
        }

        public void OnScreenRayHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRayHoverEntered(Handedness.None, HandshapeTypes.HandshapeId.None, hoverEnterEventArgs);
            }
        }

        public void OnScreenRayHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRayHoverExited(Handedness.None, HandshapeTypes.HandshapeId.None, hoverEnterEventArgs);
            }
        }

        public void OnScreenRaySelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRaySelectEntered(Handedness.None, HandshapeTypes.HandshapeId.None, selectEnterEventArgs);
            }

            if (selectEnterEventArgs.interactableObject is IXRInputEventReceiver inputEventReceiver)
            {
                SetCustomInputEventValue(Handedness.None, HandshapeTypes.HandshapeId.None, selectEnterEventArgs, inputEventReceiver);
            }
        }

        public void OnScreenRaySelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRaySelectExited(Handedness.None, HandshapeTypes.HandshapeId.None, selectExitEventArgs);
            }

            if (selectExitEventArgs.interactableObject is IXRInputEventReceiver inputEventReceiver)
            {
                JudgeCustomInputEvent(selectExitEventArgs, inputEventReceiver);
            }
        }

        public void OnScreenRayUIHoverEntered(UIHoverEventArgs uIHoverEventArgs)
        {
            if (uIHoverEventArgs.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRayUIHoverEntered(Handedness.None, HandshapeTypes.HandshapeId.None, uIHoverEventArgs);
            }
        }

        public void OnScreenRayUIHoverExited(UIHoverEventArgs uIHoverEventArgs)
        {
            if (uIHoverEventArgs.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRayUIHoverExited(Handedness.None, HandshapeTypes.HandshapeId.None, uIHoverEventArgs);
            }
        }

        private void Start()
        {
            if (screenPointerInteractor == null)
            {
                screenPointerInteractor = controller.GetComponentInChildren<ScreenPointerInteractor>();
            }

            SetupEvent();
        }

        private void Update()
        {
            if (_clickCount != 0 && Time.time - _clickedTime >= WaitingBufferTime && _inputEventReceiver != null)
            {
                switch (_clickCount)
                {
                    case 1:
                        if (_inputEventReceiver is IXRClickEventReceiver clickEventReceiver)
                        {
                            clickEventReceiver.OnClick(_lastInputData);
                        }

                        break;
                    case 2:
                        if (_inputEventReceiver is IXRDoubleClickEventReceiver doubleClickEventReceiver)
                        {
                            doubleClickEventReceiver.OnDoubleClick(_lastInputData);
                        }

                        break;
                }

                _inputEventReceiver = null;
                _clickCount = 0;
                _pointDownTime = 0;
            }

            if (_inputEventReceiver is IXRLongPressEventReciver longPressEventReciver && _pointDownTime != 0 && Time.time - _pointDownTime >= LongPressThreshold && _inputEventReceiver != null)
            {
                longPressEventReciver.OnLongPress(_lastInputData);
                _inputEventReceiver = null;
                _pointDownTime = 0;
            }
        }

        private void SetupEvent()
        {
            if (screenPointerInteractor != null)
            {
                screenPointerInteractor.hoverEntered.AddListener(OnScreenRayHoverEntered);
                screenPointerInteractor.hoverExited.AddListener(OnScreenRayHoverExited);
                screenPointerInteractor.selectEntered.AddListener(OnScreenRaySelectEntered);
                screenPointerInteractor.selectExited.AddListener(OnScreenRaySelectExited);
                screenPointerInteractor.uiHoverEntered.AddListener(OnScreenRayUIHoverEntered);
                screenPointerInteractor.uiHoverExited.AddListener(OnScreenRayUIHoverExited);
            }
        }

        private void SetCustomInputEventValue(Handedness hand, HandshapeTypes.HandshapeId handshape, BaseInteractionEventArgs selectEnterEventArgs, IXRInputEventReceiver inputEventReceiver)
        {
            _inputEventReceiver = inputEventReceiver;
            _pointDownTime = Time.time;
            _lastInputData.Hand = hand;
            _lastInputData.Handshape = handshape;
            _lastInputData.Args = selectEnterEventArgs;
        }

        private void JudgeCustomInputEvent(BaseInteractionEventArgs selectExitEventArgs, IXRInputEventReceiver inputEventReceiver)
        {
            if (_inputEventReceiver == inputEventReceiver && Time.time - _pointDownTime < ClickThreshold)
            {
                _clickCount += 1;
                _clickedTime = Time.time;

                _lastInputData.Hand = Handedness.None;
                _lastInputData.Handshape = HandshapeTypes.HandshapeId.None;
                _lastInputData.Args = selectExitEventArgs;
            }

            _pointDownTime = 0f;
        }
    }
}