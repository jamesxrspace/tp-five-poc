using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorColorCellView : UIView
    {
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private Image _targetColorImage;
        [SerializeField]
        private ColorMappingDictionary _colorMappingDictionary = new ColorMappingDictionary();

        private Color _color;

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                var hex = ColorUtility.ToHtmlStringRGB(value);
                if (_colorMappingDictionary.TryGetValue(hex, out var color))
                {
                    _targetColorImage.color = color;
                }
                else
                {
                    _targetColorImage.color = value;
                }

                _color = value;
            }
        }

        public ToggleGroup Group
        {
            get => _toggle != null ? _toggle.group : null;
            set => _toggle.group = value;
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<EditorColorCellView, EditorColorCellViewModel>();
            bindingSet.Bind(this).For(v => v.Color).To(vm => vm.Color);
            bindingSet.Bind(_toggle).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.IsSelected).TwoWay();
            bindingSet.Bind(_toggle).For(v => v.onValueChanged).To(vm => vm.SelectCmd);
            bindingSet.Build();
            ToggleOnValueChanged(_toggle.isOn);
        }

        protected override void OnEnable()
        {
            _toggle.onValueChanged.AddListener(ToggleOnValueChanged);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(ToggleOnValueChanged);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this.GetDataContext() is EditorColorCellViewModel vm)
            {
                vm.Dispose();
            }
        }

        private void ToggleOnValueChanged(bool isOn)
        {
            _toggle.interactable = !isOn;
        }
    }
}