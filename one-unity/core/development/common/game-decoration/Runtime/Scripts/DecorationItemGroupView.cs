using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Game.Decoration
{
    public class DecorationItemGroupView : EnhancedScrollerCellView
    {
        [SerializeField]
        private DecorationItemView itemPrefab;

        public void SetDataContext(List<DecorationItemViewModel> dataList)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < dataList.Count; ++i)
            {
                if (i < childCount)
                {
                    ReplaceItem(i, dataList[i]);
                }
                else
                {
                    AddItem(i, dataList[i]);
                }
            }
        }

        private void ReplaceItem(int index, object item)
        {
            var childTrans = transform.GetChild(index);
            SetItemData(childTrans.GetComponent<UIView>(), item);
        }

        private void AddItem(int index, object item)
        {
            var itemView = Instantiate(itemPrefab, transform, false);
            itemView.transform.SetSiblingIndex(index);
            SetItemData(itemView, item);
        }

        private void SetItemData(UIView view, object data)
        {
            view.SetDataContext(data);
        }
    }
}
