using System.Collections.Specialized;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorSliderListView : UIView
    {
        [FormerlySerializedAs("m_SliderCellViewTemplate")]
        [SerializeField]
        private EditorSliderCellView _sliderCellViewTemplate;
        [FormerlySerializedAs("m_Content")]
        [SerializeField]
        private Transform _content;

        private ObservableList<EditorSliderCellViewModel> _sliderCells;

        public ObservableList<EditorSliderCellViewModel> SliderCells
        {
            get => _sliderCells;

            set
            {
                if (_sliderCells == value)
                {
                    return;
                }

                if (_sliderCells != null)
                {
                    _sliderCells.CollectionChanged -= OnCollectionChanged;
                    ResetItem();
                }

                _sliderCells = value;

                OnItemsChanged();

                if (_sliderCells != null)
                {
                    _sliderCells.CollectionChanged += OnCollectionChanged;
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
            if (_sliderCells != null)
            {
                for (int i = 0; i < _sliderCells.Count; i++)
                {
                    AddItem(i, _sliderCells[i]);
                }
            }
        }

        private void AddItem(int index, object item)
        {
            var cellView = Instantiate(_sliderCellViewTemplate);
            cellView.transform.SetParent(_content, false);
            cellView.transform.SetSiblingIndex(index);
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
            var cellView = transform.GetComponent<EditorPresetStyleCellView>();
            if (cellView.GetDataContext() == oldItem)
            {
                cellView.SetDataContext(item);
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