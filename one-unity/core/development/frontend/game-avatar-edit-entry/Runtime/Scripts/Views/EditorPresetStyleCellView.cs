using DG.Tweening;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPresetStyleCellView : UIView
    {
        [SerializeField]
        private Toggle _selectToggle;
        [SerializeField]
        private Image _targetImage;

        private Sequence _tweener;

        public ToggleGroup Group { get; set; }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<EditorPresetStyleCellView, EditorPresetStyleCellViewModel>();
            bindingSet.Bind(_targetImage).For(v => v.sprite).To(vm => vm.Texture);
            bindingSet.Bind().For(v => OnShowImage).To(vm => vm.ShowImageRequest);
            bindingSet.Bind(_selectToggle).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.IsSelected).TwoWay();
            bindingSet.Bind(_selectToggle).For(v => v.onValueChanged).To(vm => vm.ValueChangedCmd);
            bindingSet.Bind(_selectToggle).For(v => v.group).ToExpression(vm => vm.IsSingleOption ? Group : null);
            bindingSet.Build();

            ShowImage();
            OnValueChanged(_selectToggle.isOn);
        }

        protected override void OnEnable()
        {
            _selectToggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDisable()
        {
            _selectToggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Kill();

            if (this.GetDataContext() is EditorPresetStyleCellViewModel vm)
            {
                vm.Dispose();
            }
        }

        private void OnValueChanged(bool isOn)
        {
            _selectToggle.interactable = !isOn;
        }

        private void OnShowImage(object sender, InteractionEventArgs args)
        {
            ShowImage();
        }

        private void ShowImage()
        {
            if (_targetImage != null && _targetImage.sprite != null)
            {
                _targetImage.enabled = true;
                Tween();
            }
        }

        private void Tween()
        {
            Kill();

            _tweener = DOTween.Sequence();
            _tweener.AppendInterval(0)
                     .Append(_targetImage.DOFade(1f, 0.5f));
        }

        private void Kill()
        {
            if (_tweener != null)
            {
                _tweener.Kill(true);
                _tweener = null;
            }
        }
    }
}