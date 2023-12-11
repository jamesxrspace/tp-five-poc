using System.Collections.Generic;
using System.Collections.Specialized;
using EnhancedUI.EnhancedScroller;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using TPFive.Game.Resource;
using UnityEngine;
using VContainer;

namespace TPFive.Game.Decoration
{
    public sealed class DecorationContentScrollView : UIView, IEnhancedScrollerDelegate
    {
        [SerializeField]
        private DecorationItemGroupView groupPrefab;
        [SerializeField]
        private float cellSize;
        private IObjectResolver objectResolver;
        private ObservableList<List<DecorationItemViewModel>> items;
        private EnhancedScroller scroller;

        public IObjectResolver ObjectResolver
        {
            get => objectResolver;
            set => objectResolver = value;
        }

        public ObservableList<List<DecorationItemViewModel>> Items
        {
            get => items;
            set
            {
                if (items == value)
                {
                    return;
                }

                if (items != null)
                {
                    items.CollectionChanged -= OnCollectionChanged;
                }

                items = value;
                OnItemsChanged();

                if (items != null)
                {
                    items.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return items == null ? 0 : items.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cellView = scroller.GetCellView(groupPrefab) as DecorationItemGroupView;
            if (cellView != null)
            {
                cellView.SetDataContext(items[dataIndex]);
                foreach (var loader in cellView.GetComponentsInChildren<AbstractAssetLoader>())
                {
                    ObjectResolver.Inject(loader);
                }
            }

            return cellView;
        }

        protected override void Awake()
        {
            base.Awake();

            scroller = GetComponent<EnhancedScroller>();
            scroller.Delegate = this;
        }

        private void OnItemsChanged()
        {
            RefreshUI();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (scroller != null)
            {
                scroller.ClearAll();
                scroller.ReloadData();
            }
        }
    }
}
