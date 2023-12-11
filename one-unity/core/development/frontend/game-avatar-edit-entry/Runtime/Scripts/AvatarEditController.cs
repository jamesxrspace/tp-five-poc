#pragma warning disable SA1305 // Field names should not use Hungarian notation
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TPFive.Game.Hud;
using TPFive.Game.SceneFlow;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;
using VContainer;
using XR.AvatarEditing;
using XR.AvatarEditing.Core;
using XR.AvatarEditing.Model;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using IOnScreenControlService = TPFive.Extended.InputDeviceProvider.OnScreen.IService;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class AvatarEditController : IAvatarEditController, IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IUIConfiguration _uiConfiguration;
        private readonly AppEntrySettings _appEntrySettings;
        private readonly IPublisher<ChangeScene> _pubSceneLoading;
        private readonly LifetimeScope _lifetimeScope;
        private readonly IAvatarApi _avatarApi;
        private readonly UI.IService _uiService;
        private readonly IOnScreenControlService _onScreenControlService;
        private readonly AvatarEditSettings _editSettings;
        private readonly SceneSettings _sceneSettings;

        private AvatarFormatInfo _currentAvatarFormatInfo;
        private XR.AvatarEditing.Core.AvatarEditor _avatarEditor;
        private List<AvatarFormatInfo> _presetAvatarFormatInfos;
        private IAvatarStyleAccessor _styleAccessor;
        private bool _disposed;
        private CancellationTokenSource _uploadCancellationTokenSource;

        [Inject]
        public AvatarEditController(
            ILoggerFactory loggerFactory,
            IUIConfiguration configuration,
            IAvatarApi avatarApi,
            UI.IService uiService,
            IOnScreenControlService onScreenControlService,
            IPublisher<ChangeScene> pubSceneLoading,
            AvatarEditSettings editSettings,
            SceneSettings sceneSettings,
            AppEntrySettings appEntrySettings,
            LifetimeScope lifetimeScope)
        {
            _loggerFactory = loggerFactory;
            _uiConfiguration = configuration;
            _uiService = uiService;
            _onScreenControlService = onScreenControlService;
            _avatarApi = avatarApi;
            _pubSceneLoading = pubSceneLoading;
            _editSettings = editSettings;
            _sceneSettings = sceneSettings;
            _appEntrySettings = appEntrySettings;
            _lifetimeScope = lifetimeScope;
            Logger = GameLoggingUtility.CreateLogger<AvatarEditController>(loggerFactory);
        }

        ~AvatarEditController()
        {
            Dispose(false);
        }

        public IAvatarStyleAccessor AvatarStyleAccessor => _styleAccessor;

        public AvatarFormatInfo CurrentAvatarFormatInfo => _currentAvatarFormatInfo;

        private ILogger Logger { get; set; }

        public void Initialize()
        {
            _onScreenControlService.MoveStickController.Value.IsActive.Value = false;
        }

        public IList<AvatarFormatInfo> GetPresetAvatarFormatInfos()
        {
            if (_presetAvatarFormatInfos == null)
            {
                var store = AvatarFormatStore.Load(_editSettings.PresetAvatarJson);

                _presetAvatarFormatInfos = new List<AvatarFormatInfo>(store
                    .SelectMany(x => x.Value)
                    .Select(x => new AvatarFormatInfo(x.Key, x.Value)));
            }

            return _presetAvatarFormatInfos.AsReadOnly();
        }

        public async UniTaskVoid ShowSelectPresetWindow()
        {
            var path = SelectPresetAvatarWindow.GetWindowAssetPath(_uiConfiguration);
            await _uiService.ShowWindow<SelectPresetAvatarWindow>(path);
        }

        public async UniTaskVoid ShowMainEditWindow()
        {
            var path = AvatarEditMainWindow.GetWindowAssetPath(_uiConfiguration);
            await _uiService.ShowWindow<AvatarEditMainWindow>(path);
        }

        public async UniTaskVoid ShowEditDetailWindow()
        {
            var path = EditAvatarDetailWindow.GetWindowAssetPath(_uiConfiguration);
            await _uiService.ShowWindow<EditAvatarDetailWindow>(path);
        }

        public IReadOnlyDictionary<AvatarEditorPage, AvatarStyleItem> GetStyleItems()
        {
            Dictionary<AvatarEditorPage, AvatarStyleItem> styles = new Dictionary<AvatarEditorPage, AvatarStyleItem>();
            foreach (AvatarEditorPage editorPage in Enum.GetValues(typeof(AvatarEditorPage)))
            {
                string stylePath = ConvertStylePath(editorPage);
                var style = FindAvatarStyle(stylePath);

                if (style != null)
                {
                    styles.Add(editorPage, style);
                }
            }

            return styles;
        }

        public async UniTask<bool> CreatePreviewAavatar(AvatarFormatInfo info)
        {
            bool result = false;

            if (info == null)
            {
                Logger.LogWarning($"{nameof(AvatarEditController)}: avatar format info is null.");
                return result;
            }

            DestoryCurrentAvatar();

            _currentAvatarFormatInfo = info;

            try
            {
                var context = new AvatarCreationContext()
                {
                    AvatarFormat = info.Format,
                    AvatarId = info.Format.avatarid,
                    AvatarType = (AvatarType)info.Format.gender,
                };

                var go = new GameObject("AvatarEditor", typeof(XR.AvatarEditing.Core.AvatarEditor));
                go.transform.SetParent(_sceneSettings.ModelRoot, false);
                _avatarEditor = go.GetComponent<XR.AvatarEditing.Core.AvatarEditor>();

                _avatarEditor.Init(_editSettings.AvatarLayerName);
                await _avatarEditor.Create(context);
                _styleAccessor = new AvatarStyleAccessor(
                    _loggerFactory,
                    _avatarEditor.StylizeEditor,
                    LayerMask.NameToLayer(_editSettings.AvatarLayerName));
                _avatarEditor.Init(_editSettings.AvatarLayerName, _styleAccessor);

                if (_avatarEditor.Avatar.TryGetComponent(out Animator animator))
                {
                    animator.runtimeAnimatorController = _editSettings.GetAnimatorController(context.AvatarType);
                }

                result = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return result;
        }

        public async UniTask<bool> UploadAvatarFormat()
        {
            if (_avatarEditor == null)
            {
                Logger.LogWarning($"{nameof(UploadAvatarFormat)}: avatar editor info is null.");
                return false;
            }

            CancelUploadEditingResult();
            _uploadCancellationTokenSource = new CancellationTokenSource();

            var crossData = new XR.Avatar.CrossData();
            await _avatarEditor.Save(crossData).ToUniTask(cancellationToken: _uploadCancellationTokenSource.Token);
            var editingResult = (AvatarEditingResult)crossData.oValue;

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                try
                {
                    var json = JsonConvert.SerializeObject(editingResult.avatarFormat);
                    Logger.LogDebug("The avatar format for uploading: {AvatarFormat}", json);
                }
                catch (JsonException e)
                {
                    Logger.LogWarning(e, "Exception occured during serializing avatar format");
                    throw;
                }
            }

            var avatarFormatData = JObject.FromObject(editingResult.avatarFormat);
            var response = await _avatarApi.GetMyselfCurrentAvatarMetadataAsync();
            var currentAvatarId = response?.Data?.Avatar?.AvatarId;
            using var assetStream = new FileStream(editingResult.cacheMeshPath, FileMode.Open, FileAccess.Read);
            var profilePicturePath = Path.Combine(editingResult.avatarAssetPath, "FaceSnapshot.png");
            using var profilePictureStream = new FileStream(profilePicturePath, FileMode.Open, FileAccess.Read);
            var halfBodyPicturePath = Path.Combine(editingResult.avatarAssetPath, "FaceSnapshot_Detail.png");
            using var halfBodyPictureStream = new FileStream(halfBodyPicturePath, FileMode.Open, FileAccess.Read);
            var fullBodyPicturePath = Path.Combine(editingResult.avatarAssetPath, "Fullbody.png");
            using var fullBodyPictureStream = new FileStream(fullBodyPicturePath, FileMode.Open, FileAccess.Read);

            var saveResponse = await _avatarApi.SaveAvatarAsync(
                   AvatarModelType.XrV2,
                   avatarFormatData,
                   assetStream,
                   profilePictureStream,
                   halfBodyPictureStream,
                   fullBodyPictureStream,
                   currentAvatarId);

            if (!saveResponse.IsSuccess)
            {
                Logger.LogWarning(
                    "Failed to save avatar. {avatarIdName}:{avatarId} {avatarFormatDataName}:{avatarFormatData}",
                    nameof(currentAvatarId),
                    currentAvatarId,
                    nameof(avatarFormatData),
                    avatarFormatData);
                return false;
            }
            else if (currentAvatarId != null)
            {
                return true;
            }

            var activateResponse = await _avatarApi.ActivateAvatarAsync(saveResponse.Data.Avatar.AvatarId);
            return activateResponse.IsSuccess;
        }

        public void EnableLoadingPanel(bool enable)
        {
            if (_sceneSettings.WaitingPanel != null)
            {
                _sceneSettings.WaitingPanel.transform.SetAsLastSibling();
                _sceneSettings.WaitingPanel.SetActive(enable);
            }
        }

        public void GoToHomeEntry()
        {
            if (!_appEntrySettings.TryGetSceneProperty("HomeEntry", out var homeEntry))
            {
                return;
            }

            if (!_appEntrySettings.TryGetSceneProperty("AvatarEditEntry", out var editorEntry))
            {
                return;
            }

            _pubSceneLoading.Publish(new ChangeScene
            {
                FromCategory = editorEntry.category,
                FromTitle = editorEntry.addressableKey,
                FromCategoryOrder = editorEntry.categoryOrder,
                FromSubOrder = editorEntry.subOrder,
                ToCategory = homeEntry.category,
                ToTitle = homeEntry.addressableKey,
                ToCategoryOrder = homeEntry.categoryOrder,
                ToSubOrder = homeEntry.subOrder,
                LifetimeScope = _lifetimeScope,
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _onScreenControlService.MoveStickController.Value.IsActive.Value = true;
                CancelUploadEditingResult();

                _disposed = true;
            }
        }

        private void DestoryCurrentAvatar()
        {
            if (_avatarEditor != null)
            {
                UnityEngine.Object.Destroy(_avatarEditor.gameObject);
                _avatarEditor = null;
                _currentAvatarFormatInfo = null;
            }
        }

        private void CancelUploadEditingResult()
        {
            if (_uploadCancellationTokenSource == null)
            {
                return;
            }

            _uploadCancellationTokenSource.Cancel();
            _uploadCancellationTokenSource.Dispose();
            _uploadCancellationTokenSource = null;
        }

        private AvatarStyleItem FindAvatarStyle(string stylePath)
        {
            if (string.IsNullOrEmpty(stylePath))
            {
                return null;
            }

            return _avatarEditor != null ? _avatarEditor.AvatarStyleItemStore.Query(stylePath) : null;
        }

        private string ConvertStylePath(AvatarEditorPage page) => page switch
        {
            AvatarEditorPage.Appearances => _editSettings.AppearancesStylePath,
            AvatarEditorPage.Apparels => _editSettings.ApparelsStylePath,
            AvatarEditorPage.Makeup => _editSettings.MakeupStylePath,
            _ => string.Empty,
        };
    }
}
#pragma warning restore SA1305 // Field names should not use Hungarian notation