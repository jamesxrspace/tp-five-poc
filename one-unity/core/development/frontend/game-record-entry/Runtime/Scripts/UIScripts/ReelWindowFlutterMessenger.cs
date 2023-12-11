using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.Record.Scene;
using TPFive.Game.Reel;
using TPFive.Model;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public class ReelWindowFlutterMessenger : ReelFlutterMessenger
    {
        private readonly MusicToMotionGenerator musicToMotionGenerator;
        private readonly bool isCreate;
        private string songPath;
        private string aigcPath;
        private bool micIsOn;
        private byte[][] musicMotion;
        private AudioClip bgmClip;
        private ReelFilePath reelFilePath;
        private RecordData[] footage;
        private RecordData[] baseFootage; // cocreate oringinal footage

        public ReelWindowFlutterMessenger(
            ILogger log,
            ReelManager reelManager,
            MusicToMotionGenerator musicToMotionGenerator,
            bool isCreate,
            ISubscriber<FlutterMessage> subFlutterMessage,
            IPublisher<PostUnityMessage> pubUnityMessage)
            : base(log, reelManager, subFlutterMessage, pubUnityMessage)
        {
            this.musicToMotionGenerator = musicToMotionGenerator;
            this.isCreate = isCreate;

            if (isCreate)
            {
                RecordState = reelManager.ReelSceneInfo.EnablePrepareState ? RecordStateTypeEnum.Preset : RecordStateTypeEnum.Standby;
            }
            else
            {
                RecordState = RecordStateTypeEnum.Watch;
            }
        }

        public byte[][] MusicMotion => musicMotion; // for debug

        public void OnSetCamera(string sessionId = null)
        {
            try
            {
                ReelManager.SetupState(ReelState.Standby);
                RecordState = RecordStateTypeEnum.Standby;
            }
            catch (Exception e)
            {
                Log.LogError($"Set camera failed, {e}");

                SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                throw;
            }

            SendUnityMessage(sessionId: sessionId);
        }

        public async UniTask OnRecord(bool isOn, Action<bool> onDurationReached = null, string sessionId = null)
        {
            async void OnDurationReachedInternal(bool isEnded)
            {
                await OnRecordDurationReached(isEnded);
                onDurationReached?.Invoke(isEnded);
            }

            if (isOn)
            {
                try
                {
                    if (ReelManager.IsSessionRunning)
                    {
                        await ReelManager.StopSession(SessionEndOption.Drop());
                    }

                    if (!isCreate)
                    {
                        baseFootage = ReelManager.Footage;
                    }

                    var option = new SessionStartOption()
                    {
                        State = ReelState.Recording,
                        JoinUserAvatar = true,
                        EnableRecordMotion = true,
                        EnableRecordVoice = micIsOn,
                        BgmClip = bgmClip,
                        MusicMotion = musicMotion, // TODO : cocreate can't get music url, so can't generate music motion
                        PlaybackFinishedHandler = isCreate ? null : OnDurationReachedInternal,
                    };

                    await ReelManager.StartSession(option);
                    Log.LogDebug("Start record");
                }
                catch (Exception e)
                {
                    Log.LogError($"Start record failed, {e}");

                    SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                    throw;
                }

                SendUnityMessage(sessionId: sessionId);
                RecordState = RecordStateTypeEnum.Recording;
            }
            else
            {
                try
                {
                    var endOption = SessionEndOption.SaveAndExport();
                    reelFilePath = new ()
                    {
                        Xrs = await ReelManager.StopSession(endOption),
                        Thumbnail = string.Empty,
                        Video = string.Empty,
                        Audio = string.Empty,
                    };
                    footage = endOption.Footage;
                    await ReelManager.SetupFootage(footage);
                    await ReelManager.SetTrackingMode(false, false);
                    ReelManager.BuildDirector();

                    SendUnityMessage(reelFilePath.ToJson(), sessionId: sessionId);
                    Log.LogDebug("Stop record");
                }
                catch (Exception e)
                {
                    Log.LogError($"Stop record failed, {e}");

                    SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                    throw;
                }

                RecordState = RecordStateTypeEnum.Preview;
            }
        }

        public async void OnMusic(MusicData data, string sessionId = null)
        {
            try
            {
                if (data != null && string.IsNullOrEmpty(data.SongPath))
                {
                    throw new Exception($"{data.SongName} : SongPath can not be null");
                }

                songPath = data?.SongPath;
                aigcPath = data?.AigcPath ?? string.Empty;
                bgmClip = songPath != null ? (await Resources.LoadAsync<AudioClip>(songPath)) as AudioClip : null;
                SendUnityMessage(sessionId: sessionId);
            }
            catch (Exception e)
            {
                Log.LogError($"Turn on music failed, {e}");
                SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                throw;
            }
        }

        public void OnMic(bool isOn, string sessionId = null)
        {
            micIsOn = isOn; // isOn -> Flutter change by message, unity by ui toggle state.

            SendUnityMessage(micIsOn.ToString(), sessionId: sessionId);
        }

        public void OnCameraChanged(int index, string sessionId = null)
        {
            ReelManager.PlaySelectedCameraTrack(index);
        }

        public async UniTask OnStartAIGC(string musicUrl = null, string sessionId = null)
        {
            try
            {
                aigcPath = musicUrl ?? aigcPath;
                musicMotion = await GetMusicMotion(aigcPath);
            }
            catch (Exception e)
            {
                Log.LogError($"OnStartAIGC failed, {e}");
                SendUnityMessage(sessionId: sessionId, type: UnityMessageTypeEnum.GeneratedAigc, errorCode: ErrorCodeEnum.Fail);
                throw;
            }

            SendUnityMessage(sessionId: sessionId, type: UnityMessageTypeEnum.GeneratedAigc);

            Log.LogDebug($"OnStartAIGC Success, send message to flutter");
        }

        public async UniTask ToggleAIGC(bool isOn, string sessionId = null)
        {
            // this is for debug not trigger flutter.
            if (musicMotion == null)
            {
                SendUnityMessage(
                    type: UnityMessageTypeEnum.ShowToast,
                    errorCode: ErrorCodeEnum.Fail,
                    errorMsg: "ToggleMusicMotion failed, music motion is null");
                Log.LogError($"PlayAIGC failed, aigcMotion is null");
                return;
            }

            if (isOn)
            {
                var startOption = new SessionStartOption
                {
                    State = RecordState switch
                    {
                        RecordStateTypeEnum.Preset => ReelState.Prepare,
                        RecordStateTypeEnum.Standby => ReelState.Standby,
                        RecordStateTypeEnum.Recording => ReelState.Recording,
                        _ => throw new ArgumentOutOfRangeException(),
                    },
                    JoinUserAvatar = true,
                    BgmClip = bgmClip,
                    MusicMotion = musicMotion,
                };

                await ReelManager.StartSession(startOption); // preview but not record
            }
            else
            {
                await ReelManager.StopSession(SessionEndOption.Drop());
            }
        }

        public void ClearAIGC()
        {
            musicMotion = null;
        }

        public async void OnPlayMusic(MusicData data, string sessionId = null)
        {
            try
            {
                if (data != null)
                {
                    ReelManager.PlayBGM((await Resources.LoadAsync<AudioClip>(data.SongPath)) as AudioClip);
                }
                else
                {
                    ReelManager.StopBGM();
                }
            }
            catch (Exception e)
            {
                Log.LogError($"Play music failed, {e}");
                SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                throw;
            }

            SendUnityMessage(sessionId: sessionId);
        }

        public async UniTask OnBackToRecordAsync(string sessionId = null)
        {
            await ReelManager.DiscardRunningSession();

            // BackRecord
            ResetToBaseFootage();

            await ReelManager.SetupFootage(footage);
            ReelManager.JoinUser();
            ReelManager.SetupState(ReelState.Standby);
            RecordState = RecordStateTypeEnum.Standby;
            SendUnityMessage(sessionId: sessionId);
        }

        public async UniTask OnBackToPreviewAsync(string sessionId = null)
        {
            await ReelManager.DiscardRunningSession();

            SendUnityMessage(sessionId: sessionId);
            ReelManager.SetupState(ReelState.Preview);
            RecordState = RecordStateTypeEnum.Preview;
        }

        public void OnJoin(string sessionId = null)
        {
            try
            {
                ReelManager.StopSession(SessionEndOption.Drop()).Forget();
                ReelManager.JoinUser();
                var state = ReelManager.ReelSceneInfo.EnablePrepareState ? ReelState.Prepare : ReelState.Standby;
                ReelManager.SetupState(state);
                RecordState = ReelManager.ReelSceneInfo.EnablePrepareState ? RecordStateTypeEnum.Preset : RecordStateTypeEnum.Standby;
            }
            catch (Exception e)
            {
                Log.LogError("Join failed, {exception}", e);
                throw;
            }

            SendUnityMessage(sessionId: sessionId);
        }

        public async UniTask<(ReelFilePath filePath, string musicUrl)> OnEnd(string sessionId = null)
        {
            await OnFilm();
            RecordState = RecordStateTypeEnum.Upload;

            return (reelFilePath, aigcPath);
        }

        public int GetCameraTrackCount()
        {
            return ReelManager.GetCameraTrackCount();
        }

        public void OnCancelUpload()
        {
            // back
            RecordState = RecordStateTypeEnum.Done;
        }

        public void OnJoinModeChanged(int index)
        {
            // change join mode
        }

        public void OnDiscard()
        {
            // reelmanager discard
        }

        public void OnDraft()
        {
            // Save draft to server
        }

        public async UniTask OnPreview(bool isOn, string sessionId = null)
        {
            if (isOn)
            {
                try
                {
                    await Replay();
                    Log.LogDebug("Start preview");
                }
                catch (Exception e)
                {
                    Log.LogError($"Start preview failed, {e}");

                    SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                    throw;
                }

                SendUnityMessage(sessionId: sessionId);
            }
            else
            {
                // TBD: pause preview havn't implement yet.
                Log.LogDebug("Should pause preview");
                SendUnityMessage(sessionId: sessionId);
            }
        }

        public void OnSetCameraTrack(string data, string sessionId = null)
        {
            var jsonData = JObject.Parse(data);

            var index = jsonData["track"].ToObject<int>();
            OnCameraChanged(index, sessionId);
        }

        public async UniTask OnFilm(string sessionId = null)
        {
            try
            {
                await ReelManager.DiscardRunningSession();

                Log.LogDebug("Start film");
                var (thumbnailPath, filmPath) = await ReelManager.CreateFilm(footage, CancellationToken.None);

                reelFilePath.Thumbnail = thumbnailPath;
                reelFilePath.Video = filmPath;

                SendUnityMessage(reelFilePath.ToJson(), sessionId);
            }
            catch (Exception e)
            {
                Log.LogError($"Start film failed, {e}");

                SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail);
                throw;
            }
        }

        public async UniTask OnUploadReel(string data, string sessionId = null)
        {
            try
            {
                var reelData = JsonConvert.DeserializeObject<CreateReelRequest>(data);
                var createReel = new CreateReelData()
                {
                    Description = reelData.Description,
                    XrsPath = reelData.Xrs,
                    ThumbnailPath = reelData.Thumbnail,
                    VideoPath = reelData.Video,
                    Type = string.Empty,
                    JoinMode = reelData.JoinMode,
                    Categories = new List<CategoriesEnum>() { CategoriesEnum.Music, CategoriesEnum.Friends },
                    Tags = new List<string>() { "reel", "tag" },
                    MusicToMotionUrl = aigcPath ?? reelData.MusicToMotionUrl,
                };

                var result = await ReelManager.CreateAndPostReel(createReel);
                if (!result)
                {
                    throw new Exception("Create and post reel failed");
                }

                SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Success);
            }
            catch (Exception e)
            {
                Log.LogError("upload reel failed, {error}", e);
                SendUnityMessage(sessionId: sessionId, errorCode: ErrorCodeEnum.Fail, errorMsg: e.Message);
                throw;
            }
        }

        public void OnGetCameraTrackCount(string sessionId = null)
        {
            SendUnityMessage(GetCameraTrackCount().ToString(), sessionId);
            RecordState = RecordStateTypeEnum.Done;
        }

        protected override void OnFlutterMessage(FlutterMessage flutterMessage)
        {
            Log.LogDebug(flutterMessage.Data);
            switch (flutterMessage.Type)
            {
                case FlutterMessageTypeEnum.StartRecord:
                    OnRecord(true, onDurationReached: null, sessionId: flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.StopRecord:
                    OnRecord(false, sessionId: flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.SetCamera:
                    OnSetCamera(flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.RequestToSocialLobbyPage:
                    OnBackToFeed(flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.SetMic:
                    OnMic(!micIsOn, flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.SetMusic:
                    var data = !string.IsNullOrEmpty(flutterMessage.Data) ? MusicData.FromJson(flutterMessage.Data) : null;
                    OnMusic(data, flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.ResetRecord:
                    OnBackToRecordAsync(flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.RequestToPreview:
                    OnBackToPreviewAsync(flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.StartAigc:
                    OnStartAIGC(flutterMessage.Data, flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.PlayMusic:
                    var musicData = !string.IsNullOrEmpty(flutterMessage.Data) ? MusicData.FromJson(flutterMessage.Data) : null;
                    OnPlayMusic(musicData, flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.StartPreview:
                    OnPreview(true, flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.StopPreview:
                    OnPreview(false, flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.SelectTrack:
                    OnSetCameraTrack(flutterMessage.Data, flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.StartFilm:
                    OnFilm(flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.UploadReel:
                    OnUploadReel(flutterMessage.Data, flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.ToggleTracking:
                    OnUpdateTracking(flutterMessage.Data, flutterMessage.SessionId).Forget();
                    break;
                case FlutterMessageTypeEnum.GetTrackCount:
                    OnGetCameraTrackCount(flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.RequestReelSceneConfig:
                    ResponceReelSceneConfig(flutterMessage.SessionId);
                    break;
                case FlutterMessageTypeEnum.CocreateJoin:
                    OnJoin(flutterMessage.SessionId);
                    break;
                default:
                    Log.LogError("No excute FlutterMessageTypeEnum type = {Type}", flutterMessage.Type);
                    break;
            }

            Log.LogDebug($"Recived flutterMessage, Type : {flutterMessage.Type}, Data : {flutterMessage.Data} , SessionId : {flutterMessage.SessionId}");
        }

        private async UniTask Replay()
        {
            await ReelManager.DiscardRunningSession();

            await ReelManager.SetupFootage(footage);
            var startOption = new SessionStartOption
            {
                State = RecordState == RecordStateTypeEnum.Preview ? ReelState.Preview : ReelState.Watch,
                PlaybackFinishedHandler = (r) =>
                {
                    Log.LogDebug("playback finished");
                },
            };
            ReelManager.StartSession(startOption).Forget();
        }

        private async UniTask OnRecordDurationReached(bool isEnded)
        {
            await OnRecord(false);
        }

        private UniTask<byte[][]> GetMusicMotion(string url)
        {
            return musicToMotionGenerator.GenerateMotionFromUrl(url);
        }

        private void ResetToBaseFootage()
        {
            footage = isCreate ? null : baseFootage.ToArray();
        }
    }
}
