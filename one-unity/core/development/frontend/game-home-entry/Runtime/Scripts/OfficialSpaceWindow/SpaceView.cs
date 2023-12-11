using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Home.Entry
{
    public class SpaceView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI spaceName;
        [SerializeField]
        private RawImage thumbnail;
        [SerializeField]
        private GameObject thumbnailPlaceholder;
        [SerializeField]
        private Button clickButton;
        [Header("Change Scene Entry Setting")]
        [SerializeField]
        private string homeEntryTitle;
        [SerializeField]
        private string roomEntryTitle;

        private bool isBinded;

        public void Initialize(SpaceViewModel viewModel)
        {
            viewModel.HomeEntryTitle = homeEntryTitle;
            viewModel.RoomEntryTitle = roomEntryTitle;

            this.SetDataContext(viewModel);

            EnsureBinding();
        }

        private void EnsureBinding()
        {
            if (isBinded)
            {
                return;
            }

            var bindingSet = this.CreateBindingSet<SpaceView, SpaceViewModel>();
            bindingSet.Bind(spaceName).For(v => v.text).To(vm => vm.Name);
            bindingSet.Bind(thumbnail).For(v => v.texture).To(vm => vm.ThumbnailTexture);
            bindingSet.Bind(thumbnail.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.ThumbnailTexture != null);
            bindingSet.Bind(thumbnailPlaceholder).For(v => v.activeSelf).ToExpression(vm => vm.ThumbnailTexture == null);
            bindingSet.Bind(clickButton).For(v => v.onClick).To(vm => vm.GoToSpaceCommand);
            bindingSet.Build();

            isBinded = true;
        }
    }
}