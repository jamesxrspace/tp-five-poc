using System;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Model;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public abstract class ReelFlutterMessenger : FlutterMessageBase
    {
        private readonly ReelManager reelManager;
        private RecordStateTypeEnum recordState;

        public ReelFlutterMessenger(
            ILogger log,
            ReelManager reelManager,
            ISubscriber<FlutterMessage> subFlutterMessage,
            IPublisher<PostUnityMessage> pubUnityMessage)
            : base(log, subFlutterMessage, pubUnityMessage)
        {
            this.reelManager = reelManager;
            reelManager.OnFaceTrackingStarted += OnFaceTrackingStarted;
            reelManager.OnBodyTrackingStarted += OnBodyTrackingStarted;
            reelManager.OnLossTrackingChanged += OnLossTrackingChanged;
        }

        public RecordStateTypeEnum RecordState
        {
            get => recordState;
            set
            {
                if (recordState == value)
                {
                    return;
                }

                Log.LogDebug($"Set record state: {value}");

                recordState = value;
                SendRecordStateMessage();
            }
        }

        protected ReelManager ReelManager { get => reelManager; }

        public void OnBackToFeed(string sessionId = null)
        {
            // Switch Scene
            try
            {
                reelManager.EndRecordReel();
            }
            catch (Exception e)
            {
                Log.LogError($"Back to feed failed, {e}");
                throw;
            }
        }

        public async UniTaskVoid OnUpdateTracking(string data, string sessionId = null)
        {
            var jsonData = JsonConvert.DeserializeObject<TrackingConfig>(data);
            try
            {
                await ReelManager.SetTrackingMode((bool)jsonData.Face, (bool)jsonData.UpperBody);
                SendUnityMessage("True", sessionId);
            }
            catch (Exception e)
            {
                Log.LogError($"Stop session failed, {e}");
                SendUnityMessage("False", sessionId);
            }
        }

        protected void SendRecordStateMessage()
        {
            SendUnityMessage(data: RecordState.ToJson(), type: UnityMessageTypeEnum.RecordState);
            Log.LogDebug($"Send record state: {RecordState}");
        }

        protected void ResponceReelSceneConfig(string sessionId = null)
        {
            var sceneInfoJson = new ReelSceneConfig()
            {
                InitState = RecordState,
                MotionButtonActive = reelManager.ReelSceneInfo.EnableMusicToMotion,
            }.ToJson();
            SendUnityMessage(data: sceneInfoJson, sessionId: sessionId);
        }

        private void OnFaceTrackingStarted()
        {
            Log.LogDebug("Face tracking started");
            SendTrackingStateMessage(TrackingFlagEnum.Face, TrackingStateTypeEnum.Tracking);
        }

        private void OnBodyTrackingStarted()
        {
            Log.LogDebug("Body tracking started");
            SendTrackingStateMessage(TrackingFlagEnum.UpperBody, TrackingStateTypeEnum.Tracking);
        }

        private void OnLossTrackingChanged(bool isLoss)
        {
            var state = isLoss ? TrackingStateTypeEnum.Detecting : TrackingStateTypeEnum.Tracking;
            SendTrackingStateMessage(TrackingFlagEnum.Face, state);
            SendTrackingStateMessage(TrackingFlagEnum.UpperBody, state);
        }

        private void SendTrackingStateMessage(TrackingFlagEnum flag, TrackingStateTypeEnum state)
        {
            var data = new TrackingState() { Type = flag, State = state }.ToJson();
            SendUnityMessage(data: data, type: UnityMessageTypeEnum.TrackingState);
            Log.LogDebug($"Send tracking state: {data}");
        }
    }
}
