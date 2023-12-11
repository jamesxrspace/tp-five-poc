using System;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using UnityEngine;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorColorCellViewModel : ViewModelBase
    {
        private readonly string _styleID;
        private Color _color;
        private bool _selected;
        private SimpleCommand<bool> _selectCmd;

        public EditorColorCellViewModel(string styleID, bool selected, Color color)
        {
            _styleID = styleID;
            _selected = selected;
            _color = color;
            _selectCmd = new SimpleCommand<bool>(OnValueChanged);
        }

        public Action<EditorColorCellViewModel, bool> OnSelected { get; set; }

        public bool IsSelected
        {
            get { return _selected; }
            set { Set(ref _selected, value, nameof(IsSelected)); }
        }

        public Color Color
        {
            get { return _color; }
            set { Set(ref _color, value, nameof(Color)); }
        }

        public string StyleID => _styleID;

        public ICommand SelectCmd => _selectCmd;

        private void OnValueChanged(bool isOn)
        {
            OnSelected?.Invoke(this, isOn);
        }
    }
}