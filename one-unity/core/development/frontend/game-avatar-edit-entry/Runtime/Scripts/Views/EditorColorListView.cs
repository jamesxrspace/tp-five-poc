using System.Collections.Specialized;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorColorListView : UIView
    {
        [SerializeField]
        private EditorColorCellView _colorCellViewTemplate;
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private ToggleGroup _toggleGroup;

        private ObservableList<EditorColorCellViewModel> _colorCells;

        public ObservableList<EditorColorCellViewModel> ColorCells
        {
            get
            {
                return _colorCells;
            }

            set
            {
                if (_colorCells == value)
                {
                    return;
                }

                if (_colorCells != null)
                {
                    _colorCells.CollectionChanged -= OnCollectionChanged;
                    ResetItem();
                }

                _colorCells = value;

                OnItemsChanged();

                if (_colorCells != null)
                {
                    _colorCells.CollectionChanged += OnCollectionChanged;
                }
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
            if (_colorCells != null)
            {
                for (int i = 0; i < _colorCells.Count; i++)
                {
                    AddItem(i, _colorCells[i]);
                }
            }
        }

        private void AddItem(int index, object item)
        {
            var cellView = Instantiate(_colorCellViewTemplate);
            cellView.transform.SetParent(_content, false);
            cellView.transform.SetSiblingIndex(index);
            cellView.Group = _toggleGroup;
            cellView.SetDataContext(item);
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
            UIView itemView = transform.GetComponent<UIView>();
            if (itemView.GetDataContext() == oldItem)
            {
                itemView.SetDataContext(item);
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