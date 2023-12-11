using Microsoft.Extensions.Logging;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.InputXREvent
{
    public class XREventInteractable : MRTKBaseInteractable, IXREventReceiver
    {
        [SerializeField]
        private XRInputDataEvent clickEvent;
        [SerializeField]
        private XRInputDataEvent doubleClickEvent;
        [SerializeField]
        private XRInputDataEvent longPressEvent;

        private ILoggerFactory _loggerFactory;
        private ILogger<XREventInteractable> _logger;

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
                        _logger = _loggerFactory.CreateLogger<XREventInteractable>();
                    }
                }

                return _logger;
            }
        }

        public void AddListener(XRInputDataEvent.EventType eventType, UnityAction<XRInputData> action)
        {
            switch (eventType)
            {
                case XRInputDataEvent.EventType.Click:
                    clickEvent.AddListener(action);
                    break;
                case XRInputDataEvent.EventType.DoubleClick:
                    doubleClickEvent.AddListener(action);
                    break;
                case XRInputDataEvent.EventType.LongPress:
                    longPressEvent.AddListener(action);
                    break;
            }
        }

        public void RemoveListener(XRInputDataEvent.EventType eventType, UnityAction<XRInputData> action)
        {
            switch (eventType)
            {
                case XRInputDataEvent.EventType.Click:
                    clickEvent.RemoveListener(action);
                    break;
                case XRInputDataEvent.EventType.DoubleClick:
                    doubleClickEvent.RemoveListener(action);
                    break;
                case XRInputDataEvent.EventType.LongPress:
                    longPressEvent.RemoveListener(action);
                    break;
            }
        }

        public virtual void OnClick(XRInputData inputData)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnClick), inputData.Hand, inputData.Handshape);
            clickEvent?.Invoke(inputData);
        }

        public virtual void OnDoubleClick(XRInputData inputData)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnDoubleClick), inputData.Hand, inputData.Handshape);
            doubleClickEvent?.Invoke(inputData);
        }

        public virtual void OnLongPress(XRInputData inputData)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnLongPress), inputData.Hand, inputData.Handshape);
            longPressEvent?.Invoke(inputData);
        }

        public virtual void OnGazePinchHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGazePinchHoverEntered), hand, handshape);
        }

        public virtual void OnGazePinchHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGazePinchHoverExited), hand, handshape);
        }

        public virtual void OnGazePinchSelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGazePinchSelectEntered), hand, handshape);
        }

        public virtual void OnGazePinchSelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGazePinchSelectExited), hand, handshape);
        }

        public virtual void OnGrabHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGrabHoverEntered), hand, handshape);
        }

        public virtual void OnGrabHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGrabHoverExited), hand, handshape);
        }

        public virtual void OnGrabSelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGrabSelectEntered), hand, handshape);
        }

        public virtual void OnGrabSelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnGrabSelectExited), hand, handshape);
        }

        public virtual void OnHeadGazeHoverEntered(HoverEnterEventArgs hoverEnterEvent)
        {
            DebugLog("{0}  {1}", gameObject.name, nameof(OnHeadGazeHoverEntered));
        }

        public virtual void OnHeadGazeHoverExited(HoverExitEventArgs hoverExitEvent)
        {
            DebugLog("{0}  {1}", gameObject.name, nameof(OnHeadGazeHoverExited));
        }

        public virtual void OnHeadGazeSelectEntered(SelectEnterEventArgs selectEnterEvent)
        {
            DebugLog("{0}  {1}", gameObject.name, nameof(OnHeadGazeSelectEntered));
        }

        public virtual void OnHeadGazeSelectExited(SelectExitEventArgs selectExitEvent)
        {
            DebugLog("{0}  {1}", gameObject.name, nameof(OnHeadGazeSelectExited));
        }

        public virtual void OnHeadGazeUIHoverEntered(UIHoverEventArgs uIHoverEvent)
        {
            DebugLog("{0}  {1}", gameObject.name, nameof(OnHeadGazeUIHoverEntered));
        }

        public virtual void OnHeadGazeUIHoverExited(UIHoverEventArgs uIHoverEvent)
        {
            DebugLog("{0}  {1}", gameObject.name, nameof(OnHeadGazeUIHoverExited));
        }

        public virtual void OnPokeHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnPokeHoverEntered), hand, handshape);
        }

        public virtual void OnPokeHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnPokeHoverExited), hand, handshape);
        }

        public virtual void OnPokeSelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnPokeSelectEntered), hand, handshape);
        }

        public virtual void OnPokeSelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnPokeSelectExited), hand, handshape);
        }

        public virtual void OnRayHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnRayHoverEntered), hand, handshape);
        }

        public virtual void OnRayHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnRayHoverExited), hand, handshape);
        }

        public virtual void OnRaySelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnRaySelectEntered), hand, handshape);
        }

        public virtual void OnRaySelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnRaySelectExited), hand, handshape);
        }

        public virtual void OnRayUIHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            UIHoverEventArgs uIHoverEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnRayUIHoverEntered), hand, handshape);
        }

        public virtual void OnRayUIHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            UIHoverEventArgs uIHoverEventArgs)
        {
            DebugLog("{0}  {1}  {2}", nameof(OnRayUIHoverExited), hand, handshape);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            clickEvent.RemoveAllListeners();
            doubleClickEvent.RemoveAllListeners();
            longPressEvent.RemoveAllListeners();
        }

        private void DebugLog(string format, params object[] args)
        {
#if UNITY_EDITOR
            Debug.LogFormat(format, args);
#else
            Logger.LogDebug(format, args);
#endif
        }
    }
}