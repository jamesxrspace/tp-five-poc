using System;
using System.Collections.Generic;
using TPFive.Extended.InputSystem.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace TPFive.Extended.InputSystem.OnScreen
{
    /// <summary>
    /// A floating stick control displayed on screen and moved around by touch or other pointer input.
    /// <br/>
    /// Base on <see cref="UnityEngine.InputSystem.OnScreen.OnScreenStick"/> to implementation the floating stick.
    /// </summary>
    /// <remarks>
    /// The <see cref="OnScreenStick"/> works by simulating events from the device specified
    /// in the <see cref="OnScreenControl.controlPath"/> property.
    /// <br/>
    /// Some parts of the Input System, such as the <see cref="PlayerInput"/> component, can be set up to
    /// auto-switch to a new device when input from them is detected. When a device is switched, any currently running
    /// inputs from the previously active device are cancelled. In the case of <see cref="OnScreenStick"/>,
    /// this can mean that the <see cref="IPointerUpHandler.OnPointerUp"/> method will be called and the stick
    /// will jump back to center, even though the pointer input has not physically been released.
    /// <br/>
    /// To avoid this situation, set the <see cref="useIsolatedInputActions"/> property to true. This will create a set of local
    /// Input Actions to drive the stick that are not cancelled when device switching occurs.
    /// </remarks>
    public class OnScreenFloatingStick :
        OnScreenControl,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler
    {
        public const string DynamicOriginClickable = "DynamicOriginClickable";

        [SerializeField]
        [Tooltip("Whether the knob is allowed to move beyond the movement range. If TRUE, " +
                 "the knob will be allowed to move beyond the movement range.")]
        private bool allowKnobBeyondMovementRange;

        [SerializeField]
        [Tooltip("Maximum distance the knob can be moved from it's origin.")]
        [Min(0)]
        private float movementRange = 50;

        [SerializeField]
        [Tooltip("Defines the circular region where the onscreen control may have it's origin placed.")]
        [Min(0)]
        private float dynamicOriginRange = 100;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string stickControlPath;

        [SerializeField]
        [Tooltip("Choose how the onscreen stick will move relative to it's origin and the press position.\n\n" +
            "RelativePositionWithStaticOrigin: The control's center of origin is fixed. " +
            "The control will begin un-actuated at it's centered position and then move relative to the pointer or finger motion.\n\n" +
            "ExactPositionWithStaticOrigin: The control's center of origin is fixed. The stick will immediately jump to the " +
            "exact position of the click or touch and begin tracking motion from there.\n\n" +
            "ExactPositionWithDynamicOrigin: The control's center of origin is determined by the initial press position. " +
            "The stick will begin un-actuated at this center position and then track the current pointer or finger position.")]
        private OnScreenStick.Behaviour behaviour;

        [SerializeField]
        [Tooltip("Set this to true to prevent cancellation of pointer events due to device switching. Cancellation " +
            "will appear as the stick jumping back and forth between the pointer position and the stick center.")]
        private bool useIsolatedInputActions;

        [SerializeField]
        [Tooltip("The action that will be used to detect pointer down events on the stick control. Note that if no bindings " +
            "are set, default ones will be provided.")]
        private InputAction pointerDownAction;

        [SerializeField]
        [Tooltip("The action that will be used to detect pointer movement on the stick control. Note that if no bindings " +
            "are set, default ones will be provided.")]
        private InputAction pointerMoveAction;

        [SerializeField]
        [Tooltip("Defines the strength threshold for the stick. If the stick is moved beyond this threshold, the " +
            "visual background will change to the strong background.")]
        private float strengthThreshold = 0.5f;

        [SerializeField]
        [Tooltip("Whether display visual elements. If TRUE will display the 'visualWeakBackgroundRect'," +
                 " 'visualStrongBackgroundRect' and 'visualKnobRect', otherwise not.")]
        private bool showVisualElement = true;

        [SerializeField]
        [Tooltip("The weak background of visual stick. If strength is not above threshold will display the weak background.")]
        private RectTransform visualWeakBackgroundRect;

        [SerializeField]
        [Tooltip("The strong background of visual stick. If strength is above threshold will display the strong background.")]
        private RectTransform visualStrongBackgroundRect;

        [SerializeField]
        [Tooltip("The knob of visual stick. Knob will display on current stick position.")]
        private RectTransform visualKnobRect;

        private Vector2 startPos;
        private Vector2 pointerDownPos;

        private List<RaycastResult> raycastResults;
        private PointerEventData pointerEventData;

        private Canvas myCanvas;
        private RectTransform myRect;
        private RectTransform parentRect;

        private int uiLayer;
        private List<RaycastResult> raycastResultCache = new List<RaycastResult>();
        private bool isInteracting;

        /// <summary>
        /// Gets or sets the behaviour of stick.<br/>
        /// Defines how the onscreen stick will move relative to it's origin and the press position.
        /// </summary>
        /// <value>stick behaviour.</value>
        public OnScreenStick.Behaviour Behaviour
        {
            get => behaviour;
            set => behaviour = value;
        }

        protected override string controlPathInternal
        {
            get => stickControlPath;
            set => stickControlPath = value;
        }

        /// <summary>
        /// Callback to handle OnPointerDown UI events.
        /// </summary>
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            // If the pointer is over another UI element, don't start the interaction.
            bool isOverOtherUi = IsOverOtherUi(eventData);

            if (useIsolatedInputActions || isOverOtherUi)
            {
                return;
            }

            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            isInteracting = true;
            BeginInteraction(eventData.position, eventData.pressEventCamera);
        }

        /// <summary>
        /// Callback to handle OnPointerUp UI events.
        /// </summary>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (useIsolatedInputActions || !isInteracting)
            {
                return;
            }

            isInteracting = false;
            EndInteraction();
        }

        /// <summary>
        /// Callback to handle OnDrag UI events.
        /// </summary>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (useIsolatedInputActions || !isInteracting)
            {
                return;
            }

            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            MoveStick(eventData.position, eventData.pressEventCamera);
        }

        protected void Start()
        {
            uiLayer = LayerMask.NameToLayer("UI");

            // Get canvas
            myCanvas = GetComponentInParent<Canvas>();

            // Retrieve the self RectTransform for calculating pointer positions
            myRect = this.transform.GetComponent<RectTransform>();

            // Retrieve the parent RectTransform for calculating pointer positions
            if (transform.parent == null)
            {
                Debug.LogError("OnScreenFloatingStick needs to be attached as a child to a UI Canvas to function properly.");
                return;
            }

            parentRect = this.transform.parent.GetComponent<RectTransform>();

            // Hide the visual elements
            SetRectTransformActive(visualWeakBackgroundRect, false);
            SetRectTransformActive(visualStrongBackgroundRect, false);
            SetRectTransformActive(visualKnobRect, false);

            if (useIsolatedInputActions)
            {
                // avoid allocations every time the pointer down event fires by allocating these here
                // and re-using them
                raycastResults = new List<RaycastResult>();
                pointerEventData = new PointerEventData(EventSystem.current);

                // if the pointer actions have no bindings (the default), add some
                if (pointerDownAction == null || pointerDownAction.bindings.Count == 0)
                {
                    if (pointerDownAction == null)
                    {
                        pointerDownAction = new InputAction();
                    }

                    pointerDownAction.AddBinding("<Mouse>/leftButton");
                    pointerDownAction.AddBinding("<Pen>/tip");
                    pointerDownAction.AddBinding("<Touchscreen>/touch*/press");
                    pointerDownAction.AddBinding("<XRController>/trigger");
                }

                if (pointerMoveAction == null || pointerMoveAction.bindings.Count == 0)
                {
                    if (pointerMoveAction == null)
                    {
                        pointerMoveAction = new InputAction();
                    }

                    pointerMoveAction.AddBinding("<Mouse>/position");
                    pointerMoveAction.AddBinding("<Pen>/position");
                    pointerMoveAction.AddBinding("<Touchscreen>/touch*/position");
                }

                pointerDownAction.started += OnPointerDown;
                pointerDownAction.canceled += OnPointerUp;
                pointerDownAction.Enable();
                pointerMoveAction.Enable();
            }

            startPos = myRect.anchoredPosition;

            if (behaviour != OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin)
            {
                return;
            }

            pointerDownPos = startPos;

            var dynamicOrigin = new GameObject(DynamicOriginClickable, typeof(Image));
            dynamicOrigin.transform.SetParent(transform);
            var image = dynamicOrigin.GetComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            var rectTransform = (RectTransform)dynamicOrigin.transform;
            rectTransform.sizeDelta = new Vector2(dynamicOriginRange * 2, dynamicOriginRange * 2);
            rectTransform.localScale = new Vector3(1, 1, 0);
            rectTransform.anchoredPosition3D = Vector3.zero;

            image.sprite = SpriteUtilities.CreateCircleSprite(16, new Color32(255, 255, 255, 255));
            image.alphaHitTestMinimumThreshold = 0.5f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            isInteracting = false;
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.matrix = ((RectTransform)transform.parent).localToWorldMatrix;

            var curtStartPos = ((RectTransform)transform).anchoredPosition;
            if (Application.isPlaying)
            {
                curtStartPos = this.startPos;
            }

            Gizmos.color = new Color32(84, 173, 219, 255);

            var center = curtStartPos;
            if (Application.isPlaying && behaviour == OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin)
            {
                center = pointerDownPos;
            }

            DrawGizmoCircle(center, movementRange);

            if (behaviour != OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin)
            {
                return;
            }

            Gizmos.color = new Color32(158, 84, 219, 255);
            DrawGizmoCircle(curtStartPos, dynamicOriginRange);
        }

        private void BeginInteraction(Vector2 pointerPosition, Camera uiCamera)
        {
            Vector2 screenPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerPosition, uiCamera, out screenPoint);
            switch (behaviour)
            {
                case OnScreenStick.Behaviour.RelativePositionWithStaticOrigin:
                    pointerDownPos = screenPoint;
                    break;
                case OnScreenStick.Behaviour.ExactPositionWithStaticOrigin:
                    pointerDownPos = screenPoint;
                    MoveStick(pointerPosition, uiCamera);
                    break;
                case OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin:
                    pointerDownPos = myRect.anchoredPosition = screenPoint;
                    break;
            }

            SetRectTransformActive(visualWeakBackgroundRect, showVisualElement);
            SetRectTransformActive(visualStrongBackgroundRect, false);
            SetVisualBackgroundPosition(pointerDownPos, false);
            SetVisualKnobPosition(pointerDownPos);
        }

        private void MoveStick(Vector2 pointerPosition, Camera uiCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerPosition, uiCamera, out var position);
            var delta = position - pointerDownPos;

            var knobPos = pointerDownPos;
            if (behaviour == OnScreenStick.Behaviour.ExactPositionWithStaticOrigin)
            {
                delta = position - pointerDownPos;
            }

            if (!allowKnobBeyondMovementRange)
            {
                delta = Vector2.ClampMagnitude(delta, movementRange);
            }

            knobPos = pointerDownPos + delta;

            var normalizePos = new Vector2(delta.x / movementRange, delta.y / movementRange);

            var isStrong = IsStrong(normalizePos);
            SetRectTransformActive(visualWeakBackgroundRect, !isStrong && showVisualElement);
            SetRectTransformActive(visualStrongBackgroundRect, isStrong && showVisualElement);
            SetRectTransformActive(visualKnobRect, showVisualElement);
            SetVisualBackgroundPosition(pointerDownPos, isStrong);
            SetVisualKnobPosition(knobPos);
            SendValueToControl(normalizePos);
        }

        private void EndInteraction()
        {
            SetRectTransformActive(visualWeakBackgroundRect, false);
            SetRectTransformActive(visualStrongBackgroundRect, false);
            SetRectTransformActive(visualKnobRect, false);
            SetVisualBackgroundPosition(startPos, true);
            SetVisualBackgroundPosition(startPos, false);
            SetVisualKnobPosition(startPos);
            pointerDownPos = startPos;

            SendValueToControl(Vector2.zero);
        }

        private void OnPointerDown(InputAction.CallbackContext ctx)
        {
            Debug.Assert(EventSystem.current != null);

            var screenPosition = Vector2.zero;
            if (ctx.control?.device is Pointer pointer)
            {
                screenPosition = pointer.position.ReadValue();
            }

            pointerEventData.position = screenPosition;
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            if (raycastResults.Count == 0)
            {
                return;
            }

            var stickSelected = false;
            foreach (var result in raycastResults)
            {
                if (result.gameObject == gameObject)
                {
                    stickSelected = true;
                    break;
                }
            }

            if (!stickSelected)
            {
                return;
            }

            BeginInteraction(screenPosition, GetCameraFromCanvas());
            pointerMoveAction.performed += OnPointerMove;
        }

        private void OnPointerMove(InputAction.CallbackContext ctx)
        {
            // only pointer devices are allowed
            Debug.Assert(ctx.control?.device is Pointer);

            var screenPosition = ((Pointer)ctx.control.device).position.ReadValue();

            MoveStick(screenPosition, GetCameraFromCanvas());
        }

        private void OnPointerUp(InputAction.CallbackContext ctx)
        {
            EndInteraction();
            pointerMoveAction.performed -= OnPointerMove;
        }

        private Camera GetCameraFromCanvas()
        {
            if (myCanvas == null)
            {
                return null;
            }

            var renderMode = myCanvas.renderMode;
            if (renderMode == RenderMode.ScreenSpaceOverlay
                || (renderMode == RenderMode.ScreenSpaceCamera && myCanvas.worldCamera == null))
            {
                return null;
            }

            return (myCanvas.worldCamera != null) ? myCanvas.worldCamera : Camera.main;
        }

        private bool IsStrong(Vector2 strength)
        {
            return Mathf.Abs(strength.x) >= strengthThreshold || Mathf.Abs(strength.y) >= strengthThreshold;
        }

        private void SetVisualBackgroundPosition(Vector2 screenPosition, bool useStrong)
        {
            var targetRect = useStrong ? visualStrongBackgroundRect : visualWeakBackgroundRect;
            SetRectTransformPosition(targetRect, screenPosition);
        }

        private void SetVisualKnobPosition(Vector2 screenPosition)
        {
            SetRectTransformPosition(visualKnobRect, screenPosition);
        }

        private void SetRectTransformActive(RectTransform rectTransform, bool isActive)
        {
            if (rectTransform == null ||
                rectTransform.gameObject.activeSelf == isActive)
            {
                return;
            }

            rectTransform.gameObject.SetActive(isActive);
        }

        private void SetRectTransformPosition(RectTransform rectTransform, Vector2 position)
        {
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchoredPosition = position;
        }

        private void DrawGizmoCircle(Vector2 center, float radius)
        {
            for (var i = 0; i < 32; ++i)
            {
                var radians = i / 32f * Mathf.PI * 2;
                var nextRadian = (i + 1) / 32f * Mathf.PI * 2;
                Gizmos.DrawLine(
                    new Vector3(center.x + (Mathf.Cos(radians) * radius), center.y + (Mathf.Sin(radians) * radius), 0),
                    new Vector3(center.x + (Mathf.Cos(nextRadian) * radius), center.y + (Mathf.Sin(nextRadian) * radius), 0));
            }
        }

        private bool IsOverOtherUi(PointerEventData eventData)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return false;
            }

            bool result = false;

            eventSystem.RaycastAll(eventData, raycastResultCache);
            int selfIndex = -1;
            for (int i = 0, count = raycastResultCache.Count; i < count; ++i)
            {
                var raycastResult = raycastResultCache[i];

                // Ignore null game objects.
                if (raycastResult.gameObject == null)
                {
                    continue;
                }

                // Cache self gameobject index.
                if (raycastResult.gameObject == this.gameObject)
                {
                    selfIndex = i;
                    continue;
                }

                // If the raycast result is on a UI layer and is after the self,
                // then we are over another UI element.
                if (selfIndex != -1 &&
                    raycastResult.gameObject.layer == uiLayer)
                {
                    result = true;
                    break;
                }
            }

            raycastResultCache.Clear();

            return result;
        }
    }
}
