using System;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.Home.Entry
{
    public class SpaceGroupListView : UIView, IEnhancedScrollerDelegate
    {
        private const int DefaultViewRowIndex = -1;

        [SerializeField]
        private SpaceGroupCellView cellViewPrefab;

        [SerializeField]
        private EnhancedScroller scroller;

        [SerializeField]
        private float defaultSpaceGroupSize = 128f;

        [Header("Buffer Area")]
        [SerializeField]
        private float bufferSizeOfScrollRect = 0f;

        private bool isInit = false;

        private ObservableList<SpaceGroupCellViewModel> items = new ObservableList<SpaceGroupCellViewModel>();

        private int inViewRowStartIndex = DefaultViewRowIndex;

        private int inViewRowEndIndex = DefaultViewRowIndex;

        private Coroutine delayCheckItemOutOfBufferCoroutine;

        public bool EnhancedScrollerReady => isInit && scroller.gameObject.activeInHierarchy;

        public ObservableList<SpaceGroupCellViewModel> Items
        {
            get => items;
            set
            {
                if (items == value)
                {
                    return;
                }

                items.Clear();

                if (value == null)
                {
                    throw new NullReferenceException($"{nameof(SpaceGroupListView)}.{nameof(Items)} set value cannot be null");
                }

                items = value;
            }
        }

        public void ReloadData(object sender, InteractionEventArgs args)
        {
            float scrollerJumpToNormalizedPosition = 0f;

            if (args?.Context != null)
            {
                scrollerJumpToNormalizedPosition = (float)args.Context;
            }

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(LoadDataJumpToNormalizePos(scrollerJumpToNormalizedPosition, true));
            }
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var data = Items[dataIndex];
            data.SetClickCallback(dataIndex, cellIndex, OnClickItem);
            var cellView = scroller.GetCellView(cellViewPrefab) as SpaceGroupCellView;
            cellView.Initialize(data);
            return cellView;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var item = Items[dataIndex];
            return item.IsExpanded ? item.CellSize : defaultSpaceGroupSize;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Items.Count;
        }

        protected override void Awake()
        {
            scroller.Delegate = this;
            scroller.scrollerScrolled = OnScrollerScrolled;

            isInit = true;
        }

        private IEnumerator LoadDataJumpToNormalizePos(float scrollerJumpToNormalizedPosition, bool resetBuffer)
        {
            // Start loading data

            // wait enhanced scroller finish initialize (getComponents)
            // or it'll show error when call ReloadData
            yield return new WaitUntil(() => EnhancedScrollerReady);

            // calculate postion
            float scrollerJumpToPosition = scrollerJumpToNormalizedPosition * scroller.ScrollSize;

            // enhanced scroller is ready, start to reload data
            InternalReloadData(scrollerJumpToPosition, resetBuffer);
        }

        private void OnScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollPosition)
        {
            // Handle item out of buffer
            if (delayCheckItemOutOfBufferCoroutine != null)
            {
                StopCoroutine(delayCheckItemOutOfBufferCoroutine);
                delayCheckItemOutOfBufferCoroutine = null;
            }

            delayCheckItemOutOfBufferCoroutine = StartCoroutine(DelayCheckItemOutOfBuffer(scroller, scrollPosition));
        }

        private IEnumerator DelayCheckItemOutOfBuffer(EnhancedScroller scroller, float scrollPosition)
        {
            // Wait for scrollRect rect size refreshed
            yield return new WaitForEndOfFrame();
            CheckItemOutOfBuffer(scroller, scrollPosition);
        }

        private void InternalReloadData(float scrollerJumpToPosition, bool resetBuffer)
        {
            // Reload data
            scroller.ReloadData(scrollerJumpToPosition);

            // Only reload whole data needs to reset buffer. Append data list doesn't need to.
            if (resetBuffer)
            {
                inViewRowStartIndex = DefaultViewRowIndex;
                inViewRowEndIndex = DefaultViewRowIndex;
            }

            CheckItemOutOfBuffer(scroller, scroller._scrollPosition);
        }

        private void CheckItemOutOfBuffer(EnhancedScroller scroller, float scrollPosition)
        {
            if (scroller.NumberOfCells == 0)
            {
                return;
            }

            // Avoid check too often
            if (inViewRowStartIndex == scroller.StartDataIndex && inViewRowEndIndex == scroller.EndDataIndex)
            {
                return;
            }

            inViewRowStartIndex = scroller.StartDataIndex;
            inViewRowEndIndex = scroller.EndDataIndex;

            // Calculate out of buffer row index
            float bufferTopPosition = scrollPosition - (scroller.ScrollRectSize * bufferSizeOfScrollRect);
            int bufferTopRowIndex = scroller.GetCellViewIndexAtPosition(bufferTopPosition);

            float bufferButtomPosition = scrollPosition + (scroller.ScrollRectSize * (bufferSizeOfScrollRect + 1));
            int bufferButtomRowIndex = scroller.GetCellViewIndexAtPosition(bufferButtomPosition);

            ItemInBuffer(bufferTopRowIndex, bufferButtomRowIndex);
            ItemsOutOfBuffer(bufferTopRowIndex - 1, bufferButtomRowIndex + 1);
        }

        private void ItemInBuffer(int startRowIndex, int endRowIndex)
        {
            var start = startRowIndex;
            var end = endRowIndex + 1;
            var itemCount = items.Count;
            var size = Mathf.Min(end, itemCount);

            for (int i = start; i < size; i++)
            {
                OnItemInBuffer(i);
            }
        }

        private void OnItemInBuffer(int itemIndex)
        {
            items[itemIndex]?.LoadTexture();
        }

        private void ItemsOutOfBuffer(int topRowIndex, int buttomRowIndex)
        {
            CalculateItemsOutOfBuffer(topRowIndex);
            CalculateItemsOutOfBuffer(buttomRowIndex);
        }

        private void CalculateItemsOutOfBuffer(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= items.Count)
            {
                return;
            }

            OnItemOutOfBuffer(rowIndex);
        }

        private void OnItemOutOfBuffer(int itemIndex)
        {
            items[itemIndex]?.ReleaseTexture();
        }

        private void OnClickItem(int dataIndex, int cellViewIndex)
        {
            // calculate and cache the scroller's position
            var cellPosition = scroller.GetScrollPositionForCellViewIndex(cellViewIndex, EnhancedScroller.CellViewPositionEnum.Before);
            var tweenCellOffset = cellPosition - scroller.ScrollPosition;

            // turn off loop jumping so that the scroller will not try to jump to a new location as the cell is expanding / collapsing
            scroller.IgnoreLoopJump(true);

            // reload the scroller to accommodate the new cell sizes
            scroller.ReloadData();

            // set the scroller's position to focus on the cell
            cellPosition = scroller.GetScrollPositionForCellViewIndex(cellViewIndex, EnhancedScroller.CellViewPositionEnum.Before);
            scroller.SetScrollPositionImmediately(cellPosition - tweenCellOffset);

            // turn loop jumping back on
            scroller.IgnoreLoopJump(false);
        }
    }
}
