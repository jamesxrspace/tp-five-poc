using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Game.Hud;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class AvatarEditMainWindow : UI.WindowBase
    {
        [SerializeField]
        private RawImage _avatarTexture;
        [SerializeField]
        private Button _btnBack;
        [SerializeField]
        private Button _btnDone;
        [SerializeField]
        private Button _btnResetAvatar;
        [SerializeField]
        private Button _btnEditAvatar;

        private AvatarEditMainWindowViewModel _viewModel;

        private ILogger Logger { get; set; }

        public static string GetWindowAssetPath(IUIConfiguration configuration)
        {
            return $"Prefabs/{configuration.GetRootDirName()}/AvatarEditMainWindow.prefab";
        }

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IAvatarEditController avatarEditorController,
            AvatarEditSettings avatarEditorSettings)
        {
            Logger = loggerFactory.CreateLogger<AvatarEditMainWindow>();
            _viewModel = new AvatarEditMainWindowViewModel(loggerFactory, avatarEditorController, avatarEditorSettings.AvatarRenderTexture);
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(_viewModel);
            bindingSet.Bind(_avatarTexture).For(v => v.texture).To(vm => vm.CurrentTexture);
            bindingSet.Bind(_btnBack).For(v => v.onClick).To(vm => vm.BackCmd);
            bindingSet.Bind(_btnDone).For(v => v.onClick).To(vm => vm.UploadAvatarCmd);
            bindingSet.Bind(_btnResetAvatar).For(v => v.onClick).To(vm => vm.ResetAvatarCmd);
            bindingSet.Bind(_btnEditAvatar).For(v => v.onClick).To(vm => vm.AppearancesCmd);
            bindingSet.Bind().For(v => v.OnDismissWindow).To(vm => vm.DismissRequest);
            bindingSet.Bind().For(v => v.OnRetryToUpload).To(vm => vm.RetryToUploadRequest);

            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            _viewModel?.Dispose();

            base.OnDestroy();
        }

        private void OnDismissWindow(object sender, InteractionEventArgs args)
        {
            Dismiss();
        }

        private void OnRetryToUpload(object sender, InteractionEventArgs args)
        {
            Logger.LogWarning($"{nameof(AvatarEditMainWindow)}: OnRetryToUpload");
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.DisplayDialog(
                "Upload Failure",
                "Retry to upload?",
                "Yes",
                "No"))
            {
                _viewModel.UploadAvatarCmd.Execute(null);
            }
#else
            // TODO: Display a dialog to inquire whether the user wants to retry.
#endif
        }
    }
}