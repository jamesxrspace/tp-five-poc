namespace TPFive.Game.RealtimeChat
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Agora.Rtc;
    using Cysharp.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Logging;
    using TPFive.OpenApi.GameServer;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using VContainer;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [AsyncStartable]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly Login.Options _loginOptions;
        private readonly AgoraRtcConfig _agoraRtcConfig;
        private readonly IAgoraApi _agoraApi;
        private readonly User.IService _userService;
        private readonly Dictionary<ChannelId, IChannel> _channels = new Dictionary<ChannelId, IChannel>();
        private readonly RtcEngineEventHandler _rtcEventHandler = RtcEngineEventHandler.GetInstance();

        private IRtcEngineEx _rtcEngine = null;
        private bool _isLocalUserSpeaking;
        private bool _isAudioInputMuted;
        private uint _localUserUid = 0;

        [Inject]
        public Service(
            Login.Options loginOptions,
            AgoraRtcConfig agoraRtcConfig,
            IAgoraApi agoraApi,
            User.IService userService,
            ILoggerFactory loggerFactory)
        {
            Logger = Utility.CreateLogger<Service>(loggerFactory);
            _loginOptions = loginOptions;
            _agoraRtcConfig = agoraRtcConfig;
            _agoraApi = agoraApi;
            _userService = userService;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);
        }

        public event AudioInputMutedEventHandler OnAudioInputMuted;

        public event LocalUserSpeakingEventHandler OnLocalUserSpeaking;

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        private ILogger Logger { get; set; }

        private bool IsLocalUserSpeaking
        {
            get => _isLocalUserSpeaking;
            set
            {
                if (_isLocalUserSpeaking == value)
                {
                    return;
                }

                if (Logger.IsDebugEnabled())
                {
                    Logger.LogDebug($"The local user {(value ? "begins to speak" : "stops speaking")}");
                }

                _isLocalUserSpeaking = value;
                OnLocalUserSpeaking?.Invoke(value);
            }
        }

        private bool IsAudioInputMuted
        {
            get => _isAudioInputMuted;
            set
            {
                if (_isAudioInputMuted == value)
                {
                    return;
                }

                _isAudioInputMuted = value;
                OnAudioInputMuted?.Invoke(value);

                // Force refresh speaking state
                if (_isAudioInputMuted)
                {
                    IsLocalUserSpeaking = false;
                }
            }
        }

        public void MuteAudioInput(bool isMute)
        {
            // In Agora 4.x version, rtcEngine.EnableLocalAudio only works on one channel
            // for multiple channel, we need to update channel media options
            foreach (var kvp in _channels)
            {
                var result = kvp.Value.MuteLocalAudio(isMute);
                if (!result)
                {
                    if (Logger.IsEnabled(LogLevel.Error))
                    {
                        Logger.LogError($"{nameof(MuteAudioInput)} isMute={isMute} failed channel= {kvp.Key.ChannelName}");
                    }
                }
            }

            IsAudioInputMuted = isMute;
        }

        public bool GetLocalUserSpeaking()
        {
            return IsLocalUserSpeaking;
        }

        public bool GetAudioInputMuted()
        {
            return IsAudioInputMuted;
        }

        public async UniTask<IChannel> CreateChannel(ChannelId channelId)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"{nameof(CreateChannel)}:: channelName={channelId.ChannelName}");
            }

            if (_channels.ContainsKey(channelId))
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                {
                    Logger.LogWarning($"{nameof(CreateChannel)}:: Channel {channelId.ChannelName} already exists.");
                }

                return _channels[channelId];
            }
            else
            {
                var user = await _userService.GetUserAsync();
                var selfXRSocialId = user.Uid;
                var tokenProvider = new AgoraRtcTokenProvider(Logger, _agoraApi, _loginOptions.AppId, channelId.ChannelName);
                IChannel channel = new AgoraRtcChannel(channelId, _localUserUid, selfXRSocialId, Logger, _rtcEngine, _rtcEventHandler, this, tokenProvider);
                _channels.Add(channelId, channel);
                return channel;
            }
        }

        public void ReleaseChannel(ChannelId channelId)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"{nameof(ReleaseChannel)}:: channelName={channelId.ChannelName}");
            }

            if (_channels.TryGetValue(channelId, out IChannel channel))
            {
                channel.Dispose();
                _channels.Remove(channelId);
            }
        }

        private void Setup()
        {
            if (_agoraRtcConfig == null)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    Logger.LogError("AgoraRtcConfig is missing");
                }

                return;
            }

            try
            {
                if (Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("AgoraRtcService:: Create RtcEngine");
                }

                _rtcEngine = RtcEngine.CreateAgoraRtcEngineEx();
                InitializeRtcEngine(_agoraRtcConfig.AppId);
            }
            catch (Exception e)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    Logger.LogError(e, "AgoraRtcService setup failed");
                }

                return;
            }
        }

        private void InitializeRtcEngine(string appId)
        {
            var context = new RtcEngineContext(
                appId,
                0,
                CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_GAME_STREAMING,
                logConfig: new LogConfig(string.Empty, level: LOG_LEVEL.LOG_LEVEL_ERROR));

            _rtcEngine.Initialize(context);
#if UNITY_IOS
            _rtcEngine.SetParameters("{\"che.audio.keep.audiosession\":true}");
#endif
            _rtcEngine.SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_HIGH_QUALITY_STEREO);
            _rtcEngine.EnableSoundPositionIndication(true);

            _rtcEventHandler.EventOnLocalAudioStateChanged += RtcEngine_OnLocalAudioStateChanged;
            _rtcEventHandler.EventOnAudioVolumeIndication += RtcEngine_OnVolumeIndication;
            _rtcEventHandler.EventOnLocalUserRegistered += RtcEngine_OnLocalUserRegistered;
            _rtcEngine.InitEventHandler(_rtcEventHandler);

            // Register user account
            _userService.GetUserAsync().ContinueWith(user =>
            {
                _rtcEngine.RegisterLocalUserAccount(appId, user?.Uid);
            });
        }

        /// <summary>
        /// This callback only triggered when user is in the channel.
        /// When user left channel, audio state will change to STOPPED.
        /// When user join channel, audio state will change to RECORDING if user EnableLocalAudio(true).
        ///                         audio state will stay in STOPPED if user EnableLocalAudio(false).
        /// </summary>
        private void RtcEngine_OnLocalAudioStateChanged(RtcConnection connection, LOCAL_AUDIO_STREAM_STATE state, LOCAL_AUDIO_STREAM_ERROR error)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"{nameof(RtcEngine_OnLocalAudioStateChanged)}:: LOCAL_AUDIO_STREAM_STATE={state}, LOCAL_AUDIO_STREAM_ERROR={error}");
            }
        }

        private void RtcEngine_OnVolumeIndication(RtcConnection connection, AudioVolumeInfo[] speakers, uint speakerNumber, int totalVolume)
        {
            if (speakerNumber == 1 && speakers[0].uid == 0)
            {
                // Local user
                if (IsAudioInputMuted)
                {
                    IsLocalUserSpeaking = false;
                }
                else
                {
                    var speaker = speakers[0];
                    IsLocalUserSpeaking = speaker.vad == 1;
                }
            }
            else
            {
                // Remote users (up to three) whose instantaneous volumes are the highest.
            }
        }

        private void RtcEngine_OnLocalUserRegistered(uint uid, string userAccount)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"{nameof(RtcEngine_OnLocalUserRegistered)}:: uid={uid}, userAccount={userAccount}");
            }

            _localUserUid = uid;
        }

        private void ReleaseAllChannel()
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"{nameof(ReleaseAllChannel)}");
            }

            foreach (var channel in _channels.Values)
            {
                channel.Dispose();
            }

            _channels.Clear();
        }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogEditorDebug("{Method}", nameof(SetupBegin));
            }

            // Setup engine & register local user
            Setup();

            // Wait for LocalUserRegistered
            await UniTask.WaitUntil(() => _localUserUid != 0, cancellationToken: cancellationToken);
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogEditorDebug("{Method}", nameof(SetupEnd));
            }

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncOperationCanceledException(
            OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            if (Logger.IsEnabled(LogLevel.Warning))
            {
                Logger.LogWarning("{Exception}", e);
            }

            await Task.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            Exception e,
            CancellationToken cancellationToken = default)
        {
            if (Logger.IsEnabled(LogLevel.Error))
            {
                Logger.LogError("{Exception}", e);
            }

            await Task.CompletedTask;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_rtcEngine != null)
                {
                    ReleaseAllChannel();

                    _rtcEventHandler.EventOnLocalAudioStateChanged -= RtcEngine_OnLocalAudioStateChanged;
                    _rtcEventHandler.EventOnAudioVolumeIndication -= RtcEngine_OnVolumeIndication;
                    _rtcEventHandler.EventOnLocalUserRegistered -= RtcEngine_OnLocalUserRegistered;
                    _rtcEngine.InitEventHandler(null);
                    _rtcEngine.LeaveChannel(); // Leave all channel
                    _rtcEngine.Dispose();
                }

                _rtcEngine = null;
            }

            _disposed = true;
        }
    }
}
