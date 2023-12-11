using System;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Game.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Home.Entry
{
    public sealed class MainMenuWindow : WindowBase
    {
        [SerializeField]
        private ItemInfo[] items;

        private IService gameUIService;
        private ILogger logger;
        private MainMenuViewModel viewModel;
        private WindowBase[] windows;

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IService gameUIService)
        {
            this.gameUIService = gameUIService;
            viewModel = new MainMenuViewModel(loggerFactory, items.Length);
            logger = loggerFactory.CreateLogger(nameof(MainMenuWindow));
        }

        protected override void OnCreate(IBundle bundle)
        {
            windows = new WindowBase[items.Length];

            var bindingSet = this.CreateBindingSet(viewModel);
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Toggle.gameObject.SetActive(items[i].IsActive);

                bindingSet.Bind(items[i].Toggle).For(v => v.onValueChanged).To(vm => vm.SelectCommands[i]);
            }

            bindingSet.Bind().For(v => v.OnSelectedRequest).To(vm => vm.SelectedRequest);
            bindingSet.Bind().For(v => v.OnDeselectedRequest).To(vm => vm.DeselectedRequest);
            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            viewModel?.Dispose();

            base.OnDestroy();
        }

        private void OnSelectedRequest(object sender, InteractionEventArgs args)
        {
            var e = (int)args.Context;

            ShowWindow(e).ContinueWith(() =>
            {
                args.Callback?.Invoke();
            });
        }

        private void OnDeselectedRequest(object sender, InteractionEventArgs args)
        {
            var e = (int)args.Context;

            HideWindow(e).Forget();
        }

        private async UniTask ShowWindow(int index)
        {
            var item = items[index];
            try
            {
                var window = await gameUIService.ShowWindow(item.WindowPath);
                window.OnDismissed += (s, e) =>
                {
                    if (item.Toggle != null)
                    {
                        item.Toggle.isOn = false;
                    }
                };
                windows[index] = window;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Show Window failed.");
            }
        }

        private async UniTaskVoid HideWindow(int index)
        {
            var window = windows[index];
            if (window == null || !window.Visibility)
            {
                return;
            }

            await window.Dismiss();
        }

        [Serializable]
        private class ItemInfo
        {
            [SerializeField]
            private Toggle toggle;
            [SerializeField]
            private AssetReferenceGameObject windowPrefabRef;
            [SerializeField]
            private bool disableOnFlutterBuild;

            public Toggle Toggle => toggle;

            public bool IsActive => !GameApp.IsFlutter || !disableOnFlutterBuild;

            public string WindowPath => windowPrefabRef.RuntimeKey as string;
        }
    }
}