using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TPFive.Game.Record.Entry
{
    public static class MediaFileUtility
    {
        public static async UniTask<AudioClip> CreateClipFromFile(string path, AudioType type)
        {
            using UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, type);
            await uwr.SendWebRequest().ToUniTask();
            return DownloadHandlerAudioClip.GetContent(uwr);
        }
    }
}
