using System.IO;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Observables;
using Newtonsoft.Json;
using TPFive.Model;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public class RecordWindowViewModel : ReelViewModelBase
    {
        private readonly SimpleCommand<bool> musicCommand;

        private ObservableList<MusicDataViewModel> musicDataViewModelList = new ();
        private bool showMusicList;

        public RecordWindowViewModel(
            ILogger log,
            ReelWindowFlutterMessenger flutterMessenger)
            : base(log, flutterMessenger)
        {
            this.musicCommand = new SimpleCommand<bool>(OnMusic);
#if UNITY_EDITOR
            LoadMusicData();
#endif
        }

        public ICommand MusicCommand => musicCommand;

        public bool ShowMusicList
        {
            get => showMusicList;
            set => Set(ref showMusicList, value, nameof(ShowMusicList));
        }

        public ObservableList<MusicDataViewModel> MusicDataViewModelList
        {
            get => musicDataViewModelList;
            set => Set(ref musicDataViewModelList, value, nameof(MusicDataViewModelList));
        }

        protected override void Dispose(bool disposing)
        {
            musicDataViewModelList?.Clear();
            base.Dispose(disposing);
        }

        private void OnMusic(bool isOn)
        {
            ShowMusicList = isOn;
        }

#if UNITY_EDITOR
        private void LoadMusicData()
        {
            string datapath = Application.dataPath;
            string filePath = datapath.Replace(
                "one-unity/unity-project/development/complete-unity/Assets",
                "one-mobile/flutter_project/assets/json/mock_music_list.json");
            var json = File.ReadAllText(filePath);
            var jsonData = JsonConvert.DeserializeObject<MusicData[]>(json);

            var dataMaxCount = Mathf.Min(jsonData.Length, 10);
            for (int i = 0; i < dataMaxCount; i++)
            {
                MusicDataViewModelList.Add(new MusicDataViewModel(jsonData[i], flutterMessenger));
            }
        }
#endif
    }
}