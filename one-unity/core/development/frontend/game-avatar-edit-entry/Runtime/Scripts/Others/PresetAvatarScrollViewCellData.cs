using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit
{
    internal sealed class PresetAvatarScrollViewCellData : IDisposable
    {
        private const int DefaultTextureHandleCount = 2;
        private readonly string _selectedPath;
        private readonly string _idlePath;
        private bool _isTextureLoading = false;
        private bool _disposed = false;
        private List<AsyncOperationHandle<Texture2D>> _textureHandles;

        public PresetAvatarScrollViewCellData(
            int index,
            string idlePath,
            string selectedPath,
            ILoggerFactory loggerFactory)
        {
            Index = index;
            _idlePath = idlePath;
            _selectedPath = selectedPath;
            _textureHandles = new List<AsyncOperationHandle<Texture2D>>(DefaultTextureHandleCount);
            Logger = GameLoggingUtility.CreateLogger<PresetAvatarScrollViewCellData>(loggerFactory);
        }

        ~PresetAvatarScrollViewCellData()
        {
            Dispose(false);
        }

        public event Action OnLoadTextureCompleted;

        public int Index { get; private set; }

        public Texture2D IdleTexture { get; private set; }

        public Texture2D SelectedTexture { get; private set; }

        public bool HasFirstTextureLoaded { get; private set; } = false;

        private ILogger Logger { get; set; }

        public async Task LoadAllTexture()
        {
            if (_isTextureLoading)
            {
                return;
            }

            _isTextureLoading = true;
            var idleTask = LoadTexture(_idlePath);
            var selectedTask = LoadTexture(_selectedPath);

            try
            {
                await Task.WhenAll(idleTask, selectedTask);
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"{nameof(LoadAllTexture)}: Exception.", ex);
            }
            finally
            {
                if (idleTask.Status == TaskStatus.RanToCompletion)
                {
                    IdleTexture = idleTask.Result;
                }

                if (selectedTask.Status == TaskStatus.RanToCompletion)
                {
                    SelectedTexture = selectedTask.Result;
                }

                _isTextureLoading = false;
                HasFirstTextureLoaded = true;
                OnLoadTextureCompleted?.Invoke();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async Task<Texture2D> LoadTexture(string path)
        {
            var handle = Addressables.LoadAssetAsync<Texture2D>(path);
            try
            {
                Texture2D result = await handle.Task;
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
                    _textureHandles?.Add(handle);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"{nameof(LoadTexture)}: Failed to load texture from {path}.", ex);
                Addressables.Release(handle);
            }

            return null;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_textureHandles != null)
                {
                    for (int i = 0; i < _textureHandles.Count; ++i)
                    {
                        Addressables.Release(_textureHandles[i]);
                    }

                    _textureHandles.Clear();
                }
            }

            OnLoadTextureCompleted = null;
            IdleTexture = null;
            SelectedTexture = null;
            _textureHandles = null;

            _disposed = true;
        }
    }
}
