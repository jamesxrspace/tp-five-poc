using System;
using System.Collections.Specialized;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPresetStyleListView : UIView
    {
        [FormerlySerializedAs("m_PartStyleCellViewTemplate")]
        [SerializeField]
        private EditorPresetStyleCellView _partStyleCellViewTemplate;
        [FormerlySerializedAs("m_scrollect")]
        [SerializeField]
        private ScrollRect _scrollect;
        [FormerlySerializedAs("m_Content")]
        [SerializeField]
        private RectTransform _content;
        [FormerlySerializedAs("m_ToggleGroup")]
        [SerializeField]
        private ToggleGroup _toggleGroup;

        private string _showIniItemName;
        private Action<int, object> _showIniItemAction;

        private ObservableList<EditorPresetStyleCellViewModel> _presetStyleCells = new ObservableList<EditorPresetStyleCellViewModel>();

        public ObservableList<EditorPresetStyleCellViewModel> PresetStyleCells
        {
            get => _presetStyleCells;

            set
            {
                if (_presetStyleCells == value)
                {
                    return;
                }

                if (_presetStyleCells != null)
                {
                    _presetStyleCells.CollectionChanged -= OnCollectionChanged;
                    ResetItem();
                }

                _presetStyleCells = value;

                if (!string.IsNullOrEmpty(ShowIniItemName) && ShowIniItemName.Length > 0)
                {
                    _showIniItemAction = ShowIniItem;
                }

                OnItemsChanged();

                if (_presetStyleCells != null)
                {
                    _presetStyleCells.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        public string ShowIniItemName
        {
            get => _showIniItemName;
            set
            {
                _showIniItemName = value;
            }
        }

        public void ShowIniItem(int itemIndex, object item)
        {
            if (PresetStyleCells == null)
            {
                return;
            }

            if (itemIndex > PresetStyleCells.Count)
            {
                return;
            }

            if (item is EditorPresetStyleCellViewModel viewModel)
            {
                if (viewModel.StyleName != ShowIniItemName)
                {
                    return;
                }

                Canvas.ForceUpdateCanvases();

                var gridLayoutGroup = _content.GetComponent<GridLayoutGroup>();

                float scrollAmount = gridLayoutGroup.padding.top + ((itemIndex / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
                _content.anchoredPosition =
                        (Vector2)_scrollect.transform.InverseTransformPoint(_content.position)
                        + new Vector2(0, scrollAmount);

                _showIniItemAction = null;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Transform transform = _content.GetChild(i);
                Destroy(transform.gameObject);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItem(eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0], eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetItem();
                    break;
                case NotifyCollectionChangedAction.Move:
                    MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
            }
        }

        private void OnItemsChanged()
        {
            if (_presetStyleCells != null)
            {
                for (int i = 0; i < _presetStyleCells.Count; i++)
                {
                    AddItem(i, _presetStyleCells[i]);
                }
            }
        }

        private void AddItem(int index, object item)
        {
            var cellView = Instantiate(_partStyleCellViewTemplate);
            cellView.transform.SetParent(_content, false);
            cellView.transform.SetSiblingIndex(index);

            cellView.SetDataContext(item);
            cellView.Group = _toggleGroup;

            _showIniItemAction?.Invoke(index, item);
        }

        private void RemoveItem(int index, object item)
        {
            Transform transform = _content.GetChild(index);
            var cellView = transform.GetComponent<UIView>();
            if (cellView.GetDataContext() == item)
            {
                Destroy(cellView.gameObject);
            }
        }

        private void ReplaceItem(int index, object oldItem, object item)
        {
            Transform transform = _content.GetChild(index);
            var cellView = transform.GetComponent<EditorPresetStyleCellView>();
            if (cellView.GetDataContext() == oldItem)
            {
                cellView.SetDataContext(item);
                cellView.Group = _toggleGroup;
            }
        }

        private void MoveItem(int oldIndex, int index, object item)
        {
            Transform transform = _content.GetChild(oldIndex);
            UIView itemView = transform.GetComponent<UIView>();
            itemView.transform.SetSiblingIndex(index);
        }

        private void ResetItem()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Transform transform = _content.GetChild(i);
                Destroy(transform.gameObject);
            }
        }
    }
}