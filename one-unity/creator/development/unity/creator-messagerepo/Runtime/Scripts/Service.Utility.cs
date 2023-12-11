namespace TPFive.Creator.MessageRepo
{
    public partial class Service
    {
        private static IServiceProvider ConvertToServiceProvider(Game.IServiceProvider sp) =>
            sp is IServiceProvider provider ? provider : default;
    }
}
