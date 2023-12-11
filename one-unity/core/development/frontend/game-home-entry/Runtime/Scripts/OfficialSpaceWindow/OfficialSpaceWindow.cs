using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using TPFive.Game.Hud;
using TPFive.Game.UI;
using TPFive.Room;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class OfficialSpaceWindow : WindowBase
    {
        [SerializeField]
        private SpaceGroupListView spaceGroupListView;
        [SerializeField]
        private Button closeButton;
        private OfficialSpaceViewModel viewModel;

        public static string GetWindowAssetPath(IUIConfiguration configuration)
        {
            return $"Prefabs/{configuration.GetRootDirName()}/OfficialSpaceWindow/OfficialSpaceWindow.prefab";
        }

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            Space.IService spaceService,
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomcmd)
        {
            viewModel = new OfficialSpaceViewModel(
                loggerFactory,
                spaceService,
                pubSceneLoading,
                pubLoadContentLevel,
                appEntrySettings,
                lifetimeScope,
                openRoomcmd);
        }

        protected override void OnCreate(IBundle bundle)
        {
            try
            {
                var bindingSet = this.CreateBindingSet(viewModel);
                bindingSet.Bind(spaceGroupListView).For(v => v.Items).ToExpression(vm => vm.SpaceGroupListViewModel.Items);
                bindingSet.Bind(spaceGroupListView).For(v => v.ReloadData).To(vm => vm.SpaceGroupListViewModel.ReloadDataRequest);
                bindingSet.Bind(closeButton).For(v => v.onClick).To(vm => vm.CloseCommand);
                bindingSet.Bind().For(v => v.OnCloseRequest).To(vm => vm.CloseRequest);
                bindingSet.Build();

                StateChanged += OnStateChanged;
            }
            catch (System.Exception e)
            {
                viewModel.Logger.LogWarning(e, $"{nameof(OfficialSpaceWindow)} create failed.");
            }
        }

        protected override void OnDestroy()
        {
            viewModel?.Dispose();

            StateChanged -= OnStateChanged;

            base.OnDestroy();
        }

        private void OnStateChanged(object sender, WindowStateEventArgs e)
        {
            if (e.State == WindowState.VISIBLE)
            {
                viewModel.SpaceGroupListViewModel.GetSpaceCommand.Execute(null);
            }
        }

        private void OnCloseRequest(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }
    }
}
