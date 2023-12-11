using UnityEngine;

namespace TPFive.Game.Record
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using VContainer;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly SessionManager sessionManager;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);
            sessionManager = new SessionManager(loggerFactory);
        }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        private ILogger Logger { get; }

        public bool IsPlaying()
        {
            return sessionManager.IsPlaying();
        }

        public RecordResult StartRecord(RecordData[] recordData)
        {
            if (recordData == null)
            {
                return new RecordResult()
                {
                    Error = "StartRecord fail : recordData is null.",
                };
            }

            try
            {
                Logger.LogDebug("StartRecord");
                sessionManager.StartRecord(recordData);
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception:{e}");
                return new RecordResult()
                {
                    Error = $"Exception:{e}",
                };
            }

            return new RecordResult();
        }

        public void StopRecord()
        {
            sessionManager.StopRecord();
        }

        public RecordData[] GetRecordData()
        {
            return sessionManager.GetRecordData();
        }

        public async UniTask<(string xrsFilePath, string audioFilePath)> ExportToFile(RecordData[] recordData)
        {
            string filePath = Path.Combine(FileStorageUtility.BaseDirectory, FileStorageUtility.GetUniqueFileName(FileExtensionType.Xrs, "combine"));
            string audioFilePath = string.Empty;

            IAudioEditor audioEditor = new DefaultAudioEditor();
            var audioRecordData = recordData.OfType<AudioRecordData>();
            foreach (var item in audioRecordData)
            {
                audioEditor.AddTrack(item.AudioSource, volume: item.Volume, label: item.Id, isMainTrack: item.Id == audioRecordData.First().Id);
            }

            if (audioEditor.GetTrackCount() != 0)
            {
                audioFilePath = Path.Combine(FileStorageUtility.BaseDirectory, FileStorageUtility.GetUniqueFileName(FileExtensionType.Wav, "audio"));
                await audioEditor.SaveAsWav(audioFilePath);
            }

            for (int i = 0; i < recordData.Length; i++)
            {
                recordData[i].BeforeSerialize();
            }

            return await UniTask.RunOnThreadPool(() =>
            {
                Logger.LogDebug($"StopRecord. Save file to {filePath}");
                FileCodec.Save(filePath, recordData);

                Logger.LogDebug($"File saved {filePath}");
                return (filePath, audioFilePath);
            });
        }

        public RecordResult StartPlay(RecordData[] recordData, Action<bool> callback)
        {
            if (recordData == null)
            {
                return new RecordResult()
                {
                    Error = "StartPlay fail : recordData is null.",
                };
            }

            try
            {
                Logger.LogDebug("PlayRecording");
                sessionManager.StartPlay(recordData, callback);
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception:{e}");
                callback.Invoke(false);
                return new RecordResult()
                {
                    Error = $"Exception:{e}",
                };
            }

            return new RecordResult();
        }

        public void StopPlay()
        {
            sessionManager.StopPlay();
        }

        public async UniTask<List<RecordData>> LoadRecordAssets(string filePath)
        {
            var result = await UniTask.RunOnThreadPool(() => FileCodec.Load(filePath), true);
            foreach (var data in result)
            {
                data.PostDeserialize();
            }

            return result;
        }

        public async UniTask<List<RecordData>> LoadRecordAssets(Stream stream)
        {
            var result = await UniTask.RunOnThreadPool(() => FileCodec.Load(stream), true);
            foreach (var data in result)
            {
                data.PostDeserialize();
            }

            return result;
        }

        public void StartFilm(int videoWidth, int videoHeight, int frameRate, AudioSource audioSource = default, params Camera[] cameras)
        {
            sessionManager.StartFilm(videoWidth, videoHeight, frameRate, audioSource, cameras);
        }

        public async UniTask<(string thumbnailPath, string filmPath)> StopFilm()
        {
            return await sessionManager.StopFilm();
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                sessionManager?.Dispose();
            }

            _disposed = true;
        }
    }
}