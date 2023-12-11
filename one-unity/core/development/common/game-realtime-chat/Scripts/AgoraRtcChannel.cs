using System;
using System.Collections.Generic;
using Agora.Rtc;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.RealtimeChat
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:Field names should not use Hungarian notation", Justification = "Reviewed")]
    public class AgoraRtcChannel : IChannel
    {
        private readonly ILogger _logger;
        private readonly IService _rtcService;
        private readonly ChannelId _channelId;
        private readonly string _localXRSocialId;
        private readonly AgoraRtcTokenProvider _tokenProvider;
        private readonly HashSet<uint> _pendingParticipantUids = new HashSet<uint>();
        private readonly Dictionary<uint, IParticipant> _participants = new Dictionary<uint, IParticipant>();
        private readonly RtcEngineEventHandler _rtcEventHandler;
        private readonly ChannelMediaOptions _defaultChannelMediaOptions;

        private IRtcEngineEx _rtcEngine;
        private RtcConnection _rtcConnection;
        private CONNECTION_STATE_TYPE _connectionState = CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED;
        private ChannelState _channelState = ChannelState.Left;
        private uint _localUserUid = 0;
        private bool _disposed = false;

        public AgoraRtcChannel(
            ChannelId id,
            uint localUserUid,
            string xrSocialId,
            ILogger logger,
            IRtcEngineEx rtcEngine,
            RtcEngineEventHandler rtcEngineEventHandler,
            IService rtcService,
            AgoraRtcTokenProvider tokenProvider)
        {
            _logger = logger;
            _rtcService = rtcService;
            _rtcEngine = rtcEngine;
            _rtcEventHandler = rtcEngineEventHandler;
            _tokenProvider = tokenProvider;

            _channelId = id;
            _localXRSocialId = xrSocialId;
            _localUserUid = localUserUid;
            _rtcConnection = new RtcConnection(_channelId.ChannelName, _localUserUid);

            // Initailize default channel media options
            _defaultChannelMediaOptions = new ChannelMediaOptions();
            _defaultChannelMediaOptions.autoSubscribeAudio.SetValue(true);
            _defaultChannelMediaOptions.publishMicrophoneTrack.SetValue(true);
            _defaultChannelMediaOptions.enableAudioRecordingOrPlayout.SetValue(true);
            _defaultChannelMediaOptions.channelProfile.SetValue(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING);
            _defaultChannelMediaOptions.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

            _tokenProvider.OnReceivedNewToken += OnReceivedNewToken;
            RegisterRtcEventHandlers();
        }

        public event OnJoinChannelHandler OnChannelJoined;

        public event OnLeaveChannelHandler OnChannelLeft;

        public event UserJoinedEventHandler OnUserJoinedChannel;

        public event UserLeftEventHandler OnUserLeftChannel;

        public ChannelId Id => _channelId;

        public ChannelState State => _channelState;

        public IEnumerable<IParticipant> Participants => _participants.Values;

        public void Join()
        {
            if (_disposed)
            {
                return;
            }

            _channelState = ChannelState.Joining;
            if (!string.IsNullOrEmpty(_tokenProvider.Token))
            {
                JoinChannel(_tokenProvider.Token);
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"{nameof(Join)}:: Channel token is empty. Generate a new token for join.");
                }

                _tokenProvider.GenerateNewToken();
            }
        }

        public void Leave()
        {
            if (_disposed)
            {
                return;
            }

            _channelState = ChannelState.Leaving;

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(Leave)}:: channelId={_channelId.ChannelName}");
            }

            int result = _rtcEngine.LeaveChannelEx(_rtcConnection);
            if (result < 0 && _logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError($"LeaveChannelEx failed: {result}");
            }
        }

        public bool MuteLocalAudio(bool isMute)
        {
            var connectionState = _rtcEngine.GetConnectionStateEx(_rtcConnection);
            if (connectionState == CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED ||
                connectionState == CONNECTION_STATE_TYPE.CONNECTION_STATE_FAILED)
            {
                return false;
            }

            var options = new ChannelMediaOptions();
            options.publishMicrophoneTrack.SetValue(!isMute);
            int result = _rtcEngine.UpdateChannelMediaOptionsEx(options, _rtcConnection);
            if (result < 0)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"UpdateChannelMediaOptionsEx failed: {result}");
                }

                return false;
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_tokenProvider != null)
                {
                    _tokenProvider.OnReceivedNewToken -= OnReceivedNewToken;
                    _tokenProvider?.Dispose();
                }

                UnregisterRtcHandlers();

                _rtcEngine?.LeaveChannelEx(_rtcConnection);
                _rtcConnection = null;
                _rtcEngine = null;
            }

            _disposed = true;
        }

        private void JoinChannel(string token)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(JoinChannel)}:: channelName={_rtcConnection.channelId}");
            }

            if (_channelState != ChannelState.Joining)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning($"{nameof(JoinChannel)}:: Invalid channel state. channelName={_rtcConnection.channelId} state={_channelState}");
                }

                return;
            }

            bool isMicMute = _rtcService.GetAudioInputMuted();
            _defaultChannelMediaOptions.publishMicrophoneTrack.SetValue(!isMicMute);

            int result = _rtcEngine.JoinChannelEx(token, _rtcConnection,  _defaultChannelMediaOptions);
            if (result < 0)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"JoinChannelEx failed: {result}");
                }
            }
        }

        private void RejoinChannel()
        {
            _channelState = ChannelState.Joining;
            _tokenProvider.GenerateNewToken();
        }

        private void RegisterRtcEventHandlers()
        {
            if (_rtcEventHandler != null)
            {
                // User events
                _rtcEventHandler.EventOnUserInfoUpdated += RtcEngine_OnUserInfoUpdated;
                _rtcEventHandler.EventOnLocalUserRegistered += RtcEngine_OnLocalUserRegistered;

                // Channel events
                _rtcEventHandler.EventOnConnectionStateChanged += Channel_OnConnectionStateChanged;
                _rtcEventHandler.EventOnConnectionLost += Channel_OnConnectionLost;
                _rtcEventHandler.EventOnTokenPrivilegeWillExpire += Channel_OnTokenPrivilegeWillExpire;
                _rtcEventHandler.EventOnRequestToken += Channel_OnRequestToken;
                _rtcEventHandler.EventOnJoinChannelSuccess += Channel_OnJoinChannel;
                _rtcEventHandler.EventOnLeaveChannel += Channel_OnLeaveChannel;
                _rtcEventHandler.EventOnUserJoined += Channel_OnRemoteUserJoined;
                _rtcEventHandler.EventOnUserOffline += Channel_OnRemoteUserOffline;
                _rtcEventHandler.EventOnError += Channel_OnError;
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"{nameof(RegisterRtcEventHandlers)}:: No rtcEventHandler !!!");
                }
            }
        }

        private void UnregisterRtcHandlers()
        {
            if (_rtcEventHandler != null)
            {
                // User events
                _rtcEventHandler.EventOnUserInfoUpdated -= RtcEngine_OnUserInfoUpdated;
                _rtcEventHandler.EventOnLocalUserRegistered -= RtcEngine_OnLocalUserRegistered;

                // Channel events
                _rtcEventHandler.EventOnConnectionStateChanged -= Channel_OnConnectionStateChanged;
                _rtcEventHandler.EventOnConnectionLost -= Channel_OnConnectionLost;
                _rtcEventHandler.EventOnTokenPrivilegeWillExpire -= Channel_OnTokenPrivilegeWillExpire;
                _rtcEventHandler.EventOnRequestToken -= Channel_OnRequestToken;
                _rtcEventHandler.EventOnJoinChannelSuccess -= Channel_OnJoinChannel;
                _rtcEventHandler.EventOnLeaveChannel -= Channel_OnLeaveChannel;
                _rtcEventHandler.EventOnUserJoined -= Channel_OnRemoteUserJoined;
                _rtcEventHandler.EventOnUserOffline -= Channel_OnRemoteUserOffline;
                _rtcEventHandler.EventOnError -= Channel_OnError;
            }
        }

        private void RtcEngine_OnUserInfoUpdated(uint uid, UserInfo userInfo)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(RtcEngine_OnUserInfoUpdated)}:: uid={uid}, userXRSocialId={userInfo.userAccount}");
            }

            if (_pendingParticipantUids.Remove(uid))
            {
                HandleOnUserJoined(uid);
            }
        }

        private void RtcEngine_OnLocalUserRegistered(uint uid, string userAccount)
        {
            _logger.LogDebug($"{nameof(RtcEngine_OnLocalUserRegistered)}:: uid={uid}, userAccount={userAccount}");

            // Refresh local user uid
            if (_localUserUid != uid)
            {
                _logger.LogWarning($"{nameof(RtcEngine_OnLocalUserRegistered)}:: Refresh local user uid. old uid= {_localUserUid}, new uid= {uid}");
                _localUserUid = uid;
                _rtcConnection = new RtcConnection(_channelId.ChannelName, _localUserUid);
            }
        }

        private void Channel_OnConnectionStateChanged(RtcConnection connection, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            _logger.LogDebug($"{nameof(Channel_OnConnectionStateChanged)}:: channelId={connection.channelId}, state={state}, reason={reason}");
            _connectionState = state;

            if (_connectionState == CONNECTION_STATE_TYPE.CONNECTION_STATE_FAILED)
            {
                _rtcEngine.LeaveChannelEx(connection);
            }
        }

        private void Channel_OnConnectionLost(RtcConnection connection)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            _logger.LogDebug($"{nameof(Channel_OnConnectionLost)}:: channelId={connection.channelId}");
        }

        private void Channel_OnTokenPrivilegeWillExpire(RtcConnection connection, string token)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            // When the token is about to expire in 30 seconds, the SDK triggers this callback to remind the app to renew the token.
            _logger.LogDebug($"{nameof(Channel_OnTokenPrivilegeWillExpire)}:: channelId={connection.channelId}");
            RejoinChannel();
        }

        private void Channel_OnRequestToken(RtcConnection connection)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            // When the token expires during a call, the SDK triggers this callback to remind the app to renew the token.
            _logger.LogDebug($"{nameof(Channel_OnRequestToken)}:: channelId={connection.channelId}");
            RejoinChannel();
        }

        private void Channel_OnJoinChannel(RtcConnection connection, int elapsed)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            _logger.LogDebug($"{nameof(Channel_OnJoinChannel)}:: channelId={connection.channelId}, uid={connection.localUid}, elapsed={elapsed}");

            _rtcEngine.EnableAudioVolumeIndicationEx(500, 3, true, connection);
            _channelState = ChannelState.Joined;

            // Use an empty dictionary upon joining the channel.
            _localUserUid = connection.localUid;
            HandleOnUserJoined(_localUserUid);
            OnChannelJoined?.Invoke(connection.channelId, connection.localUid, _localXRSocialId);
        }

        private void Channel_OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            _logger.LogDebug($"{nameof(Channel_OnLeaveChannel)}:: channelId={connection.channelId}, duration={stats.duration}, tx={stats.txBytes}, rx={stats.rxBytes}, tx kbps={stats.txKBitRate}, rx kbps={stats.rxKBitRate}");
            _channelState = ChannelState.Left;

            // Handle local user left
            if (_participants.Remove(_localUserUid, out IParticipant participant))
            {
                OnUserLeftChannel?.Invoke(participant);
            }

            OnChannelLeft?.Invoke(connection.channelId);

            // Once leaving the channel we are not able to update the dictionary. Invalidate it.
            _participants.Clear();
            _pendingParticipantUids.Clear();
            _localUserUid = 0;
        }

        private void Channel_OnRemoteUserJoined(RtcConnection connection, uint remoteUid, int elapsed)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            _logger.LogDebug($"{nameof(Channel_OnRemoteUserJoined)}:: channelId={connection.channelId}, uid={remoteUid}, elapsed={elapsed}");

            HandleOnUserJoined(remoteUid);
        }

        private void Channel_OnRemoteUserOffline(RtcConnection connection, uint remoteUid, USER_OFFLINE_REASON_TYPE reason)
        {
            if (connection.channelId != _rtcConnection.channelId)
            {
                return;
            }

            _logger.LogDebug($"{nameof(Channel_OnRemoteUserOffline)}:: channelId={connection.channelId}, uid={remoteUid}, reason={reason}");

            HandleOnUserLeft(remoteUid);
        }

        private void Channel_OnError(int err, string msg)
        {
            string description = _rtcEngine.GetErrorDescription(err);
            _logger.LogError($"{nameof(Channel_OnError)}:: error={err}, msg={msg}, description={description}");
        }

        private void WaitForJoinedUserRegistered(uint uid)
        {
            _logger.LogDebug($"{nameof(WaitForJoinedUserRegistered)}:: Waiting for {uid} registered");
            _pendingParticipantUids.Add(uid);
        }

        private void HandleOnUserJoined(uint uid)
        {
            var userInfo = new UserInfo();
            _rtcEngine.GetUserInfoByUidEx(uid, ref userInfo, _rtcConnection);
            var xrSocialId = userInfo.userAccount;
            if (string.IsNullOrEmpty(xrSocialId))
            {
                WaitForJoinedUserRegistered(uid);
            }
            else
            {
                _logger.LogDebug($"{nameof(HandleOnUserJoined)}:: {nameof(uid)}={uid}, {nameof(xrSocialId)}={xrSocialId}");
                var participant = new Participant(this, uid, xrSocialId);
                _participants[uid] = participant;
                OnUserJoinedChannel?.Invoke(participant);
            }
        }

        private void HandleOnUserLeft(uint uid)
        {
            var userInfo = new UserInfo();
            _rtcEngine.GetUserInfoByUidEx(uid, ref userInfo, _rtcConnection);
            var xrSocialId = userInfo.userAccount;
            if (string.IsNullOrEmpty(xrSocialId) && !_pendingParticipantUids.Contains(uid))
            {
                _logger.LogWarning($"{nameof(HandleOnUserLeft)}:: User XRSocialId for {uid} is not available");
            }

            _pendingParticipantUids.Remove(uid);

            _logger.LogDebug($"{nameof(HandleOnUserLeft)}:: {nameof(uid)}={uid}, {nameof(xrSocialId)}={xrSocialId}");
            if (_participants.Remove(uid, out IParticipant participant))
            {
                OnUserLeftChannel?.Invoke(participant);
            }
        }

        private void OnReceivedNewToken(string token)
        {
            if (_connectionState == CONNECTION_STATE_TYPE.CONNECTION_STATE_CONNECTED)
            {
                _rtcEngine.RenewToken(token);
            }
            else
            {
                if (_channelState == ChannelState.Joining)
                {
                    JoinChannel(token);
                }
            }
        }
    }
}
