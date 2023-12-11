using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPartListView : UIView
    {
        [FormerlySerializedAs("m_PartCellViewTemplate")]
        [SerializeField]
        private EditorPartCellView _partCellViewTemplate;
        [FormerlySerializedAs("m_Content")]
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private RectTransform _rebuildLayoutRect;
        [FormerlySerializedAs("m_ToggleGroup")]
        [SerializeField]
        private ToggleGroup _toggleGroup;

        private ObservableList<EditorPartCellViewModel> _partCells;

        public ObservableList<EditorPartCellViewModel> PartCells
        {
            get
            {
                return _partCells;
            }

            set
            {
                if (_partCells == value)
                {
                    return;
                }

                if (_partCells != null)
                {
                    _partCells.CollectionChanged -= OnCollectionChanged;
                }

                _partCells = value;

                OnItemsChanged();

                if (_partCells != null)
                {
                    _partCells.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        protected override void OnEnable()
        {
            RebuildLayoutDelay().Forget();
        }

        private async UniTask RebuildLayoutDelay()
        {
            if (_rebuildLayoutRect == null)
            {
                return;
            }

            await UniTask.Yield();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rebuildLayoutRect);
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
            for (int i = 0; i < _partCells.Count; i++)
            {
                AddItem(i, _partCells[i]);
            }
        }

        private void AddItem(int index, object item)
        {
            var cellView = Instantiate(_partCellViewTemplate);
            cellView.transform.SetParent(_content, false);
            cellView.transform.SetSiblingIndex(index);
            cellView.Group = _toggleGroup;
            cellView.SetDataContext(item);
        }

        private void RemoveItem(int index, object item)
        {
            Transform transform = _content.GetChild(index);
            var cellView = transform.GetComponent<EditorPartCellView>();
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
                ((EditorPartCellViewModel)oldItem).Dispose();
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