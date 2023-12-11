using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPresetStylePanelView : UIView
    {
        [SerializeField]
        private EditorColorListView _colorListView;
        [SerializeField]
        private EditorSliderListView _sliderListView;
        [SerializeField]
        private EditorPresetStyleListView _partStyleView;
        [SerializeField]
        private GameObject _loadingUI;

        private bool _created = false;

        public bool Created => _created;

        public void Create()
        {
            var bindingSet = this.CreateBindingSet<EditorPresetStylePanelView, EditorPresetStylePanelViewModel>();
            bindingSet.Bind(_colorListView).For(v => v.ColorCells).To(vm => vm.ColorCells);
            bindingSet.Bind(_colorListView.gameObject).For(v => v.activeSelf).To(vm => vm.HaveColorStyles);
            bindingSet.Bind(_sliderListView).For(v => v.SliderCells).To(vm => vm.SliderCells);
            bindingSet.Bind(_sliderListView.gameObject).For(v => v.activeSelf).To(vm => vm.HaveSliderStyles);
            bindingSet.Bind(_partStyleView).For(v => v.ShowIniItemName).To(vm => vm.AssetId);
            bindingSet.Bind(_partStyleView).For(v => v.PresetStyleCells).To(vm => vm.PresetStyleCells);
            bindingSet.Bind(_partStyleView.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.HavePresetStyles);
            bindingSet.Bind(_loadingUI).For(v => v.activeSelf).ToExpression(vm => !vm.PresetStylesIsReady);
            bindingSet.Build();

            _created = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this.GetDataContext() is EditorPresetStylePanelViewModel vm)
            {
                vm.Dispose();
            }
        }
    }
}