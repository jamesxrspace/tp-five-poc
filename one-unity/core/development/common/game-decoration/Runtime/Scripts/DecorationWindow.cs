using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using TPFive.Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace TPFive.Game.Decoration
{
    public sealed class DecorationWindow : WindowBase
    {
        [SerializeField]
        private DecorationCategoryScrollView categoryScrollView;
        [SerializeField]
        private DecorationContentScrollView contentScrollView;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private int categoryCount = 5;
        [SerializeField]
        private int cellCount = 50;
        [SerializeField]
        private int cellCountOfRow = 4;
        private DecorationViewModel _viewModel;

        [Inject]
        public void Construct(IObjectResolver objectResolver, IService decorationService)
        {
            contentScrollView.ObjectResolver = objectResolver;
            _viewModel = new DecorationViewModel(
                categoryCount,
                cellCount,
                cellCountOfRow,
                decorationService);
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(_viewModel);
            bindingSet.Bind(categoryScrollView).For(v => v.CategoryItemViewModels).To(vm => vm.CategoryItemViewModels);
            bindingSet.Bind(contentScrollView).For(v => v.Items).To(vm => vm.Items);
            bindingSet.Bind(closeButton).For(v => v.onClick).To(vm => vm.CloseCommand);
            bindingSet.Bind(this).For(v => v.CloseRequest).To(vm => vm.CloseRequest);
            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            _viewModel?.Dispose();

            base.OnDestroy();
        }

        private void CloseRequest(object sender, InteractionEventArgs e)
        {
            Dismiss();
        }
    }
}
