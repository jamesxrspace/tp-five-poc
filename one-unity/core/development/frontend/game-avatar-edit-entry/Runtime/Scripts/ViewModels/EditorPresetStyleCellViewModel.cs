using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using XR.Avatar;
using XR.AvatarEditing;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPresetStyleCellViewModel : ViewModelBase
    {
        private SimpleCommand<bool> _valueChangedCmd;
        private SimpleCommand _lockCommand;
        private InteractionRequest _showImageRequest;

        private string _styleID;
        private string _styleName;
        private Sprite _texture;
        private bool _isSingle;
        private bool _selected;
        private ILoggerFactory _loggerFactory;
        private AsyncOperationHandle<Sprite> _spriteHandles;
        private string _unLockTerm;
        private bool _disposed = false;

        public EditorPresetStyleCellViewModel(
            ILoggerFactory loggerFactory,
            AvatarType avatarType,
            Dictionary<string, EditAvatarDetailWindowViewModel.PresetIcon> presetIconHeader,
            bool isSingle,
            string styleID,
            bool selected,
            string styleName)
        {
            _loggerFactory = loggerFactory;
            Logger = _loggerFactory.CreateLogger<EditorPresetStyleCellViewModel>();
            _isSingle = isSingle;
            _styleID = styleID;
            IsSelected = selected;
            _styleName = styleName;

            _valueChangedCmd = new SimpleCommand<bool>(OnValueChanged);
            _ = LoadIcon(avatarType, presetIconHeader, styleName);
        }

        ~EditorPresetStyleCellViewModel()
        {
            Dispose(false);
        }

        public IInteractionRequest ShowImageRequest => _showImageRequest;

        public bool IsSelected
        {
            get => _selected;

            set
            {
                Set(ref _selected, value, "IsSelected");
            }
        }

        public Sprite Texture
        {
            get { return _texture; }
            set { Set(ref _texture, value, "Texture"); }
        }

        public string StyleID => _styleID;

        public string StyleName => _styleName;

        public bool IsSingleOption => _isSingle;

        public ICommand ValueChangedCmd => _valueChangedCmd;

        public Action<EditorPresetStyleCellViewModel, bool> OnSelected { get; set; }

        private ILogger Logger { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_spriteHandles.IsValid())
                {
                    Addressables.Release(_spriteHandles);
                }
            }

            _disposed = true;
        }

        private void OnValueChanged(bool isOn)
        {
            OnSelected?.Invoke(this, isOn);
        }

        private async Task LoadTexture(string path)
        {
            _showImageRequest = new InteractionRequest(this);

            var handle = Addressables.LoadAssetAsync<Sprite>(path);
            try
            {
                Sprite result = await handle.Task;
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    if (handle.OperationException != null)
                    {
                        Logger.LogWarning($"{nameof(LoadTexture)}: Handle failed to load texture from {path}", handle.OperationException);
                    }
                    else
                    {
                        Logger.LogWarning($"{nameof(LoadTexture)}: Handle failed to load texture from {path}");
                    }

                    Addressables.Release(handle);
                }
                else
                {
                    Texture = result;
                    _spriteHandles = handle;
                    ShowImageReq();
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"{nameof(LoadTexture)}: Failed to load texture from {path}.", ex);
                Addressables.Release(handle);
            }
        }

        private async Task LoadIcon(AvatarType avatarType, Dictionary<string,  EditAvatarDetailWindowViewModel.PresetIcon> presetIconHeader, string key)
        {
            try
            {
                string iconKey = key;
                _showImageRequest = new InteractionRequest(this);

                if (presetIconHeader.TryGetValue(_styleID, out var iconStr))
                {
                    var header = string.Empty;
                    if (!string.IsNullOrEmpty(iconStr.Header))
                    {
                        string prefix = avatarType == AvatarType.Female2 ? "f_" : "m_";
                        header = $"{prefix}{iconStr.Header}_";
                    }

                    iconKey = $"{header}{iconKey}";
                }

                iconKey = $"{iconKey}_icon";

                var (tex, err) = await AsyncLoader.LoadAsset<Texture2D>(iconKey);
                if (err == null)
                {
                    Texture = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    ShowImageReq();
                }
                else
                {
                    Logger.LogWarning(err.ToString());
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
            }
        }

        private void ShowImageReq()
        {
            if (_showImageRequest != null)
            {
                _showImageRequest.Raise();
            }
        }
    }
}