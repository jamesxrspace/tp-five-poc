#if UNITY_SERVER
using System;
using System.Threading.Tasks;
using Fusion;
using Microsoft.Extensions.Logging;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Room
{
    public class UnityMultiplayRoomManager<TRoom> : MultiplayEventCallbacks, IRoomManager, IInitializable, IStartable, ITickable, IDisposable
        where TRoom : IRoom, new()
    {
        private static readonly int KDefaultTargetFrameRate = 30;
        private static readonly ushort KMaxPlayers = 200;
        private static readonly string KServerName = "fusion-01";
        private static readonly string KGameType = string.Empty;
        private static readonly string KBuildId = string.Empty;
        private static readonly string KDefaultMap = string.Empty;

        [Inject]
        private RoomSetting roomSetting;
        [Inject]
        private IOpenRoomCmd openRoomCmd;
        [Inject]
        private IObjectResolver objectResolver;
        [Inject]
        private ILoggerFactory loggerFactory;
        private ILogger<UnityMultiplayRoomManager<TRoom>> logger;
        private IServerQueryHandler serverQueryHandler;
        private IServerEvents serverEvents;

        public GameMode Mode { get => GameMode.Server; }

        public IRoom Room { get; private set; }

        public bool IsReady { get; private set; } = false;

        public ServerConfig ServerConfig { get; private set; }

        private ILogger<UnityMultiplayRoomManager<TRoom>> Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = loggerFactory.CreateLogger<UnityMultiplayRoomManager<TRoom>>();
                }

                return logger;
            }
        }

        void IInitializable.Initialize()
        {
            SetTargetFrameRate();
        }

        void IStartable.Start()
        {
            _ = StartAsync();
        }

        void ITickable.Tick()
        {
            if (!IsReady)
            {
                return;
            }

            serverQueryHandler?.UpdateServerCheck();
        }

        void IDisposable.Dispose()
        {
            Room?.Dispose();
            Room = null;
        }

        private async Task<bool> StartAsync()
        {
            if (!await InitUnityServicesAsync())
            {
                return false;
            }

            if (!await InitServerQueryHandlerAsync())
            {
                return false;
            }

            if (!await SubscribeToServerEventsAsync())
            {
                return false;
            }

            if (!FetchServerConfig())
            {
                return false;
            }

            IsReady = true;
            Logger.LogInformation("Server has been ready.");
            return true;
        }

        private async Task<bool> InitUnityServicesAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                return true;
            }
            catch (Exception e)
            {
                Log(nameof(InitUnityServicesAsync), e);
                return false;
            }
        }

        private async Task<bool> InitServerQueryHandlerAsync()
        {
            try
            {
                serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(KMaxPlayers, KServerName, KGameType, KBuildId, KDefaultMap);
                return true;
            }
            catch (Exception ex)
            {
                Log(nameof(InitServerQueryHandlerAsync), ex);
                return false;
            }
        }

        private bool FetchServerConfig()
        {
            try
            {
                ServerConfig = MultiplayService.Instance.ServerConfig;
                return true;
            }
            catch (Exception e)
            {
                Log(nameof(FetchServerConfig), e);
                return false;
            }
        }

        private async Task<bool> SubscribeToServerEventsAsync()
        {
            try
            {
                this.Allocate += OnAllocate;
                this.Deallocate += OnDeallocate;
                this.Error += OnMultiplayError;
                serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                Log(nameof(SubscribeToServerEventsAsync), ex);
                this.Allocate -= OnAllocate;
                this.Deallocate -= OnDeallocate;
                this.Error -= OnMultiplayError;
                return false;
            }
        }

        private async Task<bool> ReadyServerForPlayerAsync()
        {
            try
            {
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
                Logger.LogInformation("Server is ready for receiving players.");
                return true;
            }
            catch (Exception ex)
            {
                Log(nameof(ReadyServerForPlayerAsync), ex);
                return false;
            }
        }

        private void Log(string context, Exception e)
        {
            var aggregateExp = e.InnerException as AggregateException;
            var innerExps = aggregateExp?.InnerExceptions;
            if (innerExps != null && innerExps.Count > 0)
            {
                Logger.LogError("{Context}: {ErrMsg}", context, innerExps[0].Message);
            }
            else
            {
                Logger.LogError("{Context}: {ErrMsg}", context, e.Message);
            }
        }

        private void OnAllocate(MultiplayAllocation allocation)
        {
            Logger.LogInformation("Server receives allocation.");
            if (!IsReady)
            {
                Logger.LogError("Server receives allocation while it is not ready yet.");
                return;
            }

            if (Room != null)
            {
                Logger.LogError("Server receives allocation while it has been allocated.");
                return;
            }

            // Fetch content of the room-opening command
            if (!openRoomCmd.Fetch(out var cmdContent))
            {
                throw new Exception("Failed fetching room-opening command");
            }

            Room = new TRoom();
            Room.OnStarted += OnRoomStarted;
            objectResolver.Inject(Room);
            Room.Open(Mode, cmdContent.SpaceID, cmdContent.RoomID, cmdContent.SceneKey, cmdContent.Region, ServerConfig.Port);
        }

        private void OnDeallocate(MultiplayDeallocation allocation)
        {
        }

        private void OnMultiplayError(MultiplayError error)
        {
            Logger.LogError("Multiplay Error: {ErrMsg}", error.Detail);
        }

        private void OnRoomStarted()
        {
            _ = ReadyServerForPlayerAsync();
        }

        private void SetTargetFrameRate()
        {
            Application.targetFrameRate = roomSetting != null ? roomSetting.ServerTargetFrameRate : KDefaultTargetFrameRate;
            Logger.LogInformation("Set target frame rate to {FrameRate}", Application.targetFrameRate);
        }
    }
}
#endif