using System.Collections.Generic;
using EasingCore;
using FancyScrollView;
using UnityEngine;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class PresetAvatarScrollView : FancyScrollView<PresetAvatarScrollViewCellData, PresetAvatarScrollContext>
    {
        [SerializeField]
        private Scroller _scroller = default;
        [SerializeField]
        private GameObject _cellPrefab = default;

        public CellSelectedEvent OnSelectedChanged { get; set; } = new CellSelectedEvent();

        public int CurrentPanelIndex
        {
            get => Context.SelectedIndex;
            set => SelectCell(value);
        }

        protected override GameObject CellPrefab => _cellPrefab;

        public void UpdateData(IList<PresetAvatarScrollViewCellData> items)
        {
            UpdateContents(items);
            _scroller.SetTotalCount(items.Count);
        }

        public void SelectCell(int index)
        {
            if (index < 0 ||
                index >= ItemsSource.Count ||
                index == Context.SelectedIndex)
            {
                return;
            }

            UpdateSelection(index);
            _scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
        }

        protected override void Initialize()
        {
            base.Initialize();

            Context.OnCellClicked = SelectCell;

            _scroller.OnValueChanged(UpdatePosition);
            _scroller.OnSelectionChanged(UpdateSelection);
        }

        private void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
            {
                return;
            }

            if (index >= ItemsSource.Count)
            {
                index = 0;
            }

            Context.SelectedIndex = index;
            OnSelectedChanged?.Invoke(Context.SelectedIndex);
            Refresh();
        }
    }
}
