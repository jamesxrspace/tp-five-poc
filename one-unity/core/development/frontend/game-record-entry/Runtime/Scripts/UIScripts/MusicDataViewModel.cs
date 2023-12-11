using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using TPFive.Model;

namespace TPFive.Game.Record.Entry
{
    public class MusicDataViewModel : ViewModelBase
    {
        private readonly MusicData musicData;
        private string thumbnailPath;
        private string songName;
        private string singerName;

        private SimpleCommand<bool> selectCommand;
        private SimpleCommand<bool> playCommand;

        private ReelWindowFlutterMessenger flutterMessenger;

        public MusicDataViewModel(MusicData data, ReelWindowFlutterMessenger flutterMessenger)
        {
            this.SongName = data.SongName;
            this.SingerName = data.SingerName;
            this.ThumbnailPath = data.ThumbnailPath;
            this.selectCommand = new SimpleCommand<bool>(OnSelect);
            this.playCommand = new SimpleCommand<bool>(OnPlay);
            this.flutterMessenger = flutterMessenger;
            this.musicData = data;
        }

        public string ThumbnailPath
        {
            get => thumbnailPath;
            set => Set(ref thumbnailPath, value, nameof(ThumbnailPath));
        }

        public string SongName
        {
            get => songName;
            set => Set(ref songName, value, nameof(SongName));
        }

        public string SingerName
        {
            get => singerName;
            set => Set(ref singerName, value, nameof(SingerName));
        }

        public ICommand PlayCommand => playCommand;

        public ICommand SelectCommand => selectCommand;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void OnSelect(bool isOn)
        {
            flutterMessenger.OnMusic(isOn ? musicData : null);
        }

        private void OnPlay(bool isOn)
        {
            flutterMessenger.OnPlayMusic(isOn ? musicData : null);
        }
    }
}
