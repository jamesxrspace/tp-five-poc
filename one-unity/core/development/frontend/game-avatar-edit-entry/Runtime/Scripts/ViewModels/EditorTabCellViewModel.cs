using System;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorTabCellViewModel : ViewModelBase
    {
        private string _group;
        private string _name;
        private bool _selected;
        private SimpleCommand<bool> _selectCmd;

        public EditorTabCellViewModel(string group, bool selected)
        {
            _group = group;
            _selected = selected;
            _selectCmd = new SimpleCommand<bool>(OnValueChanged);
        }

        public bool IsSelected
        {
            get { return _selected; }
            set { Set(ref _selected, value, "IsSelected"); }
        }

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value, "Name"); }
        }

        public ICommand SelectCmd => _selectCmd;

        public Action<string, bool> OnSelected { get; set; }

        private void OnValueChanged(bool isOn)
        {
            OnSelected?.Invoke(_group, isOn);
        }
    }
}