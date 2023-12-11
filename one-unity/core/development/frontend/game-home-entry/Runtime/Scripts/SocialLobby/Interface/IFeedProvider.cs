using TPFive.Home.Entry.SocialLobby.Enum;
using TPFive.OpenApi.GameServer.Model;

namespace TPFive.Home.Entry.SocialLobby
{
    public interface IFeedProvider
    {
        CategoriesEnum[] Categories { get; }
    }
}