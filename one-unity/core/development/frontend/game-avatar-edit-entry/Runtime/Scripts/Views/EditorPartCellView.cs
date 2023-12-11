using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPartCellView : UIView
    {
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private TextMeshProUGUI[] _icon;
        [SerializeField]
        private GameObject _newItemIcon;
        [SerializeField]
        private float _delayShowingTime;

        private EditorPartCellViewModel _viewModel;

        public ToggleGroup Group
        {
            get => _toggle != null ? _toggle.group : null;
            set => _toggle.group = value;
        }

        protected override void Start()
        {
            _viewModel = this.GetDataContext() as EditorPartCellViewModel;
            var bindingSet = this.CreateBindingSet(_viewModel);
            bindingSet.Bind(_icon[0]).For(v => v.text).To(vm => vm.Icon[0]);
            bindingSet.Bind(_icon[1]).For(v => v.text).ToExpression(vm => (vm.Icon.Length > 1) ? vm.Icon[1] : string.Empty);
            bindingSet.Bind(_toggle).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.IsSelected).TwoWay();
            bindingSet.Bind(_toggle).For(v => v.onValueChanged).To(vm => vm.OnSelectedCmd);
            bindingSet.Bind(_newItemIcon).For(v => v.activeSelf).To(vm => vm.HaveNewItem);
            bindingSet.Build();
            ToggleOnValueChanged(_viewModel.IsSelected);
            Invoke(nameof(SetAlpha), _delayShowingTime);
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

            if (this.GetDataContext() is EditorPartCellViewModel vm)
            {
                vm.Dispose();
            }
        }

        private void ToggleOnValueChanged(bool isOn)
        {
            _toggle.interactable = !isOn;
        }

        private void SetAlpha()
        {
            GetComponent<CanvasGroup>().alpha = 1.0f;
        }
    }
}