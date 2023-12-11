using System;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class PresetAvatarScrollContext
    {
        public Action<int> OnCellClicked { get; set; }

        public int SelectedIndex { get; set; } = -1;
    }
}
