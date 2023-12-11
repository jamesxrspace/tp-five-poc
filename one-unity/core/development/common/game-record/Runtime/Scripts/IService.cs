using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Record
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        bool IsPlaying();

        // 3D Data recording
        RecordResult StartRecord(RecordData[] recordData);

        void StopRecord();

        RecordData[] GetRecordData();

        UniTask<(string xrsFilePath, string audioFilePath)> ExportToFile(RecordData[] recordData);

        RecordResult StartPlay(RecordData[] recordData, Action<bool> callback);

        void StopPlay();

        UniTask<List<RecordData>> LoadRecordAssets(string filePath);

        UniTask<List<RecordData>> LoadRecordAssets(Stream stream);

        void StartFilm(int videoWidth, int videoHeight, int frameRate, AudioSource audioSource = default, params UnityEngine.Camera[] cameras);

        UniTask<(string thumbnailPath, string filmPath)> StopFilm();
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        bool IsPlaying();

        // 3D Data recording
        RecordResult StartRecord(RecordData[] recordData, bool hasMic, AudioClip bgmClip = default);

        RecordData[] StopRecord();

        UniTask<(string xrsFilePath, string audioFilePath)> ExportToFile(RecordData[] recordData);

        RecordResult StartPlay(RecordData[] recordData, Action<bool> callback);

        void StopPlay();

        UniTask<List<RecordData>> LoadRecordAssets(string filePath);

        void StartFilm(int videoWidth, int videoHeight, int frameRate, AudioSource audioSource = default, params Camera[] cameras);

        UniTask<(string thumbnailPath, string filmPath)> StopFilm();
    }
}