using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;

namespace TPFive.Room
{
    public interface IPlayerSystem
    {
        event Action<IPlayer> OnPlayerJoined;

        event Action<IPlayer> OnPlayerLeft;

        int PlayerCount { get; }

        IPlayer LocalPlayer { get; }

        void Register(IPlayer player);

        void Unregister(IPlayer player);

        IPlayer GetPlayer(string xrId);
    }

    public class PlayerSystem : IPlayerSystem
    {
        [Inject]
        private ILoggerFactory loggerFactory;
        private ILogger<PlayerSystem> logger;
        private Dictionary<string, IPlayer> players = new ();

        public event Action<IPlayer> OnPlayerJoined;

        public event Action<IPlayer> OnPlayerLeft;

        public ILogger<PlayerSystem> Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = loggerFactory.CreateLogger<PlayerSystem>();
                }

                return logger;
            }
        }

        public int PlayerCount => players.Count;

        public IPlayer LocalPlayer { get; private set; }

        public void Register(IPlayer player)
        {
            if (player == null)
            {
                throw new Exception($"{nameof(PlayerSystem)}.{nameof(Register)}: Try to register a null player.");
            }

            if (!player.IsReady)
            {
                throw new Exception($"{nameof(PlayerSystem)}.{nameof(Register)}: Try to register an unready player.");
            }

            if (players.ContainsKey(player.XRId))
            {
                throw new Exception($"{nameof(PlayerSystem)}.{nameof(Register)}: Try to register player({player.XRId}) for more than once");
            }

            players[player.XRId] = player;
            if (player.IsLocalPlayer)
            {
                if (LocalPlayer != null)
                {
                    throw new Exception($"{nameof(PlayerSystem)}.{nameof(Register)}: More than one local players on a single peer({LocalPlayer.XRId} & {player.XRId})");
                }

                LocalPlayer = player;
            }

            OnPlayerJoined?.Invoke(player);
        }

        public void Unregister(IPlayer player)
        {
            if (player == null)
            {
                throw new Exception($"{nameof(PlayerSystem)}.{nameof(Unregister)}: Try to unregister a null player.");
            }

            if (!players.Remove(player.XRId))
            {
                throw new Exception($"{nameof(PlayerSystem)}.{nameof(Unregister)}: Try to unregister non-registered player({player.XRId}).");
            }

            if (LocalPlayer != null && LocalPlayer.XRId == player.XRId)
            {
                LocalPlayer = null;
            }

            OnPlayerLeft?.Invoke(player);
        }

        public IPlayer GetPlayer(string xrId)
        {
            players.TryGetValue(xrId, out var player);
            return player;
        }
    }
}
