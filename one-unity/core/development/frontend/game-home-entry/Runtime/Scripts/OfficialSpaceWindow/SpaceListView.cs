using System;
using System.Collections;
using System.Collections.Specialized;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.Home.Entry
{
    public class SpaceListView : UIView
    {
        [SerializeField]
        private GameObject spaceViewPrefab;

        [SerializeField]
        private Transform content;

        private bool isBinded;

        private ObservableList<SpaceViewModel> items = new ObservableList<SpaceViewModel>();

        private SpaceListViewModel viewModel;

        private Loxodon.Framework.Asynchronous.IAsyncResult loadThumbnailAsyncResult;

        public ObservableList<SpaceViewModel> Items
        {
            get => items;
            set
            {
                if (items == value)
                {
                    return;
                }

                if (value == null)
                {
                    throw new NullReferenceException($"{nameof(SpaceListView)}.{nameof(Items)} set value cannot be null");
                }

                items.CollectionChanged -= OnCollectionChanged;

                items = value;

                OnItemChanged();

                items.CollectionChanged += OnCollectionChanged;
            }
        }

        public void Initialize(SpaceListViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.SetDataContext(viewModel);

            if (isBinded)
            {
                return;
            }

            var bindingSet = this.CreateBindingSet<SpaceListView, SpaceListViewModel>();
            bindingSet.Bind().For(v => v.Items).To(vm => vm.Items);
            bindingSet.Bind().For(v => v.LoadThumbnails).To(vm => vm.LoadTextureRequest);
            bindingSet.Bind().For(v => v.ReleaseThumbnails).To(vm => vm.ReleaseTextureRequest);
            bindingSet.Build();

            isBinded = true;
        }

        protected override void OnDestroy()
        {
            viewModel?.Dispose();

            base.OnDestroy();
        }

        private void OnItemChanged()
        {
            ResetItem();

            for (int i = 0; i < items.Count; ++i)
            {
                AddItem(i, items[i]);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var i = 0;
                    foreach (var item in eventArgs.NewItems)
                    {
                        AddItem(eventArgs.NewStartingIndex + i, item);
                        ++i;
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
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
            }
        }

        private void AddItem(int index, object item)
        {
            if (item is not SpaceViewModel viewModel)
            {
                return;
            }

            var itemViewGo = Instantiate(spaceViewPrefab);
            itemViewGo.transform.SetParent(content, false);
            itemViewGo.transform.SetSiblingIndex(index);
            var itemView = itemViewGo.GetComponent<SpaceView>();
            itemView.Initialize(viewModel);
            itemViewGo.SetActive(true);
        }

        private void RemoveItem(int index, object item)
        {
            Transform transform = content.GetChild(index);
            UIView itemView = transform.GetComponent<UIView>();
            if (itemView.GetDataContext() == item)
            {
                itemView.gameObject.SetActive(false);
                Destroy(itemView.gameObject);
            }
        }

        private void ReplaceItem(int index, object oldItem, object item)
        {
            Transform transform = content.GetChild(index);
            UIView itemView = transform.GetComponent<UIView>();
            if (itemView.GetDataContext() == oldItem)
            {
                itemView.SetDataContext(item);
            }
        }

        private void MoveItem(int oldIndex, int index, object item)
        {
            Transform transform = this.content.GetChild(oldIndex);
            UIView itemView = transform.GetComponent<UIView>();
            itemView.transform.SetSiblingIndex(index);
        }

        private void ResetItem()
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Transform transform = content.GetChild(i);
                Destroy(transform.gameObject);
            }
        }

        private void LoadThumbnails(object sender, InteractionEventArgs args)
        {
            if (loadThumbnailAsyncResult != null)
            {
                loadThumbnailAsyncResult.Cancel();
                loadThumbnailAsyncResult = null;
            }

            foreach (var item in Items)
            {
                item.LoadTexture();
            }

            loadThumbnailAsyncResult = Executors.RunOnCoroutine(LoadThumbnailsAsync());
        }

        private IEnumerator LoadThumbnailsAsync()
        {
            foreach (var item in Items)
            {
                item.LoadTexture();
                yield return null;
            }
        }

        private void ReleaseThumbnails(object sender, InteractionEventArgs args)
        {
            foreach (var item in Items)
            {
                item.ReleaseTexture();
            }
        }
    }
}
