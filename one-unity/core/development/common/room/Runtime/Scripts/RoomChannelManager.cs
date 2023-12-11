using System;
using Cysharp.Threading.Tasks;
using Fusion;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.Messages;
using TPFive.SCG.DisposePattern.Abstractions;
using UnityEngine;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Room
{
    public interface IChannelManager
    {
        Game.RealtimeChat.IChannel MainRtcChannel { get; }
    }

    [Dispose]
    public partial class RoomChannelManager : IChannelManager, IStartable
    {
        private const int DefaultMinimumPlayerCountForChannelJoin = 2;

        private readonly ILogger _logger;
        private readonly Game.RealtimeChat.IService _rtcService;
        private readonly IPlayerSystem _playerSystem;
        private readonly IRoomManager _roomManager;
        private readonly IDisposable _appPauseSubscription;

        private Game.RealtimeChat.IChannel _mainRtcChannel;
        private IRoom _currentRoom;
        private bool _isHostPlayerInRoom;

        public RoomChannelManager(
            ILoggerFactory loggerFactory,
            Game.RealtimeChat.IService service,
            IPlayerSystem playerSystem,
            IRoomManager roomManager,
            ISubscriber<ApplicationPause> appPauseSubscriber)
        {
            _logger = Game.Logging.Utility.CreateLogger<Game.RealtimeChat.Service>(loggerFactory);
            _rtcService = service;
            _playerSystem = playerSystem;
            _roomManager = roomManager;

            _playerSystem.OnPlayerJoined += OnPlayerJoined;
            _playerSystem.OnPlayerLeft += OnPlayerLeft;
            _appPauseSubscription = appPauseSubscriber.Subscribe(OnApplicationPause);
        }

        public Game.RealtimeChat.IChannel MainRtcChannel => _mainRtcChannel;

#if UNITY_EDITOR
        private static bool IsEnableToJoinChannelWithOnePlayer
        {
            get => UnityEditor.EditorPrefs.GetBool("JoinChannelWithOnePlayer", false);
            set => UnityEditor.EditorPrefs.SetBool("JoinChannelWithOnePlayer", value);
        }
#endif

        void IStartable.Start()
        {
            _logger.LogDebug("Init room channel manager");
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("TPFive/Rtc/Enable To JoinChannel With One Player")]
        private static void EnableToJoinChannelWithOnePlayer()
        {
            IsEnableToJoinChannelWithOnePlayer = true;
        }

        [UnityEditor.MenuItem("TPFive/Rtc/Enable To JoinChannel With One Player", validate = true)]
        private static bool ValidateEnableToJoinChannelWithOnePlayer()
        {
            return !IsEnableToJoinChannelWithOnePlayer;
        }

        [UnityEditor.MenuItem("TPFive/Rtc/Disable To JoinChannel With One Player")]
        private static void DisableToJoinChannelWithOnePlayer()
        {
            IsEnableToJoinChannelWithOnePlayer = false;
        }

        [UnityEditor.MenuItem("TPFive/Rtc/Disable To JoinChannel With One Player", validate = true)]
        private static bool ValidateDisableToJoinChannelWithOnePlayer()
        {
            return IsEnableToJoinChannelWithOnePlayer;
        }
#endif

        private int GetMinimumPlayerCountForChannelJoin()
        {
            var count = DefaultMinimumPlayerCountForChannelJoin;
#if UNITY_EDITOR
            if (IsEnableToJoinChannelWithOnePlayer)
            {
                count = 1;
            }
#endif
            return count;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _appPauseSubscription?.Dispose();

                if (_playerSystem != null)
                {
                    _playerSystem.OnPlayerJoined -= OnPlayerJoined;
                    _playerSystem.OnPlayerLeft -= OnPlayerLeft;
                }

                if (_mainRtcChannel != null)
                {
                    _rtcService.ReleaseChannel(_mainRtcChannel.Id);
                    _mainRtcChannel = null;
                }
            }

            _disposed = true;
        }

        private void OnApplicationPause(ApplicationPause appPause)
        {
            if (appPause.Pause)
            {
                LeaveChannel();
            }
            else
            {
                if (_isHostPlayerInRoom && _playerSystem.PlayerCount >= GetMinimumPlayerCountForChannelJoin())
                {
                    JoinChannel();
                }
            }
        }

        private void OnPlayerJoined(IPlayer player)
        {
            void PlayerJoinChannel()
            {
                if (player.IsLocalPlayer)
                {
                    _isHostPlayerInRoom = true;
                }

                if (_isHostPlayerInRoom && _playerSystem.PlayerCount >= GetMinimumPlayerCountForChannelJoin())
                {
                    JoinChannel();
                }
            }

            // Get current room when player joined to create channel for the room
            if (_currentRoom == null)
            {
                if (_roomManager.Mode != GameMode.Server)
                {
                    _currentRoom = _roomManager.Room;

                    CreateChannel($"{_currentRoom.Region}-{_currentRoom.RoomID}").ContinueWith(() =>
                    {
                        PlayerJoinChannel();
                    });
                }
            }
            else
            {
                PlayerJoinChannel();
            }
        }

        private void OnPlayerLeft(IPlayer player)
        {
            if (player.IsLocalPlayer)
            {
                _isHostPlayerInRoom = false;
            }

            if (!_isHostPlayerInRoom || _playerSystem.PlayerCount < GetMinimumPlayerCountForChannelJoin())
            {
                LeaveChannel();
            }
        }

        private async UniTask CreateChannel(string channelName)
        {
            var currentRoomChannelId = new Game.RealtimeChat.ChannelId(channelName);
            if (_mainRtcChannel == null)
            {
                _mainRtcChannel = await _rtcService.CreateChannel(currentRoomChannelId);
            }
            else if (_mainRtcChannel.Id != currentRoomChannelId)
            {
                _logger.LogWarning($"Room Channel Replaced: OLD channel id= {_mainRtcChannel.Id}. NEW channel id= {_mainRtcChannel.Id}.");

                // Release old
                _rtcService.ReleaseChannel(_mainRtcChannel.Id);

                // Create new
                _mainRtcChannel = await _rtcService.CreateChannel(currentRoomChannelId);
            }
        }

        private void JoinChannel()
        {
            if (_mainRtcChannel == null)
            {
                return;
            }

            RequestMicrophonePermission().ContinueWith(Join);

            void Join()
            {
                if (_mainRtcChannel.State != Game.RealtimeChat.ChannelState.Joined &&
                    _mainRtcChannel.State != Game.RealtimeChat.ChannelState.Joining)
                {
                    _mainRtcChannel.Join();
                }
            }
        }

        private UniTask RequestMicrophonePermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
            {
                UnityEngine.Android.Permission.RequestUserPermissions(new string[] { UnityEngine.Android.Permission.Microphone });
            }
#else
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                return Application.RequestUserAuthorization(UserAuthorization.Microphone).ToUniTask();
            }
#endif
            return UniTask.CompletedTask;
        }

        private void LeaveChannel()
        {
            if (_mainRtcChannel == null)
            {
                return;
            }

            if (_mainRtcChannel.State != Game.RealtimeChat.ChannelState.Left &&
                _mainRtcChannel.State != Game.RealtimeChat.ChannelState.Leaving)
            {
                _mainRtcChannel.Leave();
            }
        }
    }
}
