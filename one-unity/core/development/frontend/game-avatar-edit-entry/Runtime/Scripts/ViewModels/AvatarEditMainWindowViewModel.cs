using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using UnityEngine;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class AvatarEditMainWindowViewModel : ViewModelBase
    {
        private readonly IAvatarEditController _avatarEditController;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RenderTexture _renderTexture;
        private readonly RelayCommand _backCmd;
        private readonly RelayCommand _uploadAvatarCmd;
        private readonly RelayCommand _resetAvatarCmd;
        private readonly RelayCommand _appearancesCmd;
        private readonly InteractionRequest _dismissRequest;
        private readonly InteractionRequest _retryToUploadRequest;

        private Texture2D _textureCopy;
        private Texture _currentTexture;
        private bool _disposed = false;
        private bool _interactable = true;

        public AvatarEditMainWindowViewModel(
            ILoggerFactory loggerFactory,
            IAvatarEditController avatarEditController,
            RenderTexture renderTexture)
        {
            Logger = GameLoggingUtility.CreateLogger<SelectPresetAvatarWindowViewModel>(loggerFactory);
            _loggerFactory = loggerFactory;
            _avatarEditController = avatarEditController;
            _renderTexture = renderTexture;
            CurrentTexture = _renderTexture;

            _backCmd = new RelayCommand(OnBackButtonClick, CanInteract);
            _uploadAvatarCmd = new RelayCommand(OnUploadButtonClick, CanInteract);
            _resetAvatarCmd = new RelayCommand(OnResetButtonClick, CanInteract);
            _appearancesCmd = new RelayCommand(AppearancesClick, CanInteract);

            _dismissRequest = new InteractionRequest();
            _retryToUploadRequest = new InteractionRequest();
        }

        public ILoggerFactory LoggerFactory => _loggerFactory;

        public Texture CurrentTexture
        {
            get => _currentTexture;
            set => Set(ref _currentTexture, value, nameof(CurrentTexture));
        }

        public ICommand BackCmd => _backCmd;

        public ICommand UploadAvatarCmd => _uploadAvatarCmd;

        public ICommand ResetAvatarCmd => _resetAvatarCmd;

        public ICommand AppearancesCmd => _appearancesCmd;

        public IInteractionRequest DismissRequest => _dismissRequest;

        public IInteractionRequest RetryToUploadRequest => _retryToUploadRequest;

        private ILogger Logger { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (disposing)
            {
                if (_textureCopy != null)
                {
                    Object.Destroy(_textureCopy);
                }
            }

            _disposed = true;
        }

        private bool CanInteract() => _interactable;

        private void OnBackButtonClick()
        {
            _interactable = false;
            _dismissRequest.Raise();
            _avatarEditController.GoToHomeEntry();
        }

        private void OnUploadButtonClick()
        {
            _interactable = false;
            UploadAvatar().Forget();
        }

        private void OnResetButtonClick()
        {
            _interactable = false;
            _dismissRequest.Raise();
            _avatarEditController.ShowSelectPresetWindow();
        }

        private void AppearancesClick()
        {
            _interactable = false;
            _dismissRequest.Raise();
            _avatarEditController.ShowEditDetailWindow();
        }

        private async UniTask UploadAvatar()
        {
            // Avoid seeing postures for taking pictures
            _textureCopy = CopyRenderTexture(_renderTexture);
            CurrentTexture = _textureCopy;
            await UploadAvatarFormat();
        }

        private async UniTask UploadAvatarFormat()
        {
            bool ok = false;

            _avatarEditController.EnableLoadingPanel(true);

            try
            {
                ok = await _avatarEditController.UploadAvatarFormat();
            }
            catch (System.Exception e)
            {
                Logger.LogError($"{nameof(AvatarEditMainWindowViewModel)} UploadAvatarFormat failed. {e}");
            }
            finally
            {
                _interactable = true;

                _avatarEditController.EnableLoadingPanel(false);

                if (ok)
                {
                    _dismissRequest.Raise();
                    _avatarEditController.GoToHomeEntry();
                }
                else
                {
                    _retryToUploadRequest.Raise();
                }
            }
        }

        private Texture2D CopyRenderTexture(RenderTexture rt)
        {
            var texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            var temp = RenderTexture.active;
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();
            RenderTexture.active = temp;
            return texture;
        }
    }
}