using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPFive.Game.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Selectable))]
    public class UITransition : UIBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
    {
        private readonly List<CanvasGroup> canvasGroupCache = new List<CanvasGroup>();

        // Graphic that will be colored.
        [SerializeField]
        private Graphic targetGraphic;

        [Tooltip("Can the Selectable be interacted with?")]
        [SerializeField]
        private bool interactable = true;

        // Ignored unity select/deselect events.
        [SerializeField]
        private bool ignoredSelectEvent;

        // Type of the transition that occurs when the button state changes.
        [SerializeField]
        private TransitionType transition = TransitionType.ColorTint;

        // Colors used for a color tint-based transition.
        [SerializeField]
        private ColorBlock colors = ColorBlock.defaultColorBlock;

        // Sprites used for a Image swap-based transition.
        [SerializeField]
        private SpriteState spriteState;

        private bool groupsAllowInteraction = true;

        private bool isPointerInside;
        private bool isPointerDown;
        private bool hasSelection;

        private bool isInteractableOfSelectable;
        private Selectable selectable;

        /// <summary>
        /// Transition mode for a UITransition.
        /// </summary>
        [Flags]
        public enum TransitionType
        {
            /// <summary>
            /// Use an color tint transition.
            /// </summary>
            ColorTint = 1 << 0,

            /// <summary>
            /// Use a sprite swap transition.
            /// </summary>
            SpriteSwap = 1 << 1,
        }

        /// <summary>
        /// An enumeration of selected states of objects.
        /// </summary>
        public enum SelectionState
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled,
        }

        public TransitionType Transition
        {
            get => transition;
            set
            {
                if (transition == value)
                {
                    return;
                }

                transition = value;
                OnSetProperty();
            }
        }

        public ColorBlock Colors
        {
            get => colors;
            set
            {
                if (colors == value)
                {
                    return;
                }

                colors = value;
                OnSetProperty();
            }
        }

        public SpriteState SpriteState
        {
            get => spriteState;
            set
            {
                if (spriteState.Equals(value))
                {
                    return;
                }

                spriteState = value;
                OnSetProperty();
            }
        }

        public bool Interactable
        {
            get => interactable;
            set
            {
                if (interactable == value)
                {
                    return;
                }

                interactable = value;
                OnSetProperty();
            }
        }

        public bool IgnoredSelectEvent
        {
            get => ignoredSelectEvent;
            set
            {
                if (ignoredSelectEvent == value)
                {
                    return;
                }

                ignoredSelectEvent = value;
                OnSetProperty();
            }
        }

        public Graphic TargetGraphic
        {
            get => targetGraphic;
            set
            {
                if (ReferenceEquals(targetGraphic, value))
                {
                    return;
                }

                targetGraphic = value;
                OnSetProperty();
            }
        }

        private SelectionState CurrentSelectionState
        {
            get
            {
                if (!IsInteractable())
                {
                    return SelectionState.Disabled;
                }

                if (isPointerDown)
                {
                    return SelectionState.Pressed;
                }

                if (hasSelection)
                {
                    return SelectionState.Selected;
                }

                if (isPointerInside)
                {
                    return SelectionState.Highlighted;
                }

                return SelectionState.Normal;
            }
        }

        private Selectable Selectable
        {
            get
            {
                if (selectable == null)
                {
                    if (!TryGetComponent<Selectable>(out selectable))
                    {
                        selectable = gameObject.AddComponent<Selectable>();
                        selectable.transition = Selectable.Transition.None;
                        selectable.navigation = new Navigation() { mode = Navigation.Mode.None, };
                    }
                }

                return selectable;
            }
        }

        public bool IsInteractable()
        {
            return groupsAllowInteraction && interactable && isInteractableOfSelectable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void Select()
        {
            hasSelection = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void Deselect()
        {
            hasSelection = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!ignoredSelectEvent)
            {
                Select();
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!ignoredSelectEvent)
            {
                Deselect();
            }
        }

        protected void Update()
        {
            if (isInteractableOfSelectable != Selectable.interactable)
            {
                isInteractableOfSelectable = Selectable.interactable;
                OnSetProperty();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isInteractableOfSelectable = Selectable.interactable;

            // is parent group allows interaction?
            groupsAllowInteraction = ParentGroupAllowsInteraction();

            isPointerDown = false;

            // has been selected?
            if (EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject == gameObject)
            {
                hasSelection = true;
            }

            DoStateTransition(CurrentSelectionState, true);
        }

        protected override void OnDisable()
        {
            InstantClearState();
            base.OnDisable();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            // If our parenting changes figure out if we are under a new CanvasGroup.
            OnCanvasGroupChanged();
        }

        protected override void OnCanvasGroupChanged()
        {
            // Figure out if parent groups allow interaction
            // If no interaction is alowed... then we need
            // to not do that :)
            bool parentGroupAllowsInteraction = ParentGroupAllowsInteraction();
            if (parentGroupAllowsInteraction == groupsAllowInteraction)
            {
                return;
            }

            groupsAllowInteraction = parentGroupAllowsInteraction;
            OnSetProperty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);

            // Need to clear out the override image on the target...
            DoSpriteSwap(null);

            // If the transition mode got changed, we need to clear all the transitions, since we don't know what the old transition mode was.
            StartColorTween(Color.white, true);

            // And now go to the right state.
            DoStateTransition(CurrentSelectionState, true);
        }
#endif // if UNITY_EDITOR

        // Call from unity if animation properties have changed
        protected override void OnDidApplyAnimationProperties()
        {
            OnSetProperty();
        }

        private void OnSetProperty()
        {
            bool instant = false;
#if UNITY_EDITOR
            instant = !Application.isPlaying;
#endif
            DoStateTransition(CurrentSelectionState, instant);
        }

        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            DoStateTransition(CurrentSelectionState, false);
        }

        private void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            var (tintColor, transitionSprite) = state switch
            {
                SelectionState.Selected => (colors.selectedColor, spriteState.selectedSprite),
                SelectionState.Normal => (colors.normalColor, null),
                SelectionState.Highlighted => (colors.highlightedColor, spriteState.highlightedSprite),
                SelectionState.Pressed => (colors.pressedColor, spriteState.pressedSprite),
                SelectionState.Disabled => (colors.disabledColor, spriteState.disabledSprite),
                _ => (Color.black, null),
            };

            if (transition.HasFlag(TransitionType.ColorTint))
            {
                StartColorTween(tintColor * colors.colorMultiplier, instant);
            }

            if (transition.HasFlag(TransitionType.SpriteSwap))
            {
                DoSpriteSwap(transitionSprite);
            }
        }

        private void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
            {
                return;
            }

            float duration = instant ? 0f : colors.fadeDuration;
            targetGraphic.CrossFadeColor(targetColor, duration, true, true);
        }

        private void DoSpriteSwap(Sprite newSprite)
        {
            // Convenience function that converts the referenced Graphic to a Image, if possible.
            var image = targetGraphic as Image;
            if (image == null)
            {
                return;
            }

            image.overrideSprite = newSprite;
        }

        private void InstantClearState()
        {
            isPointerInside = false;
            isPointerDown = false;
            hasSelection = false;

            if (transition.HasFlag(TransitionType.ColorTint))
            {
                StartColorTween(Color.white, true);
            }

            if (transition.HasFlag(TransitionType.SpriteSwap))
            {
                DoSpriteSwap(null);
            }
        }

        /// <summary>
        /// Figure out if parent groups allow interaction or not.
        /// </summary>
        /// <returns>If TRUE means allow interaction, otherwise not.</returns>
        private bool ParentGroupAllowsInteraction()
        {
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(canvasGroupCache);
                foreach (var canvasGroup in canvasGroupCache)
                {
                    if (canvasGroup.enabled && !canvasGroup.interactable)
                    {
                        return false;
                    }

                    if (canvasGroup.ignoreParentGroups)
                    {
                        return true;
                    }
                }

                t = t.parent;
            }

            return true;
        }
    }
}