using System;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [Serializable]
    public class EntitySpawnerSetting
    {
        [SerializeField]
        private CategoriesEnum[] categories;

        [SerializeField]
        private SpawnPointGroup spawnSpawnPointGroup;

        [SerializeField]
        private EntityDataGroup dataGroup;
        [SerializeField]
        private SpawnPoint spawnPointForMe;

        public CategoriesEnum[] Categories => categories;

        public SpawnPointGroup SpawnPointGroup => spawnSpawnPointGroup;

        public EntityDataGroup DataGroup => dataGroup;

        public SpawnPoint SpawnPointForMe => spawnPointForMe;
    }
}