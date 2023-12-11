using System;
using System.Collections.Generic;
using TPFive.OpenApi.GameServer.Model;

namespace TPFive.Home.Entry.SocialLobby
{
    /// <summary>
    /// A struct to store openAPI "GetSocialLobbyFeedByFeedTag" parameters.
    /// </summary>
    [Serializable]
    public struct FeedQueryParameter
    {
        public int Offset;
        public int Size;
        public List<CategoriesEnum> Categories;
    }
}