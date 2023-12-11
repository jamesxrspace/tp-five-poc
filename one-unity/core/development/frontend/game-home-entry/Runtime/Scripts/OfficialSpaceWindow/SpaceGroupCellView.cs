using EnhancedUI.EnhancedScroller;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Home.Entry
{
    public class SpaceGroupCellView : EnhancedScrollerCellView
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI spaceName;

        [SerializeField]
        private TMPro.TextMeshProUGUI description;

        [SerializeField]
        private RawImage thumbnail;

        [SerializeField]
        private RawImage thumbnailPlaceholder;

        [SerializeField]
        private Button clickButton;

        [SerializeField]
        private RectTransform contentRectTransform;

        [SerializeField]
        private SpaceListView spaceListView;

        [SerializeField]
        private float defaultRectSize = 128f;

        private bool isBinded;

        public void Initialize(SpaceGroupCellViewModel data)
        {
            this.SetDataContext(data);

            spaceListView.Initialize(data.SpaceListViewModel);

            EnsureBinding();
        }

        private void EnsureBinding()
        {
            if (isBinded)
            {
                return;
            }

            var bindingSet = this.CreateBindingSet<SpaceGroupCellView, SpaceGroupCellViewModel>();
            bindingSet.Bind(spaceName).For(v => v.text).To(vm => vm.SpaceName);
            bindingSet.Bind(description).For(v => v.text).To(vm => vm.Description);
            bindingSet.Bind(description.gameObject).For(v => v.activeSelf).To(vm => vm.IsExpanded);
            bindingSet.Bind(spaceListView.gameObject).For(v => v.activeSelf).To(vm => vm.IsExpanded);
            bindingSet.Bind(thumbnail).For(v => v.texture).To(vm => vm.ThumbnailTexture);
            bindingSet.Bind(thumbnail.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.ThumbnailTexture != null);
            bindingSet.Bind(thumbnailPlaceholder.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.ThumbnailTexture == null);
            bindingSet.Bind(clickButton).For(v => v.onClick).To(vm => vm.ClickCommand);
            bindingSet.Bind().For(v => v.CalculateCellSize).To(vm => vm.CalculateCellSizeRequest);
            bindingSet.Build();

            isBinded = true;
        }

        private void CalculateCellSize(object sender, InteractionEventArgs args)
        {
            if (this.GetDataContext() is SpaceGroupCellViewModel data)
            {
                if (data.IsExpanded)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
                    var size = contentRectTransform.rect.height;
                    data.SetCellSizeCommand.Execute(size);
                }
                else
                {
                    data.SetCellSizeCommand.Execute(defaultRectSize);
                }
            }
            else
            {
                Debug.LogWarning("SpaceGroupCellView.GetDataContext() is not SpaceGroupCellViewModel");
            }
        }
    }
}
