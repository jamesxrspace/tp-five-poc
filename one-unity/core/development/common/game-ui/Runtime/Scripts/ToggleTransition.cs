using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPFive.Game.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Toggle))]
    public sealed class ToggleTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Toggle toggle;
        private bool pointerInside;
        [SerializeField]
        private List<SpriteElement> spriteElements;
        [SerializeField]
        private List<GraphicElement> graphicElements;

        private enum State
        {
            Normal,
            Hover,
            Selected,
        }

        private interface ITransitionElement
        {
            void SetState(State state, bool setImmediately = false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerInside = true;
            UpdateState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerInside = false;
            UpdateState();
        }

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(_ => UpdateState());
            UpdateState(true);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateState(true);
        }
#endif

        private void UpdateState(bool setImmediately = false)
        {
            if (toggle == null)
            {
                return;
            }

            if (toggle.isOn)
            {
                ChangeState(State.Selected, setImmediately);
            }
            else if (pointerInside)
            {
                ChangeState(State.Hover, setImmediately);
            }
            else
            {
                ChangeState(State.Normal, setImmediately);
            }
        }

        private void ChangeState(State state, bool setImmediately = false)
        {
            void SetState(ITransitionElement element)
            {
                element?.SetState(state, setImmediately);
            }

            spriteElements?.ForEach(SetState);
            graphicElements?.ForEach(SetState);
        }

        [Serializable]
        private class SpriteElement : ITransitionElement
        {
            [SerializeField]
            private Image targetImage;
            [SerializeField]
            private Sprite normalSprite;
            [SerializeField]
            private Sprite hoverSprite;
            [SerializeField]
            private Sprite selectedSprite;

            public void SetState(State state, bool setImmediately)
            {
                switch (state)
                {
                    case State.Normal:
                        targetImage.overrideSprite = normalSprite;
                        break;
                    case State.Hover:
                        targetImage.overrideSprite = hoverSprite;
                        break;
                    case State.Selected:
                        targetImage.overrideSprite = selectedSprite;
                        break;
                }
            }
        }

        [Serializable]
        private class GraphicElement : ITransitionElement
        {
            [SerializeField]
            private Graphic targetGraphic;
            [SerializeField]
            private float fadeDuration = 0.1f;
            [SerializeField]
            private Color normalColor = Color.white;
            [SerializeField]
            private Color hoverColor = Color.white;
            [SerializeField]
            private Color selectedColor = Color.white;

            public void SetState(State state, bool setImmediately)
            {
                switch (state)
                {
                    case State.Normal:
                        targetGraphic.CrossFadeColor(normalColor, setImmediately ? 0 : fadeDuration, true, true);
                        break;
                    case State.Hover:
                        targetGraphic.CrossFadeColor(hoverColor, setImmediately ? 0 : fadeDuration, true, true);
                        break;
                    case State.Selected:
                        targetGraphic.CrossFadeColor(selectedColor, setImmediately ? 0 : fadeDuration, true, true);
                        break;
                }
            }
        }
    }
}
