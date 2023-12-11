using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TPFive.Model;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public abstract class ReelViewModelBase : ViewModelBase
    {
#pragma warning disable SA1401
        protected readonly ILogger log;
        protected readonly ReelWindowFlutterMessenger flutterMessenger;
        protected readonly SimpleCommand setCameraCommand;
        protected readonly SimpleCommand<bool> recordCommand;
        protected readonly SimpleCommand<bool> faceTrackingCommand;
        protected readonly SimpleCommand<bool> bodyTrackingCommand;
        protected readonly SimpleCommand<bool> micCommand;
        protected readonly SimpleCommand<int> cameraTrackCommand;
        protected readonly SimpleCommand nextCommand;
        protected readonly SimpleCommand backToRecordCommand;
        protected readonly SimpleCommand backToFeedCommand;
        protected readonly SimpleCommand endCommand;
        protected readonly RelayCommand motionCommand;
        protected readonly SimpleCommand<bool> aigcCommand;
        protected readonly SimpleCommand clearAIGCCommand;
        protected readonly SimpleCommand cancelUploadCommand;
        protected readonly SimpleCommand<int> joinModeCommand;
        protected readonly SimpleCommand discardCommand;
        protected readonly SimpleCommand draftCommand;
        protected readonly SimpleCommand uploadCommand;
        protected readonly TrackingConfig trackingConfig;

#pragma warning restore SA1401
        private string musicUrl;
        private string descriptionText;
        private int cameraTrackCount;
        private ReelFilePath reelFilePath;
        private int cacheCameraIndex;
        private RecordStateTypeEnum recordState;

        public ReelViewModelBase(
            ILogger log,
            ReelWindowFlutterMessenger flutterMessenger)
        {
            this.log = log;

            this.setCameraCommand = new SimpleCommand(OnSetCamera);
            this.recordCommand = new SimpleCommand<bool>(OnRecord);
            this.faceTrackingCommand = new SimpleCommand<bool>(OnFaceTracking);
            this.bodyTrackingCommand = new SimpleCommand<bool>(OnBodyTracking);
            this.micCommand = new SimpleCommand<bool>(OnMic);
            this.cameraTrackCommand = new SimpleCommand<int>(OnCameraChanged);
            this.nextCommand = new SimpleCommand(OnNext);
            this.backToRecordCommand = new SimpleCommand(OnBackToRecord);
            this.backToFeedCommand = new SimpleCommand(OnBackToFeed);
            this.endCommand = new SimpleCommand(OnEnd);
            this.motionCommand = new RelayCommand(OnMotion, IsGenerateMusicMotionEnabled());
            this.aigcCommand = new SimpleCommand<bool>(OnToggleAIGC);
            this.clearAIGCCommand = new SimpleCommand(OnClearAIGC);
            this.cancelUploadCommand = new SimpleCommand(OnCancelUpload);
            this.joinModeCommand = new SimpleCommand<int>(OnJoinModeChanged);
            this.discardCommand = new SimpleCommand(OnDiscard);
            this.draftCommand = new SimpleCommand(OnDraft);
            this.uploadCommand = new SimpleCommand(OnUpload);
            this.flutterMessenger = flutterMessenger;
            clearAIGCCommand.Enabled = false;
            aigcCommand.Enabled = false;
            this.trackingConfig = new ()
            {
                Face = false,
                UpperBody = false,
                FullBody = false,
            };
            UpdateState();
        }

        public ICommand SetCameraCommand => setCameraCommand;

        public ICommand RecordCommand => recordCommand;

        public ICommand FaceTrackingCommand => faceTrackingCommand;

        public ICommand BodyTrackingCommand => bodyTrackingCommand;

        public ICommand MicCommand => micCommand;

        public ICommand CameraTrackCommand => cameraTrackCommand;

        public ICommand NextCommand => nextCommand;

        public ICommand BackToRecordCommand => backToRecordCommand;

        public ICommand BackToFeedCommand => backToFeedCommand;

        public ICommand EndCommand => endCommand;

        public ICommand MotionCommand => motionCommand;

        public ICommand AIGCCommand => aigcCommand;

        public ICommand ClearAIGCCommand => clearAIGCCommand;

        public ICommand CancelUploadCommand => cancelUploadCommand;

        public ICommand JoinModeCommand => joinModeCommand;

        public ICommand DiscardCommand => discardCommand;

        public ICommand DraftCommand => draftCommand;

        public ICommand UploadCommand => uploadCommand;

        public string DescriptionText
        {
            get => descriptionText;
            set => Set(ref descriptionText, value, nameof(DescriptionText));
        }

        public RecordStateTypeEnum RecordState
        {
            get => recordState;
            set => Set(ref recordState, value, nameof(RecordState));
        }

        public int CameraTrackCount
        {
            get => cameraTrackCount;
            set => Set(ref cameraTrackCount, value, nameof(CameraTrackCount));
        }

        protected virtual bool IsGenerateMusicMotionEnabled() => true;

        protected virtual async void OnRecord(bool isOn)
        {
            await flutterMessenger.OnRecord(isOn);
            if (!isOn)
            {
                flutterMessenger.OnPreview(true).Forget();
            }

            UpdateState();
        }

        protected virtual async void OnMotion()
        {
            await flutterMessenger.OnStartAIGC();

            aigcCommand.Enabled = true;
            log.LogDebug("On motion generated.");
        }

        protected virtual void OnSetCamera()
        {
            flutterMessenger.OnSetCamera();
            UpdateState();
        }

        protected virtual void OnFaceTracking(bool isOn)
        {
            trackingConfig.Face = isOn;

            flutterMessenger.OnUpdateTracking(JsonConvert.SerializeObject(trackingConfig)).Forget();
        }

        protected virtual void OnBodyTracking(bool isOn)
        {
            trackingConfig.UpperBody = isOn;
            flutterMessenger.OnUpdateTracking(JsonConvert.SerializeObject(trackingConfig)).Forget();
        }

        protected virtual void OnMic(bool isOn)
        {
            flutterMessenger.OnMic(isOn);
        }

        protected virtual void OnCameraChanged(int index)
        {
            flutterMessenger.OnPreview(true).Forget();
            flutterMessenger.OnCameraChanged(index);
            cacheCameraIndex = index;
        }

        protected virtual void OnNext()
        {
            flutterMessenger.OnGetCameraTrackCount();

            // ToEdit
            UpdateState();

            // TODO : Set up Avatar(not record avatar), play record
            CameraTrackCount = flutterMessenger.GetCameraTrackCount();
            CameraTrackCommand.Execute(0);
        }

        protected virtual void OnBackToRecord()
        {
            flutterMessenger.OnBackToRecordAsync().ContinueWith(UpdateState).Forget();
        }

        protected virtual void OnBackToFeed()
        {
            flutterMessenger.OnBackToFeed();
        }

        protected virtual async void OnEnd()
        {
            try
            {
                flutterMessenger.OnCameraChanged(cacheCameraIndex);
                (reelFilePath, musicUrl) = await flutterMessenger.OnEnd();
            }
            catch (System.Exception e)
            {
                log.LogError($"OnEnd(): Failed, {e}");
            }

            UpdateState();
        }

        protected virtual void OnToggleAIGC(bool isOn)
        {
            flutterMessenger.ToggleAIGC(isOn).Forget();

            clearAIGCCommand.Enabled = !isOn && flutterMessenger.MusicMotion != null;

            log.LogDebug("PlayAIGC");
        }

        protected virtual void OnClearAIGC()
        {
            flutterMessenger.ClearAIGC();
            aigcCommand.Enabled = false;
            clearAIGCCommand.Enabled = false;
            log.LogDebug("ClearAIGC");
        }

        protected virtual void OnCancelUpload()
        {
            flutterMessenger.OnCancelUpload();
            UpdateState();
        }

        protected virtual void OnJoinModeChanged(int index)
        {
            flutterMessenger.OnJoinModeChanged(index);
        }

        protected virtual void OnDiscard()
        {
            flutterMessenger.OnDiscard();
        }

        protected virtual void OnDraft()
        {
            flutterMessenger.OnDraft();
        }

        protected virtual async void OnUpload()
        {
            var data = new
            {
                title = string.Empty,
                thumbnail = reelFilePath.Thumbnail,
                video = reelFilePath.Video,
                xrs = reelFilePath.Xrs,
                categories = new string[0],
                join_mode = "all",
                description = DescriptionText,
                music_to_motion_url = musicUrl,
            };
            await flutterMessenger.OnUploadReel(JsonConvert.SerializeObject(data));
            flutterMessenger.OnBackToFeed();
        }

        protected void UpdateState()
        {
            RecordState = flutterMessenger.RecordState;
            log.LogDebug($"UpdateState: {RecordState}");
        }
    }
}
