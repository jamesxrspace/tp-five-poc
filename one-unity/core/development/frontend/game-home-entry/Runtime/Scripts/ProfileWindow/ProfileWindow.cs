using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using MessagePipe;
using TPFive.Game.App.Entry;
using TPFive.Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class ProfileWindow : WindowBase
    {
        [SerializeField]
        private Button editorAvatarButton;
        [SerializeField]
        private Button mocapButton;
        [SerializeField]
        private Button closeButton;

        private ProfileViewModel viewModel;

        [Inject]
        public void Construct(
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            Settings appEntrySettings,
            Game.Mocap.IService mocapService,
            LifetimeScope lifetimeScope)
        {
            viewModel = new ProfileViewModel(
                pubSceneLoading,
                mocapService,
                appEntrySettings,
                lifetimeScope);
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(editorAvatarButton).For(v => v.onClick).To(vm => vm.EditAvatarCommand);
            bindingSet.Bind(mocapButton).For(v => v.onClick).To(vm => vm.MocapCommand);
            bindingSet.Bind(closeButton).For(v => v.onClick).To(vm => vm.CloseCommand);
            bindingSet.Bind().For(v => v.OnCloseRequest).To(vm => vm.CloseRequest);
            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            viewModel?.Dispose();

            base.OnDestroy();
        }

        private void OnCloseRequest(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }
    }
}
