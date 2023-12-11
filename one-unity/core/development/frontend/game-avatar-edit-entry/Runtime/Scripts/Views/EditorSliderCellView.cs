using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorSliderCellView : UIView
    {
        [FormerlySerializedAs("m_Slider")]
        [SerializeField]
        private Slider _slider;
        [FormerlySerializedAs("m_Value")]
        [SerializeField]
        private TextMeshProUGUI _value;

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<EditorSliderCellView, EditorSliderCellViewModel>();
            bindingSet.Bind(_value).For(v => v.text).ToExpression(vm => vm.SliderValue.ToString());
            bindingSet.Bind(_slider).For(v => v.maxValue).To(vm => vm.SliderMaxValue);
            bindingSet.Bind(_slider).For(v => v.minValue).To(vm => vm.SliderMinValue);
            bindingSet.Bind(_slider).For(v => v.value, v => v.onValueChanged).To(vm => vm.SliderValue).TwoWay();
            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this.GetDataContext() is EditorSliderCellViewModel vm)
            {
                vm.Dispose();
            }
        }
    }
}