using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using TPFive.Game.Extensions;
using UnityEngine.XR.Interaction.Toolkit;
using IResourceService = TPFive.Game.Resource.IService;

namespace TPFive.Game.Decoration
{
    public sealed class DecorationViewModel : ViewModelBase
    {
        private readonly SimpleCommand closeCommand;
        private readonly InteractionRequest closeRequest;
        private readonly XRInteractionManager interactionManager;
        private readonly int categoryCount;
        private readonly int cellCount;
        private readonly int cellCountOfRow;
        private readonly IService decorationService;
        private ObservableList<DecorationCategoryItemViewModel> categoryItemViewModels = new ();
        private ObservableList<List<DecorationItemViewModel>> items = new ();
        private CancellationTokenSource destroyCancellationTokenSource;
        private bool disposed;

        public DecorationViewModel(
            int categoryCount,
            int cellCount,
            int cellCountOfRow,
            IService decorationService)
        {
            this.decorationService = decorationService;
            this.categoryCount = categoryCount;
            this.cellCount = cellCount;
            this.cellCountOfRow = cellCountOfRow;
            closeCommand = new SimpleCommand(OnCloseClicked);
            closeRequest = new InteractionRequest();
            destroyCancellationTokenSource = new CancellationTokenSource();
            InitCategories(destroyCancellationTokenSource.Token).Forget();
        }

        public ObservableList<DecorationCategoryItemViewModel> CategoryItemViewModels => categoryItemViewModels;

        public ObservableList<List<DecorationItemViewModel>> Items => items;

        public ICommand CloseCommand => closeCommand;

        public IInteractionRequest CloseRequest => closeRequest;

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                destroyCancellationTokenSource.Cancel();
                destroyCancellationTokenSource.Dispose();
                CategoryItemViewModels.DisposeAll();
                CategoryItemViewModels.Clear();
                ClearAllItems();
            }

            base.Dispose(disposing);
            disposed = true;
        }

        private async UniTask InitCategories(CancellationToken token)
        {
            var categoryItems = await decorationService.GetCategoryListAsync(categoryCount, 0, token);
            if (categoryItems == null)
            {
                return;
            }

            foreach (var categoryItem in categoryItems)
            {
                CategoryItemViewModels.Add(new DecorationCategoryItemViewModel(categoryItem.Id, categoryItem.TitleI18n, OnCategoryClicked));
            }

            // Select the first category by default
            CategoryItemViewModels.FirstOrDefault()?.ClickCommand.Execute(null);
        }

        private void OnCategoryClicked(DecorationCategoryItemViewModel categoryItemViewModel)
        {
            CancelLoading();
            RefreshDecorationContentAsync(categoryItemViewModel, destroyCancellationTokenSource.Token).Forget();
        }

        private async UniTask RefreshDecorationContentAsync(DecorationCategoryItemViewModel categoryItemViewModel, CancellationToken token)
        {
            foreach (var viewModel in CategoryItemViewModels)
            {
                if (categoryItemViewModel != viewModel)
                {
                    viewModel.IsOn = false;
                }
            }

            ClearAllItems();

            var itemDataList = await decorationService.GetDecorationListAsync(cellCount, 0, categoryItemViewModel.CategoryId, token: token);
            token.ThrowIfCancellationRequested();
            if (itemDataList == null)
            {
                return;
            }

            var row = new List<DecorationItemViewModel>(cellCountOfRow);
            foreach (var decoItemData in itemDataList)
            {
                var cellViewModel = new DecorationItemViewModel(decoItemData.BundleId)
                {
                    IsVisible = true,
                };
                row.Add(cellViewModel);

                if (row.Count == cellCountOfRow)
                {
                    Items.Add(new List<DecorationItemViewModel>(row));
                    row.Clear();
                }
            }

            if (row.Count > 0)
            {
                for (int i = row.Count; i < cellCountOfRow; ++i)
                {
                    var cellViewModel = new DecorationItemViewModel(null)
                    {
                        IsVisible = false,
                    };
                    row.Add(cellViewModel);
                }

                Items.Add(new List<DecorationItemViewModel>(row));
                row.Clear();
            }
        }

        private void CancelLoading()
        {
            if (destroyCancellationTokenSource != null)
            {
                destroyCancellationTokenSource.Cancel();
                destroyCancellationTokenSource.Dispose();
                destroyCancellationTokenSource = new CancellationTokenSource();
            }
        }

        private void ClearAllItems()
        {
            foreach (var list in Items)
            {
                list.DisposeAll();
                list.Clear();
            }

            Items.Clear();
        }

        private void OnCloseClicked()
        {
            closeRequest.Raise();
        }
    }
}
