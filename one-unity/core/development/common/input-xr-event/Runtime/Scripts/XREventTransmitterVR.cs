using Microsoft.Extensions.Logging;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.InputXREvent
{
    public sealed class XREventTransmitterVR : MonoBehaviour, ITransmitterVR
    {
        private readonly XRInputData _rightLastInputData = new XRInputData();
        private readonly XRInputData _leftLastInputData = new XRInputData();

        [SerializeField]
        private Transform rightHand;
        [SerializeField]
        private Transform leftHand;
        [SerializeField]
        private PokeInteractor rightPokeInteractor;
        [SerializeField]
        private MRTKRayInteractor rightRayInteractor;
        [SerializeField]
        private GrabInteractor rightGrabInteractor;
        [SerializeField]
        private GazePinchInteractor rightGazePinchInteractor;
        [SerializeField]
        private PokeInteractor leftPokeInteractor;
        [SerializeField]
        private MRTKRayInteractor leftRayInteractor;
        [SerializeField]
        private GrabInteractor leftGrabInteractor;
        [SerializeField]
        private GazePinchInteractor leftGazePinchInteractor;
        [SerializeField]
        private GazeInteractor gazeInteractor;

        private ILoggerFactory _loggerFactory;
        private ILogger<XREventTransmitterVR> _logger;
        private PlatformInputSettings _platformInputSettings;
        private IXRInputEventReceiver _rightInputEnterReceiver;
        private float _rightPointDownTime;
        private float _rightClickedTime;
        private int _rightClickCount;
        private IXRInputEventReceiver _leftInputEnterReceiver;
        private float _leftPointDownTime;
        private float _leftClickedTime;
        private int _leftClickCount;

        public Transform RightHand => rightHand;

        public Transform LeftHand => leftHand;

        public PokeInteractor RightPokeInteractor => rightPokeInteractor;

        public MRTKRayInteractor RightRayInteractor => rightRayInteractor;

        public GrabInteractor RightGrabInteractor => rightGrabInteractor;

        public GazePinchInteractor RightGazePinchInteractor => rightGazePinchInteractor;

        public PokeInteractor LeftPokeInteractor => leftPokeInteractor;

        public MRTKRayInteractor LeftRayInteractor => leftRayInteractor;

        public GrabInteractor LeftGrabInteractor => leftGrabInteractor;

        public GazePinchInteractor LeftGazePinchInteractor => leftGazePinchInteractor;

        public GazeInteractor GazeInteractor => gazeInteractor;

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
                        _logger = _loggerFactory.CreateLogger<XREventTransmitterVR>();
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

        public void OnHeadGazeHoverEntered(HoverEnterEventArgs hoverEnterEvent)
        {
            if (hoverEnterEvent.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnHeadGazeHoverEntered(hoverEnterEvent);
            }
        }

        public void OnHeadGazeHoverExited(HoverExitEventArgs hoverExitEvent)
        {
            if (hoverExitEvent.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnHeadGazeHoverExited(hoverExitEvent);
            }
        }

        public void OnHeadGazeSelectEntered(SelectEnterEventArgs selectEnterEvent)
        {
            if (selectEnterEvent.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnHeadGazeSelectEntered(selectEnterEvent);
            }
        }

        public void OnHeadGazeSelectExited(SelectExitEventArgs selectExitEvent)
        {
            if (selectExitEvent.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnHeadGazeSelectExited(selectExitEvent);
            }
        }

        public void OnHeadGazeUIHoverEntered(UIHoverEventArgs uIHoverEvent)
        {
            if (uIHoverEvent.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnHeadGazeUIHoverEntered(uIHoverEvent);
            }
        }

        public void OnHeadGazeUIHoverExited(UIHoverEventArgs uIHoverEvent)
        {
            if (uIHoverEvent.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnHeadGazeUIHoverExited(uIHoverEvent);
            }
        }

        public void OnRightPokeHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnPokeHoverEntered(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightPokeHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnPokeHoverExited(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightPokeSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnPokeSelectEntered(Handedness.Right, handshapeId, selectEnterEventArgs);
            }
        }

        public void OnRightPokeSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnPokeSelectExited(Handedness.Right, handshapeId, selectExitEventArgs);
            }
        }

        public void OnRightRayHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnRayHoverEntered(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightRayHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnRayHoverExited(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightRaySelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnRaySelectEntered(Handedness.Right, handshapeId, selectEnterEventArgs);
            }

            if (selectEnterEventArgs.interactableObject is IXRInputEventReceiver inputEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                SetCustomInputEventValue(Handedness.Right, handshapeId, selectEnterEventArgs, inputEventReceiver);
            }
        }

        public void OnRightRaySelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                mrtkEventReceiver.OnRaySelectExited(Handedness.Right, handshapeId, selectExitEventArgs);
            }

            if (selectExitEventArgs.interactableObject is IXRInputEventReceiver inputEventReceiver)
            {
                JudgeCustomInputEvent(Handedness.Right, handshapeId, selectExitEventArgs, inputEventReceiver);
            }
        }

        public void OnRightRayUIHoverEntered(UIHoverEventArgs uIHoverEventArgs)
        {
            if (uIHoverEventArgs.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnRayUIHoverEntered(Handedness.Right, handshapeId, uIHoverEventArgs);
            }
        }

        public void OnRightRayUIHoverExited(UIHoverEventArgs uIHoverEventArgs)
        {
            if (uIHoverEventArgs.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnRayUIHoverExited(Handedness.Right, handshapeId, uIHoverEventArgs);
            }
        }

        public void OnRightGrabHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGrabHoverEntered(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightGrabHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGrabHoverExited(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightGrabSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGrabSelectEntered(Handedness.Right, handshapeId, selectEnterEventArgs);
            }
        }

        public void OnRightGrabSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGrabSelectExited(Handedness.Right, handshapeId, selectExitEventArgs);
            }
        }

        public void OnRightGazePinchHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchHoverEntered(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightGazePinchHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchHoverExited(Handedness.Right, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnRightGazePinchSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchSelectEntered(Handedness.Right, handshapeId, selectEnterEventArgs);
            }
        }

        public void OnRightGazePinchSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.RightHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchSelectExited(Handedness.Right, handshapeId, selectExitEventArgs);
            }
        }

        public void OnLeftPokeHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnPokeHoverEntered(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftPokeHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnPokeHoverExited(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftPokeSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnPokeSelectEntered(Handedness.Left, handshapeId, selectEnterEventArgs);
            }
        }

        public void OnLeftPokeSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnPokeSelectExited(Handedness.Left, handshapeId, selectExitEventArgs);
            }
        }

        public void OnLeftRayHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnRayHoverEntered(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftRayHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnRayHoverExited(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftRaySelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnRaySelectEntered(Handedness.Left, handshapeId, selectEnterEventArgs);
            }

            if (selectEnterEventArgs.interactableObject is IXRInputEventReceiver inputEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                SetCustomInputEventValue(Handedness.Left, handshapeId, selectEnterEventArgs, inputEventReceiver);
            }
        }

        public void OnLeftRaySelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnRaySelectExited(Handedness.Left, handshapeId, selectExitEventArgs);
            }

            if (selectExitEventArgs.interactableObject is IXRInputEventReceiver inputEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                JudgeCustomInputEvent(Handedness.Left, handshapeId, selectExitEventArgs, inputEventReceiver);
            }
        }

        public void OnLeftRayUIHoverEntered(UIHoverEventArgs uIHoverEventArgs)
        {
            if (uIHoverEventArgs.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnRayUIHoverEntered(Handedness.Left, handshapeId, uIHoverEventArgs);
            }
        }

        public void OnLeftRayUIHoverExited(UIHoverEventArgs uIHoverEventArgs)
        {
            if (uIHoverEventArgs.interactorObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnRayUIHoverExited(Handedness.Left, handshapeId, uIHoverEventArgs);
            }
        }

        public void OnLeftGrabHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGrabHoverEntered(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftGrabHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGrabHoverExited(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftGrabSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGrabSelectEntered(Handedness.Left, handshapeId, selectEnterEventArgs);
            }
        }

        public void OnLeftGrabSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGrabSelectExited(Handedness.Left, handshapeId, selectExitEventArgs);
            }
        }

        public void OnLeftGazePinchHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchHoverEntered(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftGazePinchHoverExited(HoverExitEventArgs hoverEnterEventArgs)
        {
            if (hoverEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchHoverExited(Handedness.Left, handshapeId, hoverEnterEventArgs);
            }
        }

        public void OnLeftGazePinchSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (selectEnterEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchSelectEntered(Handedness.Left, handshapeId, selectEnterEventArgs);
            }
        }

        public void OnLeftGazePinchSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (selectExitEventArgs.interactableObject is IXRMrtkEventReceiver mrtkEventReceiver)
            {
                HandshapeHelpers.TryGetHandshapeId(XRNode.LeftHand, out var handshapeId);
                mrtkEventReceiver.OnGazePinchSelectExited(Handedness.Left, handshapeId, selectExitEventArgs);
            }
        }

        private void Start()
        {
            HandshapeHelpers.TransmitterVR = this;

            if (rightPokeInteractor == null)
            {
                rightPokeInteractor = rightHand.GetComponentInChildren<PokeInteractor>();
            }

            if (rightRayInteractor == null)
            {
                rightRayInteractor = rightHand.GetComponentInChildren<MRTKRayInteractor>();
            }

            if (rightGrabInteractor == null)
            {
                rightGrabInteractor = rightHand.GetComponentInChildren<GrabInteractor>();
            }

            if (rightGazePinchInteractor == null)
            {
                rightGazePinchInteractor = rightHand.GetComponentInChildren<GazePinchInteractor>();
            }

            if (leftPokeInteractor == null)
            {
                leftPokeInteractor = leftHand.GetComponentInChildren<PokeInteractor>();
            }

            if (leftPokeInteractor == null)
            {
                leftRayInteractor = leftHand.GetComponentInChildren<MRTKRayInteractor>();
            }

            if (leftGrabInteractor == null)
            {
                leftGrabInteractor = leftHand.GetComponentInChildren<GrabInteractor>();
            }

            if (leftGazePinchInteractor == null)
            {
                leftGazePinchInteractor = leftHand.GetComponentInChildren<GazePinchInteractor>();
            }

            if (gazeInteractor == null)
            {
                gazeInteractor = GetComponentInChildren<GazeInteractor>();
            }

            SetupEvent();
        }

        private void Update()
        {
            // Left hand input event judgement
            if (_leftClickCount != 0 && Time.time - _leftClickedTime >= WaitingBufferTime && _leftInputEnterReceiver != null)
            {
                switch (_leftClickCount)
                {
                    case 1:
                        if (_leftInputEnterReceiver is IXRClickEventReceiver clickEventReceiver)
                        {
                            clickEventReceiver.OnClick(_leftLastInputData);
                        }

                        break;
                    case 2:
                        if (_leftInputEnterReceiver is IXRDoubleClickEventReceiver doubleClickEventReceiver)
                        {
                            doubleClickEventReceiver.OnDoubleClick(_leftLastInputData);
                        }

                        break;
                }

                _leftInputEnterReceiver = null;
                _leftClickCount = 0;
                _leftPointDownTime = 0;
            }

            if (_leftInputEnterReceiver is IXRLongPressEventReciver leftLongPressEventReciver && _leftPointDownTime != 0 && Time.time - _leftPointDownTime >= LongPressThreshold && _leftInputEnterReceiver != null)
            {
                leftLongPressEventReciver.OnLongPress(_leftLastInputData);
                _leftInputEnterReceiver = null;
                _leftPointDownTime = 0;
            }

            // Right hand input event judgement
            if (_rightClickCount != 0 && Time.time - _rightClickedTime >= WaitingBufferTime && _rightInputEnterReceiver != null)
            {
                switch (_rightClickCount)
                {
                    case 1:
                        if (_rightInputEnterReceiver is IXRClickEventReceiver clickEventReceiver)
                        {
                            clickEventReceiver.OnClick(_rightLastInputData);
                        }

                        break;
                    case 2:
                        if (_rightInputEnterReceiver is IXRDoubleClickEventReceiver doubleClickEventReceiver)
                        {
                            doubleClickEventReceiver.OnDoubleClick(_rightLastInputData);
                        }

                        break;
                }

                _rightInputEnterReceiver = null;
                _rightClickCount = 0;
                _rightPointDownTime = 0;
            }

            if (_rightInputEnterReceiver is IXRLongPressEventReciver rightLongPressEventReciver && _rightPointDownTime != 0 && Time.time - _rightPointDownTime >= LongPressThreshold && _rightInputEnterReceiver != null)
            {
                rightLongPressEventReciver.OnLongPress(_rightLastInputData);
                _rightInputEnterReceiver = null;
                _rightPointDownTime = 0;
            }
        }

        private void SetupEvent()
        {
            if (rightPokeInteractor != null)
            {
                rightPokeInteractor.hoverEntered.AddListener(OnRightPokeHoverEntered);
                rightPokeInteractor.hoverExited.AddListener(OnRightPokeHoverExited);
                rightPokeInteractor.selectEntered.AddListener(OnRightPokeSelectEntered);
                rightPokeInteractor.selectExited.AddListener(OnRightPokeSelectExited);
            }

            if (rightRayInteractor != null)
            {
                rightRayInteractor.hoverEntered.AddListener(OnRightRayHoverEntered);
                rightRayInteractor.hoverExited.AddListener(OnRightRayHoverExited);
                rightRayInteractor.selectEntered.AddListener(OnRightRaySelectEntered);
                rightRayInteractor.selectExited.AddListener(OnRightRaySelectExited);
                rightRayInteractor.uiHoverEntered.AddListener(OnRightRayUIHoverEntered);
                rightRayInteractor.uiHoverExited.AddListener(OnRightRayUIHoverExited);
            }

            if (rightGrabInteractor != null)
            {
                rightGrabInteractor.hoverEntered.AddListener(OnRightGrabHoverEntered);
                rightGrabInteractor.hoverExited.AddListener(OnRightGrabHoverExited);
                rightGrabInteractor.selectEntered.AddListener(OnRightGrabSelectEntered);
                rightGrabInteractor.selectExited.AddListener(OnRightGrabSelectExited);
            }

            if (rightGazePinchInteractor != null)
            {
                rightGazePinchInteractor.hoverEntered.AddListener(OnRightGazePinchHoverEntered);
                rightGazePinchInteractor.hoverExited.AddListener(OnRightGazePinchHoverExited);
                rightGazePinchInteractor.selectEntered.AddListener(OnRightGazePinchSelectEntered);
                rightGazePinchInteractor.selectExited.AddListener(OnRightGazePinchSelectExited);
            }

            if (leftPokeInteractor != null)
            {
                leftPokeInteractor.hoverEntered.AddListener(OnLeftPokeHoverEntered);
                leftPokeInteractor.hoverExited.AddListener(OnLeftPokeHoverExited);
                leftPokeInteractor.selectEntered.AddListener(OnLeftPokeSelectEntered);
                leftPokeInteractor.selectExited.AddListener(OnLeftPokeSelectExited);
            }

            if (leftRayInteractor != null)
            {
                leftRayInteractor.hoverEntered.AddListener(OnLeftRayHoverEntered);
                leftRayInteractor.hoverExited.AddListener(OnLeftRayHoverExited);
                leftRayInteractor.selectEntered.AddListener(OnLeftRaySelectEntered);
                leftRayInteractor.selectExited.AddListener(OnLeftRaySelectExited);
                leftRayInteractor.uiHoverEntered.AddListener(OnLeftRayUIHoverEntered);
                leftRayInteractor.uiHoverExited.AddListener(OnLeftRayUIHoverExited);
            }

            if (leftGrabInteractor != null)
            {
                leftGrabInteractor.hoverEntered.AddListener(OnLeftGrabHoverEntered);
                leftGrabInteractor.hoverExited.AddListener(OnLeftGrabHoverExited);
                leftGrabInteractor.selectEntered.AddListener(OnLeftGrabSelectEntered);
                leftGrabInteractor.selectExited.AddListener(OnLeftGrabSelectExited);
            }

            if (leftGazePinchInteractor != null)
            {
                leftGazePinchInteractor.hoverEntered.AddListener(OnLeftGazePinchHoverEntered);
                leftGazePinchInteractor.hoverExited.AddListener(OnLeftGazePinchHoverExited);
                leftGazePinchInteractor.selectEntered.AddListener(OnLeftGazePinchSelectEntered);
                leftGazePinchInteractor.selectExited.AddListener(OnLeftGazePinchSelectExited);
            }

            if (gazeInteractor != null)
            {
                gazeInteractor.hoverEntered.AddListener(OnHeadGazeHoverEntered);
                gazeInteractor.hoverExited.AddListener(OnHeadGazeHoverExited);
                gazeInteractor.selectEntered.AddListener(OnHeadGazeSelectEntered);
                gazeInteractor.selectExited.AddListener(OnHeadGazeSelectExited);
                gazeInteractor.uiHoverEntered.AddListener(OnHeadGazeUIHoverEntered);
                gazeInteractor.uiHoverExited.AddListener(OnHeadGazeUIHoverExited);
            }
        }

        private void SetCustomInputEventValue(Handedness hand, HandshapeTypes.HandshapeId handshape, BaseInteractionEventArgs selectEnterEventArgs, IXRInputEventReceiver inputEventReceiver)
        {
            switch (hand)
            {
                case Handedness.Left:
                    _leftInputEnterReceiver = inputEventReceiver;
                    _leftPointDownTime = Time.time;
                    _leftLastInputData.Hand = Handedness.Left;
                    _leftLastInputData.Handshape = handshape;
                    _leftLastInputData.Args = selectEnterEventArgs;
                    break;
                case Handedness.Right:
                    _rightInputEnterReceiver = inputEventReceiver;
                    _rightPointDownTime = Time.time;
                    _rightLastInputData.Hand = Handedness.Right;
                    _rightLastInputData.Handshape = handshape;
                    _rightLastInputData.Args = selectEnterEventArgs;
                    break;
            }
        }

        private void JudgeCustomInputEvent(Handedness hand, HandshapeTypes.HandshapeId handshape, BaseInteractionEventArgs selectExitEventArgs, IXRInputEventReceiver inputEventReceiver)
        {
            switch (hand)
            {
                case Handedness.Left:
                    if (_leftInputEnterReceiver == inputEventReceiver && Time.time - _leftPointDownTime < ClickThreshold)
                    {
                        _leftClickCount += 1;
                        _leftClickedTime = Time.time;

                        _leftLastInputData.Hand = hand;
                        _leftLastInputData.Handshape = handshape;
                        _leftLastInputData.Args = selectExitEventArgs;
                    }

                    _leftPointDownTime = 0f;
                    break;
                case Handedness.Right:
                    if (_rightInputEnterReceiver == inputEventReceiver && Time.time - _rightPointDownTime < ClickThreshold)
                    {
                        _rightClickCount += 1;
                        _rightClickedTime = Time.time;

                        _rightLastInputData.Hand = hand;
                        _rightLastInputData.Handshape = handshape;
                        _rightLastInputData.Args = selectExitEventArgs;
                    }

                    _rightPointDownTime = 0f;
                    break;
            }
        }
    }
}