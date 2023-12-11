using System;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [Serializable]
    public class MockEntityDataConfig
    {
        [SerializeField]
        private Vector3 size;
        [SerializeField]
        private int sortOrder;
        [SerializeField]
        private int count;
        [SerializeField]
        [Multiline]
        private string message;
        [SerializeField]
        private string nickname;
        [SerializeField]
        private string thumbnailUrl;
        [SerializeField]
        private CategoriesEnum[] categories;

        public Vector3 Size => size;

        public int SortOrder => sortOrder;

        public int Count => count;

        public string Message => message;

        public string Nickname => nickname;

        public string ThumbnailUrl => thumbnailUrl;

        public CategoriesEnum[] Categories => categories;
    }
}