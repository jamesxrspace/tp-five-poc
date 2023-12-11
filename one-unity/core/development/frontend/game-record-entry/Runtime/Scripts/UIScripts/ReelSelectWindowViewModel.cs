using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Observables;
using Microsoft.Extensions.Logging;
using TPFive.Model;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public class ReelSelectWindowViewModel : ReelViewModelBase
    {
        private readonly RelayCommand<int> selectFileCommand;
        private readonly RelayCommand joinCommand;
        private readonly ReelManager reelManager;
        private bool interactable = true;
        private bool isJoined;
        private float recordDuration;

        private ObservableList<string> files = new ObservableList<string>();
        private List<RecordData[]> footages = new List<RecordData[]>();
        private OpenApi.GameServer.Model.Reel reel;

        public ReelSelectWindowViewModel(
            ILogger log,
            ReelManager reelManager,
            ReelWindowFlutterMessenger flutterMessenger,
            OpenApi.GameServer.Model.Reel reel)
            : base(log, flutterMessenger)
        {
            this.reelManager = reelManager;
            this.reel = reel;

            selectFileCommand = new RelayCommand<int>(OnSelectFile, IsInteractable);
            joinCommand = new RelayCommand(OnJoin, () => IsInteractable() && !isJoined);
            aigcCommand.Enabled = false;

            RefreshFiles();

            RecordState = RecordStateTypeEnum.Watch;
        }

        public event Action<string/*file path*/> OnFileSelected;

        public bool IsJoined
        {
            get => isJoined;
            set => Set(ref isJoined, value, nameof(IsJoined));
        }

        public float RecordDuration
        {
            get => recordDuration;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"Invalid record duration: {value}");
                }

                Set(ref recordDuration, value, nameof(RecordDuration));
            }
        }

        public ObservableList<string> Files
        {
            get => files;
            set => Set(ref files, value, nameof(Files));
        }

        public ICommand SelectFileCommand => selectFileCommand;

        public ICommand JoinCommand => joinCommand;

        public bool IsInteractable() => interactable;

        protected override bool IsGenerateMusicMotionEnabled() => reel != null && !string.IsNullOrEmpty(reel.MusicToMotionUrl);

        protected override async void OnRecord(bool isOn)
        {
            await flutterMessenger.OnRecord(isOn, OnRecordDurationReached);
            RecordDuration = ReelManager.GetFootageLength(reelManager.Footage);
            UpdateState();
        }

        protected override async void OnMotion()
        {
            await flutterMessenger.OnStartAIGC(reel.MusicToMotionUrl);

            aigcCommand.Enabled = true;
            log.LogDebug("On motion generated.");
        }

        private async void OnSelectFile(int index)
        {
            var sourceFootage = await reelManager.ReadFootageFromFile(files[index]);
            footages.Clear();
            footages.Add(sourceFootage);
            await reelManager.SetupFootage(sourceFootage);
            reelManager.StartSession(SessionStartOption.Watch()).Forget();
            files.Clear();
            RefreshFiles();
            reel = null;
        }

        private void RefreshFiles()
        {
            var targetPath = FileStorageUtility.GetPersitentDataPath("Recordings", true);
            var fileArray = Directory.GetFiles(targetPath, "*.xrs");
            files.AddRange(fileArray);
        }

        private void OnJoin()
        {
            flutterMessenger.OnJoin();
            IsJoined = true;
            UpdateState();
        }

        private void OnRecordDurationReached(bool isEnded)
        {
            RecordDuration = 0;
            flutterMessenger.OnPreview(true).Forget();
            UpdateState();
        }
    }
}
