using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Game.Hud;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class SelectPresetAvatarWindow : UI.WindowBase
    {
        [SerializeField]
        private PresetAvatarScrollView _scrollView = default;
        [SerializeField]
        private Button _btnNext;
        [SerializeField]
        private Button _btnBack;

        private SelectPresetAvatarWindowViewModel _viewModel;
        private CancellationTokenSource _cancelLoadTextureTokenSource;
        private ObservableList<PresetAvatarScrollViewCellData> _items;

        public ObservableList<PresetAvatarScrollViewCellData> Items
        {
            get => _items;
            set
            {
                if (_items == value)
                {
                    return;
                }

                _items = value;
                OnItemsChanged(_items);
            }
        }

        private ILogger Logger { get; set; }

        public static string GetWindowAssetPath(IUIConfiguration configuration)
        {
            return $"Prefabs/{configuration.GetRootDirName()}/SelectPresetAvatarWindow.prefab";
        }

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IAvatarEditController controller,
            AvatarEditSettings editSettings)
        {
            Logger = loggerFactory.CreateLogger<SelectPresetAvatarWindow>();
            _viewModel = new SelectPresetAvatarWindowViewModel(
                loggerFactory,
                controller,
                controller.GetPresetAvatarFormatInfos(),
                editSettings);
        }

        public void DismissWindow()
        {
            StopAllCoroutines();

            Dismiss();
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(_viewModel);
            bindingSet.Bind().For(v => v.Items).To(vm => vm.ScrollViewViewModel.Items);
            bindingSet.Bind(_scrollView).For(v => v.CurrentPanelIndex, v => v.OnSelectedChanged).To(vm => vm.ScrollViewViewModel.CurrentIndex).TwoWay();
            bindingSet.Bind(_btnNext).For(v => v.onClick).To(vm => vm.ConfirmCmd);
            bindingSet.Bind(_btnBack).For(v => v.onClick).To(vm => vm.CancelCmd);
            bindingSet.Bind().For(v => v.OnDismissWindow).To(vm => vm.DismissRequest);
            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            if (_cancelLoadTextureTokenSource != null)
            {
                _cancelLoadTextureTokenSource.Cancel();
            }

            StopAllCoroutines();

            _viewModel?.Dispose();

            base.OnDestroy();
        }

        private void OnItemsChanged(IList<PresetAvatarScrollViewCellData> items)
        {
            _scrollView.UpdateData(items);
            _scrollView.SelectCell(0);

            LoadItemsTextures(items).Forget();
        }

        private async UniTaskVoid LoadItemsTextures(IList<PresetAvatarScrollViewCellData> items)
        {
            if (items == null)
            {
                return;
            }

            if (_cancelLoadTextureTokenSource != null)
            {
                _cancelLoadTextureTokenSource.Cancel();
                _cancelLoadTextureTokenSource = null;
            }

            _cancelLoadTextureTokenSource = new CancellationTokenSource();
            var token = _cancelLoadTextureTokenSource.Token;
            try
            {
                Logger.LogDebug($"{nameof(LoadItemsTextures)}: Item Count({items.Count})");
                for (int i = 0; i < items.Count; ++i)
                {
                    if (items[i].HasFirstTextureLoaded == false)
                    {
                        await items[i].LoadAllTexture();
                        token.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (ex is System.OperationCanceledException cancel)
                {
                    Logger.LogDebug($"{nameof(LoadItemsTextures)} is Canceled.");
                }
                else
                {
                    Logger.LogWarning($"{nameof(LoadItemsTextures)} is fail.", ex);
                }
            }
            finally
            {
                _cancelLoadTextureTokenSource.Dispose();
                _cancelLoadTextureTokenSource = null;
            }
        }

        private void OnDismissWindow(object sender, InteractionEventArgs args)
        {
            StopAllCoroutines();

            Dismiss();
        }
    }
}