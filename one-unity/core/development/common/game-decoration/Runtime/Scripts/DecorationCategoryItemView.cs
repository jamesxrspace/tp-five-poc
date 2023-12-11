using System;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPFive.Game.Decoration
{
    public sealed class DecorationCategoryItemView : EnhancedScrollerCellView, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI categoryNameText;
        [SerializeField]
        private List<GraphicElement> graphicElements;
        [SerializeField]
        private CategoryNameDictionary categoryNameDictionary;
        private string categoryNameId;
        private DecorationCategoryItemViewModel viewModel;
        private bool isOn;
        private int insidePointerNumber;

        private enum State
        {
            Normal,
            Hover,
            Selected,
        }

        public string CategoryNameId
        {
            get => categoryNameId;
            set
            {
                categoryNameId = value;
                if (categoryNameDictionary.TryGetValue(categoryNameId, out var categoryName))
                {
                    categoryNameText.text = categoryName;
                }
                else
                {
                    categoryNameText.text = categoryNameId;
                }
            }
        }

        public void SetDataContext(DecorationCategoryItemViewModel viewModel)
        {
            button.onClick.RemoveAllListeners();

            this.viewModel = viewModel;
            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(this).For(v => v.CategoryNameId).To(vm => vm.CategoryName);
            bindingSet.Bind(this).For(v => v.isOn).To(vm => vm.IsOn);
            bindingSet.Bind(this).For(v => v.insidePointerNumber).To(vm => vm.InsidePointerNumber);
            bindingSet.Bind(button).For(v => v.onClick).To(vm => vm.ClickCommand);
            bindingSet.Bind(this).For(v => v.ClickRequest).To(vm => vm.ClickRequest);
            bindingSet.Build();

            viewModel.InsidePointerNumber = 0;
            UpdateState(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ++viewModel.InsidePointerNumber;
            UpdateState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            --viewModel.InsidePointerNumber;
            UpdateState();
        }

        private void Start()
        {
            var rectTransrform = GetComponentInParent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransrform);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateState(true);
        }
#endif

        private void ClickRequest(object sender, InteractionEventArgs e)
        {
            UpdateState();
        }

        private void UpdateState(bool setImmediately = false)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = !isOn;

            if (isOn)
            {
                ChangeState(State.Selected, setImmediately);
            }
            else if (insidePointerNumber > 0)
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
            void SetState(GraphicElement element)
            {
                element?.SetState(state, setImmediately);
            }

            graphicElements?.ForEach(SetState);
        }

        [Serializable]
        private class GraphicElement
        {
            [SerializeField]
            private Graphic graphic;
            [SerializeField]
            private Color normalColor;
            [SerializeField]
            private Color hoveredColor;
            [SerializeField]
            private Color selectedColor;
            [SerializeField]
            private float fadeDuration = 0.1f;

            public void SetState(State state, bool setImmediately)
            {
                if (graphic == null)
                {
                    return;
                }

                switch (state)
                {
                    case State.Normal:
                        graphic.CrossFadeColor(normalColor, setImmediately ? 0 : fadeDuration, true, true);
                        break;
                    case State.Hover:
                        graphic.CrossFadeColor(hoveredColor, setImmediately ? 0 : fadeDuration, true, true);
                        break;
                    case State.Selected:
                        graphic.CrossFadeColor(selectedColor, setImmediately ? 0 : fadeDuration, true, true);
                        break;
                }
            }
        }

        [Serializable]
        private class CategoryNameDictionary : SerializableDictionaryBase<string, string>
        {
        }
    }
}
