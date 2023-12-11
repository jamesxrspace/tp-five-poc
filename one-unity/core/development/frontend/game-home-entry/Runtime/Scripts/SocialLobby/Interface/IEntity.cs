namespace TPFive.Home.Entry.SocialLobby
{
    public interface IEntity : ILayout, IFeedProvider
    {
        bool IsVisible { get; set; }

        void InitializeVisibility();
    }
}