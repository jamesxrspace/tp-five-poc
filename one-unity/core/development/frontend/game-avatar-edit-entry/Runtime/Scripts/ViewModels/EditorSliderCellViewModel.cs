using System;
using Loxodon.Framework.ViewModels;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorSliderCellViewModel : ViewModelBase
    {
        private readonly string _styleID;
        private string _nameTerm;
        private float _sliderValue;
        private Type _valueType;
        private SliderItem _sliderItem;

        public EditorSliderCellViewModel(
            string styleID,
            object currentValue,
            SliderItem sliderItem)
        {
            _styleID = styleID;
            _nameTerm = $"Avatar 2.0/{styleID}";
            _sliderItem = sliderItem;
            _valueType = currentValue.GetType();

            _sliderValue = currentValue is int value ? SettingValueToSliderValue(value) : SettingValueToSliderValue((float)currentValue);
        }

        public string NameTerm
        {
            get { return _nameTerm; }
            set { Set(ref _nameTerm, value, nameof(NameTerm)); }
        }

        public float SliderValue
        {
            get => _sliderValue;

            set
            {
                Set(ref _sliderValue, value, nameof(SliderValue));
                var settingValue = SliderValueToSettingValue(_sliderValue);
                if (_valueType == typeof(float))
                {
                    OnValueChanged?.Invoke(_styleID, settingValue);
                }
                else
                {
                    OnValueChanged?.Invoke(_styleID, (int)settingValue);
                }
            }
        }

        public float SliderMaxValue => _sliderItem.ValueCount;

        public float SliderMinValue => 1;

        public Action<string, object> OnValueChanged { get; set; }

        private float SliderValueToSettingValue(float sliderValue)
        {
            var m = (_sliderItem.Maximum - _sliderItem.Minimum) / (SliderMaxValue - SliderMinValue);
            var settingvalue = (m * (sliderValue - SliderMinValue)) + _sliderItem.Minimum;

            if (_sliderItem.ValueMapping != null)
            {
                foreach (var value in _sliderItem.ValueMapping)
                {
                    if (settingvalue == value.Value)
                    {
                        settingvalue = value.Key;
                        break;
                    }
                }
            }

            return settingvalue;
        }

        private float SettingValueToSliderValue(float settingValue)
        {
            // change value by value mapping
            if (_sliderItem.ValueMapping != null)
            {
                _sliderItem.ValueMapping.TryGetValue(settingValue, out settingValue);
            }

            var m = (_sliderItem.Maximum - _sliderItem.Minimum) / (SliderMaxValue - SliderMinValue);
            var result = ((settingValue - _sliderItem.Minimum) / m) + SliderMinValue;
            return MathF.Round(result, 1);
        }
    }
}
