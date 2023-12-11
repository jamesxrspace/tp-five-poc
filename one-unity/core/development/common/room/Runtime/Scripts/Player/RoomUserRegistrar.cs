using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine.Assertions;
using XRSpace.OpenAPI;

namespace TPFive.Room
{
    /// <summary>
    /// RoomUserRegistrar is responsible for synchronizing the users in rooms with GameServer.
    /// </summary>
    public class RoomUserRegistrar : IRoomUserRegistrar
    {
        private IRoomApi roomApi;
        private ILogger logger;

        private Queue<Work> workQueue = new ();

        public RoomUserRegistrar(IRoomApi roomApi, ILoggerFactory loggerFactory)
        {
            this.roomApi = roomApi;
            logger = loggerFactory.CreateLogger<RoomUserRegistrar>();
        }

        /// <summary>
        /// Register player into the room he belongs to with GameServer.
        /// </summary>
        /// <param name="player">The player to register.</param>
        public void Register(IPlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("The given player is null.");
            }

            if (!player.HasStateAuthority)
            {
                throw new ArgumentNullException("The given player has no state authority.");
            }

            AddWork(new RegisterWork(roomApi, player));
        }

        /// <summary>
        /// Unregister player from the room he used to belong to with GameServer.
        /// </summary>
        /// <param name="player">The player to unregister.</param>
        public void Unregister(IPlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("The given player is null.");
            }

            if (!player.HasStateAuthority)
            {
                throw new ArgumentNullException("The given player has no state authority.");
            }

            AddWork(new UnregisterWork(roomApi, player));
        }

        private void AddWork(Work work)
        {
            workQueue.Enqueue(work);
            if (workQueue.Count == 1)
            {
                ProcessWork(workQueue.Peek());
            }
        }

        private void ProcessWork(Work work)
        {
            // Handler for work completion.
            void OnCompleted(bool isSuccess, int httpStatusCode, string errMsg)
            {
                if (!isSuccess)
                {
                    logger.LogWarning("Received error from GameServer: {Code} {Msg}", httpStatusCode, errMsg);
                }

                workQueue.Dequeue();
                if (workQueue.Count > 0)
                {
                    ProcessWork(workQueue.Peek());
                }
            }

            // Start processing the work.
            work.Process(OnCompleted);
        }

        private abstract class Work
        {
            public Work(IRoomApi roomApi, IPlayer player)
            {
                RoomApi = roomApi;
                Registry = new () { UserId = player.XRId, RoomId = player.RoomID, SpaceId = player.SpaceID };
            }

            public bool IsProcessing { get; private set; } = false;

            private IRoomApi RoomApi { get; set; }

            private RoomUserRegistry Registry { get; set; }

            private Action<bool, int, string> OnCompletedCallback { get; set; }

            public void Process(Action<bool, int, string> onCompletedCallback)
            {
                if (IsProcessing)
                {
                    throw new Exception("Try to process an in progress work");
                }

                OnCompletedCallback = onCompletedCallback;
                IsProcessing = true;
                Process(RoomApi, Registry);
            }

            protected abstract void Process(IRoomApi roomApi, RoomUserRegistry registry);

            protected void OnCompleted(IResponse response)
            {
                Assert.IsTrue(IsProcessing, "An idle work is being completed.");
                IsProcessing = false;
                var onCompletedCallbackTemp = OnCompletedCallback;
                OnCompletedCallback = null;
                onCompletedCallbackTemp?.Invoke(response.IsSuccess, response.HttpStatusCode, response.Message);
            }
        }

        private class RegisterWork : Work
        {
            public RegisterWork(IRoomApi roomApi, IPlayer player)
            : base(roomApi, player)
            {
            }

            protected override void Process(IRoomApi roomApi, RoomUserRegistry registry)
            {
                roomApi.RegisterRoomUserAsync(registry).ContinueWith((task) => OnCompleted(task.Result));
            }
        }

        private class UnregisterWork : Work
        {
            public UnregisterWork(IRoomApi roomApi, IPlayer player)
            : base(roomApi, player)
            {
            }

            protected override void Process(IRoomApi roomApi, RoomUserRegistry registry)
            {
                roomApi.UnregisterRoomUserAsync(registry).ContinueWith((task) => OnCompleted(task.Result));
            }
        }
    }
}