using System.Collections.Specialized;
using EnhancedUI.EnhancedScroller;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using TPFive.UI;
using UnityEngine;

namespace TPFive.Game.Decoration
{
    [RequireComponent(typeof(EnhancedScroller))]
    public sealed class DecorationCategoryScrollView : UIView, IEnhancedScrollerDelegate
    {
        [SerializeField]
        private DecorationCategoryItemView categoryItemViewPrefab;
        [SerializeField]
        private float cellSize;
        private ObservableList<DecorationCategoryItemViewModel> categoryItemViewModels = new ();
        private EnhancedScroller scroller;

        public ObservableList<DecorationCategoryItemViewModel> CategoryItemViewModels
        {
            get => categoryItemViewModels;
            set
            {
                if (categoryItemViewModels == value)
                {
                    return;
                }

                if (categoryItemViewModels != null)
                {
                    categoryItemViewModels.CollectionChanged -= OnCollectionChanged;
                }

                categoryItemViewModels = value;
                OnItemsChanged();

                if (categoryItemViewModels != null)
                {
                    categoryItemViewModels.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return CategoryItemViewModels == null ? 0 : categoryItemViewModels.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var view = scroller.GetCellView(categoryItemViewPrefab) as DecorationCategoryItemView;
            if (view != null)
            {
                view.SetDataContext(categoryItemViewModels[dataIndex]);
            }

            return view;
        }

        protected override void Awake()
        {
            base.Awake();

            scroller = GetComponent<EnhancedScroller>();
            scroller.Delegate = this;
        }

        protected override void Start()
        {
            base.Start();

            var childObject = gameObject.transform.GetChild(0).gameObject;
            childObject.AddComponent<CanvasRenderer>();
            childObject.AddComponent<UIRaycastBlockable>();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (scroller != null)
            {
                scroller.ReloadData();
            }
        }

        private void OnItemsChanged()
        {
            if (scroller != null)
            {
                scroller.ReloadData();
            }
        }
    }
}